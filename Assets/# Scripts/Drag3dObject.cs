using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BackpackInventory
{
    /// <summary>
    /// ��������� ����������� ���������� ������� � 3� ������������ � ������� ����
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Drag3dObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Rigidbody m_rigidbody;
        private Vector3 m_offset;


        /// <summary>
        /// �������� ����������� � ������� ����������� �������
        /// </summary>
        public UnityEvent BeginDraggingEvent { get; private set; } = new();
        /// <summary>
        /// �������� ����������� � ����� ����������� �������
        /// </summary>
        public UnityEvent EndDraggingEvent { get; private set; } = new();


        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }


        /// <summary>
        /// ���������� �������� �������
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_rigidbody.isKinematic = true;

            // ���������� �������� ����� �������� ������� � �������� �������
            m_offset = transform.position - GetMouseWorldPos();

            BeginDraggingEvent.Invoke();
        }

        /// <summary>
        /// ������ ������� �������� ��� ����������� �����
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = GetMouseWorldPos() + m_offset;
        }

        /// <summary>
        /// ��������� �������� �������
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            m_rigidbody.isKinematic = false;

            EndDraggingEvent.Invoke();
        }

        /// <summary>
        /// ��������� ������� ���� �� ����������� ��������� � ���������
        /// </summary>
        /// <returns>������� Vector3 � ���������� ������� ���������</returns>
        private Vector3 GetMouseWorldPos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���������, ���������� �� ��� ������� �����
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point;
            }

            // ���� ��� �� � ��� �� ���������, ���������� ������� �������
            return transform.position;
        }
    }
}
