using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BackpackInventory
{
    /// <summary>
    /// ��������� ����������� ����������� ������� � 3D ������������ � ������� ����
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Drag3dObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Rigidbody m_rigidbody;
        private Vector3 m_offset;
        private float m_objectHeight;
        private Collider m_collider;

        /// <summary>
        /// �������, ����������� � ������ ����������� �������
        /// </summary>
        public UnityEvent BeginDraggingEvent { get; private set; } = new();
        /// <summary>
        /// �������, ����������� ����� ����������� �������
        /// </summary>
        public UnityEvent EndDraggingEvent { get; private set; } = new();

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_collider = GetComponent<Collider>();
            m_objectHeight = m_collider.bounds.extents.y; // ���������� ������ ������� ��� ���������� ��������� �� �����������
        }

        /// <summary>
        /// ���������� �������� ������� � ������ �����������
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_rigidbody.isKinematic = true;
            m_offset = transform.position - GetMouseWorldPos();
            BeginDraggingEvent.Invoke();
        }

        /// <summary>
        /// ������ ������� ������� ��� ����������� �����
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 targetPosition = GetMouseWorldPos() + m_offset;

            // ����� ��������� ����������� ��� ��������
            RaycastHit? closestHit = GetClosestHit(
                targetPosition + Vector3.up * 5f,
                Vector3.down,
                10f,
                ~0
            );

            // ������������� ������� ������� �� ������
            if (closestHit.HasValue)
            {
                targetPosition.y = closestHit.Value.point.y + m_objectHeight;
            }

            transform.position = targetPosition;
        }

        /// <summary>
        /// ��������� �������� ������� ����� �����������
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            m_rigidbody.isKinematic = false;

            // �������� �������� ������� � ������ �����������
            RaycastHit? closestHit = GetClosestHit(
                transform.position + Vector3.up * 5f,
                Vector3.down,
                10f,
                ~0
            );

            if (closestHit.HasValue)
            {
                Vector3 finalPosition = transform.position;
                finalPosition.y = closestHit.Value.point.y + m_objectHeight;
                transform.position = finalPosition;
            }

            EndDraggingEvent.Invoke();
        }

        /// <summary>
        /// ��������� ������� ���� �� ����������� ��������� � ���������
        /// </summary>
        /// <returns>������� Vector3 � ���������� ������� ���������</returns>
        private Vector3 GetMouseWorldPos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit? closestHit = GetClosestHit(
                ray.origin,
                ray.direction,
                Mathf.Infinity,
                ~0
            );

            return closestHit?.point ?? transform.position;
        }

        /// <summary>
        /// ������� ��������� ����������� ���� � �������������, ��������� ����������� ���������
        /// </summary>
        /// <param name="origin">��������� ����� ����</param>
        /// <param name="direction">����������� ����</param>
        /// <param name="maxDistance">������������ ���������</param>
        /// <param name="layerMask">����� �����</param>
        /// <returns>��������� ����������� ��� null</returns>
        private RaycastHit? GetClosestHit(Vector3 origin, Vector3 direction, float maxDistance, LayerMask layerMask)
        {
            Ray ray = new Ray(origin, direction);
            RaycastHit[] hits = Physics.RaycastAll(ray, maxDistance, layerMask, QueryTriggerInteraction.Ignore);

            RaycastHit? closestHit = null;
            foreach (var hit in hits)
            {
                if (hit.collider == m_collider) continue;

                if (closestHit == null || hit.distance < closestHit.Value.distance)
                {
                    closestHit = hit;
                }
            }
            return closestHit;
        }
    }
}