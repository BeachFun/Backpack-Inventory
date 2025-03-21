using UnityEngine;

namespace BackpackInventory
{
    /// <summary>
    /// ������ ���������� ������������ ��������� ������
    /// </summary>
    public class CameraControl : MonoBehaviour
    {
        [Tooltip("�������� �������� ������")]
        [SerializeField] private float rotationSpeed = 14.0f;
        [SerializeField] private float minAngle = -45f;
        [SerializeField] private float maxAngle = 45f;

        private float m_xRotation = 0f; // ������� ���� �������� �� ��� X

        private void Update()
        {
            float mouseInput = 0f;

            // ���������� �������� �����
            if (Input.GetKey(KeyCode.W))
                mouseInput = 1f;
            else if (Input.GetKey(KeyCode.S))
                mouseInput = -1f;

            // ���������� ���� �������� � ������ ������� ����� �������
            m_xRotation -= mouseInput * rotationSpeed * Time.deltaTime;
            m_xRotation = Mathf.Clamp(m_xRotation, minAngle, maxAngle);

            // ��������� ���� ��������
            Vector3 currentRotation = transform.localEulerAngles;
            transform.localEulerAngles = new Vector3(m_xRotation, currentRotation.y, currentRotation.z);
        }
    }
}
