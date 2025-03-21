using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BackpackInventory;

namespace RH.Instruments
{
    /// <summary>
    /// Делает скриншоты объектов на сцене видимые камерой, сохраняет их в определенной папке и связывает их с InventoryItemConfig.
    /// Скрипт перебирает игровые объекты и создает иконки каждого отдельно.
    /// </summary>
    public class IconGenerator : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private string _pathFolder;

        [Header("Binding")]
        [SerializeField] private Camera _camera;
        [SerializeField] private List<GameObject> _sceneObjects;
        [SerializeField] private List<InventoryItemConfig> _configObjects;


        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Запускает процесс создаения иконок для списка игровых объектов
        /// </summary>
        [ContextMenu("Screenshot")]
        private void ProcessScreenshots()
        {
            StartCoroutine(Screenshot());
        }

        /// <summary>
        /// Перебирает игровые объекты и создает иконку для каждого
        /// </summary>
        /// <returns></returns>
        private IEnumerator Screenshot()
        {
            for (int i = 0; i < _sceneObjects.Count; i++)
            {
                GameObject obj = _sceneObjects[i];
                InventoryItemConfig config = _configObjects[i];

                obj.SetActive(true);
                yield return null;
                TakeScreenshot($"{Application.dataPath}/{_pathFolder}/{config.id}_Icon.png");
                yield return null;
                obj.SetActive(false);

                Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{_pathFolder}/{config.id}_Icon.png");
                if (s is not null)
                {
                    config.icon = s;
                    EditorUtility.SetDirty(config);
                }

                yield return null;
            }
        }

        /// <summary>
        /// Создает скриншот экрана и сохраняет его по указаному пути
        /// </summary>
        private void TakeScreenshot(string fullPath)
        {
            if (_camera is null)
                throw new("Camera is missing");

            // Сохранение настроек камеры в буфере
            CameraClearFlags clearFlags = _camera.clearFlags;
            _camera.clearFlags = CameraClearFlags.Depth;

            // Алгоритм создания скриншота камеры
            var rt = new RenderTexture(256, 256, 24);
            _camera.targetTexture = rt;
            var screenshot = new Texture2D(256, 256, TextureFormat.ARGB32, false);
            _camera.Render();
            RenderTexture.active = rt;
            screenshot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
            _camera.targetTexture = null;
            RenderTexture.active = null;

            if (Application.isEditor)
            {
                DestroyImmediate(rt);
            }
            else
            {
                Destroy(rt);
            }

            byte[] bytes = screenshot.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            _camera.clearFlags = clearFlags; // восстановление настроек камеры
        }
#endif
    }
}
