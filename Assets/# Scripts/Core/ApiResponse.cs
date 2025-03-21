namespace BackpackInventory
{
    /// <summary>
    /// Структура POST-запроса на сервер
    /// </summary>
    [System.Serializable]
    public class ApiResponse
    {
        public string response;
        public string status;
        public string data_submitted;
    }
}
