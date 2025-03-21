using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

namespace BackpackInventory
{
    /// <summary>
    /// Система инвентаря, предназначенная для хранения объектов в 3д и логически
    /// </summary>
    public class Inventory : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Inventory Config")]
        [Tooltip("Если установлено значение 0, то максимальный вес неограничен")]
        [SerializeField] private int maxTotalWeight = 0; // if 0 then unlimited
        [SerializeField] private Slot[] slots;

        [Header("Binding")]
        [Tooltip("Область в которой объект считается помещенным в инвентарь")]
        [SerializeField] private Collider triggerZone;
        [SerializeField] private InventoryGUI inventoryGUI;


        /// <summary>
        /// Слоты инвентаря
        /// </summary>
        public Slot[] Slots => slots;
        /// <summary>
        /// Текущий вес инвентаря
        /// </summary>
        public int CurrentWeight
        {
            get => slots.Where(x => x.Item is not null).Sum(x => x.Item.Config.weight);
        }
        /// <summary>
        /// Событие добавление предмета в инвентарь
        /// </summary>
        public UnityEvent<string> ItemAddedEvent { get; private set; } = new();
        /// <summary>
        /// Событие удаление предмета из инвентаря
        /// </summary>
        public UnityEvent<string> ItemRemovedEvent { get; private set; } = new();


        public void OnPointerDown(PointerEventData data)
        {
            inventoryGUI?.gameObject.SetActive(true);
            triggerZone.enabled = false;
        }

        public void OnPointerUp(PointerEventData data)
        {
            inventoryGUI?.gameObject.SetActive(false);

            Item item = GetItemUnderCursor();
            if (item is not null)
            {
                item.OnPointerUp(data);
            }

            triggerZone.enabled = true;
        }


        /// <summary>
        /// Добавление предмета в инвентарь
        /// </summary>
        /// <returns>Возвращает true при успешном добавлении</returns>
        public bool PutItem(Item item)
        {
            if (item is null)
                throw new("Item is null");

            ItemType itemType = item.Config.type;

            // Поиск пустого слота по типу, если таких слотов нет, то поиск слотов принимающих любые предметы
            Slot slot =
                slots.Where(x => x.ItemType == itemType && x.Item is null).FirstOrDefault() ??
                slots.Where(x => x.isAny).FirstOrDefault();

            // Добавление предмета в доступный слот
            if (slot is not null)
            {
                // Логика добавления, при переполнении допустимого веса
                if (maxTotalWeight > 0 && CurrentWeight + item.Config.weight > maxTotalWeight)
                {
                    return false;
                }

                slot.Item = item;
                item.transform.parent = slot.slotTransform;
                item.transform.DOMove(slot.slotTransform.position, .4f);

                ItemAddedEvent.Invoke(item.Config.id);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Удаление предмета из инвентаря
        /// </summary>
        /// <returns>Возвращает true при успешном добавлении</returns>
        public bool RemoveItem(Item item)
        {
            if (item is null)
                throw new("Item is null");

            // Поиск предмета в слотах
            Slot slot = slots.Where(x => x.Item == item).FirstOrDefault();

            // Удаление предмета из слота и перенос на сцену
            if (slot is not null)
            {
                slot.Item = null;
                item.transform.parent = null;

                ItemRemovedEvent.Invoke(item.Config.id);

                return true;
            }

            return false;
        }


        /// <summary>
        /// Получение предмета, если курсор находится на предмете
        /// </summary>
        private Item GetItemUnderCursor()
        {
            // Создание луча из позиции курсора
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(ray, out hit); // Бросание луча (raycast)

            return hit.transform.GetComponent<Item>();
        }
    }
}
