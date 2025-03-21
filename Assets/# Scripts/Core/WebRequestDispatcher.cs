using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace BackpackInventory
{
    /// <summary>
    /// Модуль отправки POST-запросов на сервер при добавлении и удалении предметов из инвентаря
    /// </summary>
    public class WebRequestDispatcher : IDisposable
    {
        private string _serverUrl = "https://wadahub.manerai.com/api/inventory/status";
        private string _authToken;

        private Inventory _inventory;


        [Inject]
        public WebRequestDispatcher(Inventory inventory)
        {
            _inventory = inventory;

            _authToken = File.ReadAllText("D:\\Token.txt");

            _inventory?.ItemAddedEvent.AddListener(OnItemAdded);
            _inventory?.ItemRemovedEvent.AddListener(OnItemRemoved);

            Debug.Log("WebRequestDispatcher is started");
        }

        public void Dispose()
        {
            _inventory?.ItemAddedEvent.RemoveListener(OnItemAdded);
            _inventory?.ItemRemovedEvent.RemoveListener(OnItemRemoved);
        }


        private void OnItemAdded(string itemId) => SendPOSTRequest(itemId, InventoryEventType.ItemAdded).Forget();
        private void OnItemRemoved(string itemId) => SendPOSTRequest(itemId, InventoryEventType.ItemRemoved).Forget();


        /// <summary>
        /// Отправка сведений в POST-запросе на сервер
        /// </summary>
        public async UniTaskVoid SendPOSTRequest(string itemId, InventoryEventType eventType)
        {
            // Сериализация данных в строку
            string jsonData = JsonUtility.ToJson(new ApiResponse()
            {
                response = "success",
                status = "data received",
                data_submitted = $"{itemId} {eventType.ToString()}"
            });

            // Создание нового запроса
            using (UnityWebRequest webRequest = new UnityWebRequest(_serverUrl, "POST"))
            {
                // Преобразование строки в массив байтов и настройка запроса
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.SetRequestHeader("Authorization", _authToken);

                // Отправка запроса и ожидание ответа
                await webRequest.SendWebRequest().ToUniTask();

                // Обработка ответа от сервера
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
                else
                {
                    Debug.Log("Response: " + webRequest.downloadHandler.text);
                }
            }
        }
    }
}
