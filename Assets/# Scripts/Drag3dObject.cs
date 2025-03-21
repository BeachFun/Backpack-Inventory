using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BackpackInventory
{
    /// <summary>
    /// Добавляет возможность пермещения объекта в 3д пространстве с помощью мыши
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Drag3dObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Rigidbody m_rigidbody;
        private Vector3 m_offset;


        /// <summary>
        /// Собыитые возникающее в началае перемещения объекта
        /// </summary>
        public UnityEvent BeginDraggingEvent { get; private set; } = new();
        /// <summary>
        /// Собыитые возникающее в после перемещения объекта
        /// </summary>
        public UnityEvent EndDraggingEvent { get; private set; } = new();


        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
        }


        /// <summary>
        /// Отключение динамики объекта
        /// </summary>
        /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            m_rigidbody.isKinematic = true;

            // Вычисление смещения между позицией объекта и позицией курсора
            m_offset = transform.position - GetMouseWorldPos();

            BeginDraggingEvent.Invoke();
        }

        /// <summary>
        /// Расчет позиции предмета при перемещении мышью
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            transform.position = GetMouseWorldPos() + m_offset;
        }

        /// <summary>
        /// Включение динамики объекта
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            m_rigidbody.isKinematic = false;

            EndDraggingEvent.Invoke();
        }

        /// <summary>
        /// Получение позиции мыши на поверхности предметов и окружения
        /// </summary>
        /// <returns>Позиция Vector3 в глабальной системе координат</returns>
        private Vector3 GetMouseWorldPos()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Проверяем, пересекает ли луч объекты сцены
            if (Physics.Raycast(ray, out hit))
            {
                return hit.point;
            }

            // Если луч ни с чем не пересекся, возвращаем позицию объекта
            return transform.position;
        }
    }
}
