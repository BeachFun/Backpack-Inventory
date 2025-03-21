using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BackpackInventory
{
    [RequireComponent(typeof(Drag3dObject))]
    public class Item : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private InventoryItemConfig config;

        private Inventory _inventory;

        private bool m_isInInventory;
        private bool m_isDragging;
        private Vector3? m_positionOnStartDrag;
        private Drag3dObject m_dragSystem;
        private Rigidbody m_rigidbody;


        public InventoryItemConfig Config => config;


        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_dragSystem = GetComponent<Drag3dObject>();

            // �������� �� ������� ����������� �������
            m_dragSystem.BeginDraggingEvent.AddListener(OnBeginDragging);
            m_dragSystem.EndDraggingEvent.AddListener(OnEndDragging);
        }

        private void OnDestroy()
        {
            // ������� �� ������� ����������� �������
            m_dragSystem.BeginDraggingEvent.RemoveListener(OnBeginDragging);
            m_dragSystem.EndDraggingEvent.RemoveListener(OnEndDragging);
        }


        private void OnBeginDragging()
        {
            m_isDragging = true;
            m_positionOnStartDrag = transform.position;

            // ���� ������� ��� ��������� � ���������, ������� ��� ������ ����� ���������������
            if (_inventory is not null)
            {
                _inventory.RemoveItem(this);
            }
        }

        private void OnEndDragging()
        {
            m_isDragging = false;

            // ���� ������� ��� ������� � ���� ���������, ��������� ��� � ���������
            if (_inventory is not null)
            {
                // �������� ������� � ���������, ���� �� �������, �� ������� ������� �� �������� �������
                m_isInInventory = _inventory.PutItem(this);
                if (m_isInInventory)
                {
                    m_rigidbody.isKinematic = true;
                }
                else
                {
                    transform.position = m_positionOnStartDrag.Value;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (m_isDragging && other.CompareTag("Inventory"))
            {
                _inventory = other.gameObject.GetComponent<Inventory>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (m_isDragging && other.CompareTag("Inventory"))
            {
                _inventory = null;
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            // ��������� ��� ���������� ������ OnPointerUp
        }

        /// <summary>
        /// ����������� �������� �� ��������� �� �����, ���� �� ��������� � ���
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (!m_isInInventory) return;

            // �������� �������� �� ���������
            if (_inventory.RemoveItem(this))
            {
                m_isInInventory = false;
            }

            // ���������� ������ ������ � �������� � �������������� �������� ��������� � �������, ���� ��������� ������� ���
            if (m_positionOnStartDrag is null)
            {
                Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, Camera.main.WorldToScreenPoint(transform.position).z);
                m_positionOnStartDrag = Camera.main.ScreenToWorldPoint(screenCenter);
            }

            _inventory = null;
            transform.DOMove(m_positionOnStartDrag.Value, .4f); // ����������� ������� �� �������� �������
        }
    }
}
