using System;

namespace InventoryControl.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string InventoryNumber { get; set; }
        public string EquipmentType { get; set; }
        public decimal EquipmentCost { get; set; }
        public string Location { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}