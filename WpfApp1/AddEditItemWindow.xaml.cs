using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InventoryControl.Models;

namespace InventoryControl
{
    public partial class AddEditItemWindow : Window
    {
        public InventoryItem Item { get; private set; }
        private readonly bool _isEditMode;

        public AddEditItemWindow(InventoryItem item)
        {
            InitializeComponent();

            if (item != null)
            {
                _isEditMode = true;
                Item = new InventoryItem
                {
                    Id = item.Id,
                    FullName = item.FullName,
                    InventoryNumber = item.InventoryNumber,
                    EquipmentType = item.EquipmentType,
                    EquipmentCost = item.EquipmentCost,
                    Location = item.Location
                };

                Title = "Редактирование записи";
                FillFields();
            }
            else
            {
                _isEditMode = false;
                Item = new InventoryItem();
                Title = "Добавление записи";
            }
        }

        private void FillFields()
        {
            txtFullName.Text = Item.FullName;
            txtInventoryNumber.Text = Item.InventoryNumber;
            cmbEquipmentType.Text = Item.EquipmentType;
            txtCost.Text = Item.EquipmentCost.ToString();
            txtLocation.Text = Item.Location;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            Item.FullName = txtFullName.Text.Trim();
            Item.InventoryNumber = txtInventoryNumber.Text.Trim();
            Item.EquipmentType = cmbEquipmentType.Text.Trim();
            Item.EquipmentCost = decimal.Parse(txtCost.Text);
            Item.Location = txtLocation.Text.Trim();

            DialogResult = true;
            Close();
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtFullName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtInventoryNumber.Text))
            {
                MessageBox.Show("Введите инвентарный номер!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtInventoryNumber.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbEquipmentType.Text))
            {
                MessageBox.Show("Выберите или введите тип оборудования!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbEquipmentType.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtCost.Text))
            {
                MessageBox.Show("Введите стоимость!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCost.Focus();
                return false;
            }

            if (!decimal.TryParse(txtCost.Text, out decimal cost) || cost <= 0)
            {
                MessageBox.Show("Стоимость должна быть положительным числом!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtCost.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Введите место расположения!", "Ошибка валидации",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                txtLocation.Focus();
                return false;
            }

            return true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void txtCost_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}