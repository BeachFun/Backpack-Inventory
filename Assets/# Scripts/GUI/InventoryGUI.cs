using UnityEngine;
using UnityEngine.UI;

namespace BackpackInventory
{
    /// <summary>
    /// Пользовательский интерфейс для отображения содержимого инвентаря
    /// </summary>
    public class InventoryGUI : MonoBehaviour
    {
        [SerializeField] private Image[] slotIcons;

        private Inventory _inventory;


        private void Awake()
        {
            gameObject.SetActive(false);
            _inventory = GetComponentInParent<Inventory>();

            foreach (var item in slotIcons) item.color = new Color(0, 0, 0, 0); // прозрачная картинка

            _inventory?.ItemAddedEvent.AddListener(ReDraw);
            _inventory?.ItemRemovedEvent.AddListener(ReDraw);
        }

        private void Start()
        {
            if (_inventory.Slots.Length != slotIcons.Length) throw new("Кол-во слотов в UI не совпадает с кол-ом слотов в инвентаре");
        }


        /// <summary>
        /// Обновляет сведения инвентаря в GUI на актуальные
        /// </summary>
        public void ReDraw(string id)
        {
            Slot[] slots = _inventory.Slots;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Item is null)
                {
                    slotIcons[i].sprite = null;
                    slotIcons[i].color = new Color(0, 0, 0, 0); // прозрачная картинка
                }
                else
                {
                    slotIcons[i].sprite = slots[i].Item.Config.icon;
                    slotIcons[i].color = Color.white; // стандартная, видимая картинка
                }
            }
        }
    }
}
