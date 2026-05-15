using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using InventoryControl.Data;
using InventoryControl.Models;

namespace InventoryControl
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper _dbHelper;
        private List<InventoryItem> _allItems;
        private readonly string _connectionString = "Host=localhost;Database=InventoryControl;Username=postgres;Password=sa";



        public MainWindow()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper(_connectionString);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _allItems = _dbHelper.GetAllItems();
                dgInventory.ItemsSource = _allItems;
                txtRecordCount.Text = $"Записей: {_allItems.Count}";
                txtStatus.Text = "Данные загружены успешно";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                LoadData();
            }
        }

        private void PerformSearch()
        {
            string searchTerm = txtSearch.Text.Trim();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                LoadData();
                return;
            }

            string searchField = GetSearchFieldName();

            try
            {
                var results = _dbHelper.SearchItems(searchTerm, searchField);
                dgInventory.ItemsSource = results;
                txtRecordCount.Text = $"Найдено: {results.Count}";
                txtStatus.Text = $"Поиск по полю '{GetSearchFieldDisplayName()}'";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetSearchFieldName()
        {
            var selectedItem = (ComboBoxItem)cmbSearchField.SelectedItem;
            switch (selectedItem.Content.ToString())
            {
                case "ФИО": return "FullName";
                case "Инвентарный номер": return "InventoryNumber";
                case "Тип оборудования": return "EquipmentType";
                case "Место расположения": return "Location";
                default: return "FullName";
            }
        }

        private string GetSearchFieldDisplayName()
        {
            return ((ComboBoxItem)cmbSearchField.SelectedItem).Content.ToString();
        }

        private void btnResetSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            cmbSearchField.SelectedIndex = 0;
            LoadData();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddEditItemWindow(null);
            if (dialog.ShowDialog() == true)
            {
                if (_dbHelper.AddItem(dialog.Item))
                {
                    MessageBox.Show("Запись успешно добавлена!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении записи!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (dgInventory.SelectedItem is InventoryItem selectedItem)
            {
                var dialog = new AddEditItemWindow(selectedItem);
                if (dialog.ShowDialog() == true)
                {
                    if (_dbHelper.UpdateItem(dialog.Item))
                    {
                        MessageBox.Show("Запись успешно изменена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при изменении записи!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgInventory.SelectedItem is InventoryItem selectedItem)
            {
                var result = MessageBox.Show(
                    $"Вы действительно хотите удалить запись?\n\nФИО: {selectedItem.FullName}\nИнв. номер: {selectedItem.InventoryNumber}",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (_dbHelper.DeleteItem(selectedItem.Id))
                    {
                        MessageBox.Show("Запись успешно удалена!", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении записи!", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления!", "Внимание",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}