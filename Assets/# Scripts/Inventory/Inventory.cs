using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;

namespace BackpackInventory
{
    /// <summary>
    /// ������� ���������, ��������������� ��� �������� �������� � 3� � ���������
    /// </summary>
    public class Inventory : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Inventory Config")]
        [Tooltip("���� ����������� �������� 0, �� ������������ ��� �����������")]
        [SerializeField] private int maxTotalWeight = 0; // if 0 then unlimited
        [SerializeField] private Slot[] slots;

        [Header("Binding")]
        [Tooltip("������� � ������� ������ ��������� ���������� � ���������")]
        [SerializeField] private Collider triggerZone;
        [SerializeField] private InventoryGUI inventoryGUI;


        /// <summary>
        /// ����� ���������
        /// </summary>
        public Slot[] Slots => slots;
        /// <summary>
        /// ������� ��� ���������
        /// </summary>
        public int CurrentWeight
        {
            get => slots.Where(x => x.Item is not null).Sum(x => x.Item.Config.weight);
        }
        /// <summary>
        /// ������� ���������� �������� � ���������
        /// </summary>
        public UnityEvent<string> ItemAddedEvent { get; private set; } = new();
        /// <summary>
        /// ������� �������� �������� �� ���������
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
        /// ���������� �������� � ���������
        /// </summary>
        /// <returns>���������� true ��� �������� ����������</returns>
        public bool PutItem(Item item)
        {
            if (item is null)
                throw new("Item is null");

            ItemType itemType = item.Config.type;

            // ����� ������� ����� �� ����, ���� ����� ������ ���, �� ����� ������ ����������� ����� ��������
            Slot slot =
                slots.Where(x => x.ItemType == itemType && x.Item is null).FirstOrDefault() ??
                slots.Where(x => x.isAny).FirstOrDefault();

            // ���������� �������� � ��������� ����
            if (slot is not null)
            {
                // ������ ����������, ��� ������������ ����������� ����
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
        /// �������� �������� �� ���������
        /// </summary>
        /// <returns>���������� true ��� �������� ����������</returns>
        public bool RemoveItem(Item item)
        {
            if (item is null)
                throw new("Item is null");

            // ����� �������� � ������
            Slot slot = slots.Where(x => x.Item == item).FirstOrDefault();

            // �������� �������� �� ����� � ������� �� �����
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
        /// ��������� ��������, ���� ������ ��������� �� ��������
        /// </summary>
        private Item GetItemUnderCursor()
        {
            // �������� ���� �� ������� �������
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Physics.Raycast(ray, out hit); // �������� ���� (raycast)

            return hit.transform.GetComponent<Item>();
        }
    }
}
