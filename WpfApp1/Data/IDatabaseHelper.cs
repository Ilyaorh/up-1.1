using System.Collections.Generic;
using InventoryControl.Models;

namespace InventoryControl.Data
{
    public interface IDatabaseHelper
    {
        List<InventoryItem> GetAllItems();
        List<InventoryItem> SearchItems(string searchTerm, string searchField);
        bool AddItem(InventoryItem item);
        bool UpdateItem(InventoryItem item);
        bool DeleteItem(int id);
        List<InventoryItem> SortItems(string sortBy, bool ascending = true);
    }
}