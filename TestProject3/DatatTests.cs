using FluentAssertions;
using Moq;
using Npgsql;
using System.Data;
using Xunit;
using InventoryControl.Data;
using InventoryControl.Models;
using System.Collections.Generic;

namespace InventoryControl.Tests.Data
{
    public class DatabaseHelperTests
    {
        private readonly string _testConnectionString = "Host=localhost;Database=InventoryControl;Username=postgres;Password=sa";

        #region GetAllItems Tests

        [Fact]
        public void GetAllItems_WhenDatabaseReturnsItems_ShouldReturnListOfItems()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);
          

            // Act
            var items = dbHelper.GetAllItems();

            // Assert
            items.Should().NotBeNull();
        }


        #endregion

        #region AddItem Tests

        [Fact]
        public void AddItem_WithValidItem_ShouldReturnTrue()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);
            var item = new InventoryItem
            {
                FullName = "Тестовый Пользователь",
                InventoryNumber = "TEST-001",
                EquipmentType = "Компьютер",
                EquipmentCost = 10000,
                Location = "Тестовая локация"
            };

            // Act
            var result = dbHelper.AddItem(item);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void AddItem_WithNullItem_ShouldReturnFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);

            // Act
            var result = dbHelper.AddItem(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AddItem_WithInvalidCost_ShouldReturnFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);
            var item = new InventoryItem
            {
                FullName = "Тест",
                InventoryNumber = "TEST-002",
                EquipmentType = "Ноутбук",
                EquipmentCost = -100, 
                Location = "Локация"
            };

            // Act
            var result = dbHelper.AddItem(item);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region UpdateItem Tests


        [Fact]
        public void UpdateItem_WithNonExistentId_ShouldReturnFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);
            var item = new InventoryItem
            {
                Id = 99999, 
                FullName = "Не существует",
                InventoryNumber = "NONE-001",
                EquipmentType = "Клавиатура",
                EquipmentCost = 5000,
                Location = "Где-то"
            };

            // Act
            var result = dbHelper.UpdateItem(item);

            // Assert
            result.Should().BeFalse();
        }

        #endregion


        [Fact]
        public void DeleteItem_WithNonExistentId_ShouldReturnFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);
            int nonExistentId = 99999;

            // Act
            var result = dbHelper.DeleteItem(nonExistentId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void DeleteItem_WithInvalidId_ShouldReturnFalse()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);
            int invalidId = -1;

            // Act
            var result = dbHelper.DeleteItem(invalidId);

            // Assert
            result.Should().BeFalse();
        }



        #region SearchItems Tests

        [Theory]
        [InlineData("Иванов", "FullName")]
        [InlineData("INV-001", "InventoryNumber")]
        [InlineData("Компьютер", "EquipmentType")]
        public void SearchItems_WithValidSearchTerm_ShouldReturnMatchingItems(
            string searchTerm, string searchField)
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);

            // Act
            var results = dbHelper.SearchItems(searchTerm, searchField);

            // Assert
            results.Should().NotBeNull();
        }

        [Fact]
        public void SearchItems_WithEmptySearchTerm_ShouldReturnEmptyList()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);

            // Act
            var results = dbHelper.SearchItems("", "FullName");

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void SearchItems_WithInvalidField_ShouldThrowException()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                dbHelper.SearchItems("test", "InvalidFieldName"));
        }

        #endregion

        #region SortItems Tests

        [Theory]
        [InlineData("FullName", true)]
        [InlineData("FullName", false)]
        [InlineData("EquipmentCost", true)]
        [InlineData("CreatedDate", false)]
        public void SortItems_WithValidParameters_ShouldReturnSortedItems(
            string sortBy, bool ascending)
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);

            // Act
            var items = dbHelper.SortItems(sortBy, ascending);

            // Assert
            items.Should().NotBeNull();
        }

        [Fact]
        public void SortItems_WithInvalidSortField_ShouldThrowException()
        {
            // Arrange
            var dbHelper = new DatabaseHelper(_testConnectionString);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                dbHelper.SortItems("InvalidField", true));
        }

        #endregion
    }
}