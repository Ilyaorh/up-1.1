using FluentAssertions;
using Npgsql;
using Xunit;
using InventoryControl.Data;
using InventoryControl.Models;

namespace InventoryControl.Tests.Integration
{
    [Collection("Database")]
    public class DatabaseIntegrationTests
    {
        private readonly DatabaseHelper _dbHelper;
        private readonly string _testConnectionString;

        public DatabaseIntegrationTests()
        {
            _testConnectionString = "Host=localhost;Database=InventoryControl;Username=postgres;Password=sa";
            _dbHelper = new DatabaseHelper(_testConnectionString);
            SetupTestDatabase();
        }

        private void SetupTestDatabase()
        {
            using var connection = new NpgsqlConnection(_testConnectionString);
            connection.Open();

            using var cmd = new NpgsqlCommand("DELETE FROM Inventory", connection);
            cmd.ExecuteNonQuery();
        }

        [Fact]
        public void AddItem_IntegrationTest_ShouldAddItemToDatabase()
        {
            // Arrange
            var item = new InventoryItem
            {
                FullName = "Integration Test User",
                InventoryNumber = "INT-001",
                EquipmentType = "Компьютер",
                EquipmentCost = 75000,
                Location = "Test Lab"
            };

            // Act
            var addResult = _dbHelper.AddItem(item);
            var allItems = _dbHelper.GetAllItems();

            // Assert
            addResult.Should().BeTrue();
            allItems.Should().ContainSingle(i => i.InventoryNumber == "INT-001");
        }

        [Fact]
        public void UpdateItem_IntegrationTest_ShouldUpdateItemInDatabase()
        {
            // Arrange
            var item = new InventoryItem
            {
                FullName = "Update Test",
                InventoryNumber = "UPD-INT-001",
                EquipmentType = "Монитор",
                EquipmentCost = 20000,
                Location = "Room 1"
            };
            _dbHelper.AddItem(item);
            var items = _dbHelper.GetAllItems();
            var addedItem = items.First(i => i.InventoryNumber == "UPD-INT-001");

            // Act
            addedItem.EquipmentCost = 25000;
            addedItem.Location = "Room 2";
            var updateResult = _dbHelper.UpdateItem(addedItem);
            var updatedItems = _dbHelper.GetAllItems();

            // Assert
            updateResult.Should().BeTrue();
            var updatedItem = updatedItems.First(i => i.Id == addedItem.Id);
            updatedItem.EquipmentCost.Should().Be(25000);
            updatedItem.Location.Should().Be("Room 2");
        }

        [Fact]
        public void DeleteItem_IntegrationTest_ShouldRemoveItemFromDatabase()
        {
            // Arrange
            var item = new InventoryItem
            {
                FullName = "Delete Test",
                InventoryNumber = "DEL-INT-001",
                EquipmentType = "Клавиатура",
                EquipmentCost = 3000,
                Location = "Storage"
            };
            _dbHelper.AddItem(item);
            var items = _dbHelper.GetAllItems();
            var itemToDelete = items.First(i => i.InventoryNumber == "DEL-INT-001");

            // Act
            var deleteResult = _dbHelper.DeleteItem(itemToDelete.Id);
            var remainingItems = _dbHelper.GetAllItems();

            // Assert
            deleteResult.Should().BeTrue();
            remainingItems.Should().NotContain(i => i.Id == itemToDelete.Id);
        }

        [Fact]
        public void SearchItems_IntegrationTest_ShouldFindMatchingItems()
        {
            // Arrange
            var item1 = new InventoryItem
            {
                FullName = "John Doe",
                InventoryNumber = "SRCH-001",
                EquipmentType = "Ноутбук",
                EquipmentCost = 80000,
                Location = "Office A"
            };
            var item2 = new InventoryItem
            {
                FullName = "Jane Smith",
                InventoryNumber = "SRCH-002",
                EquipmentType = "Компьютер",
                EquipmentCost = 60000,
                Location = "Office B"
            };
            _dbHelper.AddItem(item1);
            _dbHelper.AddItem(item2);

            // Act
            var results = _dbHelper.SearchItems("John", "FullName");

            // Assert
            results.Should().ContainSingle();
            results.First().FullName.Should().Contain("John");
        }
    }

    [CollectionDefinition("Database")]
    public class DatabaseCollection : ICollectionFixture<DatabaseIntegrationTests>
    {
    }
}