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
        public DataSubmitted data_submitted;
    }

    /// <summary>
    /// Структура данных для отправки на сервер
    /// </summary>
    [System.Serializable]
    public class DataSubmitted
    {
        public string item_id;
        public string event_type;
    }
}
