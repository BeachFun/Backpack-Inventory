using UnityEngine;
using UnityEngine.UI;

namespace BackpackInventory
{
    /// <summary>
    /// ���������������� ��������� ��� ����������� ����������� ���������
    /// </summary>
    public class InventoryGUI : MonoBehaviour
    {
        [SerializeField] private Image[] slotIcons;

        private Inventory _inventory;


        private void Awake()
        {
            gameObject.SetActive(false);
            _inventory = GetComponentInParent<Inventory>();

            foreach (var item in slotIcons) item.color = new Color(0, 0, 0, 0); // ���������� ��������

            _inventory?.ItemAddedEvent.AddListener(ReDraw);
            _inventory?.ItemRemovedEvent.AddListener(ReDraw);
        }

        private void Start()
        {
            if (_inventory.Slots.Length != slotIcons.Length) throw new("���-�� ������ � UI �� ��������� � ���-�� ������ � ���������");
        }


        /// <summary>
        /// ��������� �������� ��������� � GUI �� ����������
        /// </summary>
        public void ReDraw(string id)
        {
            Slot[] slots = _inventory.Slots;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].Item is null)
                {
                    slotIcons[i].sprite = null;
                    slotIcons[i].color = new Color(0, 0, 0, 0); // ���������� ��������
                }
                else
                {
                    slotIcons[i].sprite = slots[i].Item.Config.icon;
                    slotIcons[i].color = Color.white; // �����������, ������� ��������
                }
            }
        }
    }
}
