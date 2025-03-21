using System;
using UnityEngine;
using MoreMountains.Tools;

namespace BackpackInventory
{
    /// <summary>
    /// Структура данных для конфигурации слота в инвентаре
    /// </summary>
    [Serializable]
    public class Slot
    {
        [Tooltip("Можно ли поместить в ячейку любой объект")]
        public bool isAny = true;
        [MMCondition("isAny", true, true)]
        public ItemType ItemType;
        public Transform slotTransform;
        [SerializeField, Tooltip("Если значение не null, значит слот заполнен")]
        private InventoryItemConfig inventoryItemConfig; // для будущей сериализации/десериализации при сохранении и загрузке

        private Item m_item;

        /// <summary>
        /// Предмет, содержащийся в слоте
        /// </summary>
        public Item Item
        {
            get => m_item;
            set
            {
                m_item = value;
                inventoryItemConfig = value?.Config;
            }
        }
    }
}
