using FluentAssertions;
using WpfApp1.Models;
using Xunit;
using InventoryControl.Models;

namespace WpfApp1.Models
{
    public class InventoryItemTests
    {
        [Fact]
        public void InventoryItem_Properties_ShouldBeSetAndGetCorrectly()
        {
            // Arrange
            var item = new InventoryItem();
            var testDate = DateTime.Now;

            // Act
            item.Id = 1;
            item.FullName = "Иванов Иван Иванович";
            item.InventoryNumber = "INV-001";
            item.EquipmentType = "Компьютер";
            item.EquipmentCost = 50000.50m;
            item.Location = "Кабинет 101";
            item.CreatedDate = testDate;

            // Assert
            item.Id.Should().Be(1);
            item.FullName.Should().Be("Иванов Иван Иванович");
            item.InventoryNumber.Should().Be("INV-001");
            item.EquipmentType.Should().Be("Компьютер");
            item.EquipmentCost.Should().Be(50000.50m);
            item.Location.Should().Be("Кабинет 101");
            item.CreatedDate.Should().Be(testDate);
        }

        [Fact]
        public void InventoryItem_Initialization_ShouldSetDefaultValues()
        {
            // Arrange & Act
            var item = new InventoryItem();

            // Assert
            item.Id.Should().Be(0);
            item.FullName.Should().BeNull();
            item.InventoryNumber.Should().BeNull();
            item.EquipmentType.Should().BeNull();
            item.EquipmentCost.Should().Be(0);
            item.Location.Should().BeNull();
            item.CreatedDate.Should().Be(default);
        }
    }
}