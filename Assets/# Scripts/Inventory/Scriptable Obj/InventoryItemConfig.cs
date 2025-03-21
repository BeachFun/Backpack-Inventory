using UnityEngine;

namespace BackpackInventory
{
    /// <summary>
    /// Конфигурация предмета для инвентаря
    /// </summary>
    [CreateAssetMenu(fileName = "new ItemConfig", menuName = "Inventory/ItemConfig")]
    public class InventoryItemConfig : ScriptableObject
    {
        [Tooltip("Разное")]
        public ItemType type = ItemType.Miscellaneous;
        public string id;
        public string itemName;
        public int weight;
        public Sprite icon;
        public GameObject prefab;
    }
}
