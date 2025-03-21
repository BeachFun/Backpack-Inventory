using UnityEngine;

namespace BackpackInventory
{
    /// <summary>
    /// Скрипт управления вертикальным поворотом камеры
    /// </summary>
    public class CameraControl : MonoBehaviour
    {
        [Tooltip("Скорость поворота камеры")]
        [SerializeField] private float rotationSpeed = 14.0f;
        [SerializeField] private float minAngle = -45f;
        [SerializeField] private float maxAngle = 45f;

        private float m_xRotation = 0f; // Текущий угол поворота по оси X

        private void Update()
        {
            float mouseInput = 0f;

            // Считывание значений ввода
            if (Input.GetKey(KeyCode.W))
                mouseInput = 1f;
            else if (Input.GetKey(KeyCode.S))
                mouseInput = -1f;

            // Вычисление угла поворота с учетом времени между кадрами
            m_xRotation -= mouseInput * rotationSpeed * Time.deltaTime;
            m_xRotation = Mathf.Clamp(m_xRotation, minAngle, maxAngle);

            // Изменение угла поворота
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(m_xRotation, currentRotation.y, currentRotation.z);
        }
    }
}
