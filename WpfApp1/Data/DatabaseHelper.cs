using System;
using System.Collections.Generic;
using System.Data;
using Npgsql;
using InventoryControl.Models;

namespace InventoryControl.Data
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<InventoryItem> GetAllItems()
        {
            var items = new List<InventoryItem>();
            using (var connection = new NpgsqlConnection(_connectionString))
            using (var command = new NpgsqlCommand("SELECT * FROM Inventory ORDER BY Id", connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new InventoryItem
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            FullName = reader["FullName"].ToString(),
                            InventoryNumber = reader["InventoryNumber"].ToString(),
                            EquipmentType = reader["EquipmentType"].ToString(),
                            EquipmentCost = Convert.ToDecimal(reader["EquipmentCost"]),
                            Location = reader["Location"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                        });
                    }
                }
            }
            return items;
        }
        public bool AddItem(InventoryItem item)
        {
            try
            {
       
                if (item == null)
                    return false;

                if (item.EquipmentCost <= 0)
                    return false;

                if (string.IsNullOrWhiteSpace(item.FullName) ||
                    string.IsNullOrWhiteSpace(item.InventoryNumber) ||
                    string.IsNullOrWhiteSpace(item.EquipmentType) ||
                    string.IsNullOrWhiteSpace(item.Location))
                    return false;

                using (var connection = new NpgsqlConnection(_connectionString))
                using (var command = new NpgsqlCommand(@"
            INSERT INTO Inventory (FullName, InventoryNumber, EquipmentType, EquipmentCost, Location)
            VALUES (@FullName, @InventoryNumber, @EquipmentType, @EquipmentCost, @Location)", connection))
                {
                    command.Parameters.AddWithValue("@FullName", item.FullName);
                    command.Parameters.AddWithValue("@InventoryNumber", item.InventoryNumber);
                    command.Parameters.AddWithValue("@EquipmentType", item.EquipmentType);
                    command.Parameters.AddWithValue("@EquipmentCost", item.EquipmentCost);
                    command.Parameters.AddWithValue("@Location", item.Location);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateItem(InventoryItem item)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                using (var command = new NpgsqlCommand(@"
            UPDATE Inventory
            SET FullName = @FullName,
                InventoryNumber = @InventoryNumber,
                EquipmentType = @EquipmentType,
                EquipmentCost = @EquipmentCost,
                Location = @Location
            WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", item.Id);
                    command.Parameters.AddWithValue("@FullName", item.FullName);
                    command.Parameters.AddWithValue("@InventoryNumber", item.InventoryNumber);
                    command.Parameters.AddWithValue("@EquipmentType", item.EquipmentType);
                    command.Parameters.AddWithValue("@EquipmentCost", item.EquipmentCost);
                    command.Parameters.AddWithValue("@Location", item.Location);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0; 
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteItem(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(_connectionString))
                using (var command = new NpgsqlCommand("DELETE FROM Inventory WHERE Id = @Id", connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;  
                }
            }
            catch
            {
                return false;
            }
        }

        public List<InventoryItem> SearchItems(string searchTerm, string searchField)
        {
            var items = new List<InventoryItem>();

        
            if (string.IsNullOrWhiteSpace(searchTerm))
                return items; 

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                string query = $@"
            SELECT * FROM Inventory
            WHERE {searchField} ILIKE @searchTerm
            ORDER BY Id";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new InventoryItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FullName = reader["FullName"].ToString(),
                                InventoryNumber = reader["InventoryNumber"].ToString(),
                                EquipmentType = reader["EquipmentType"].ToString(),
                                EquipmentCost = Convert.ToDecimal(reader["EquipmentCost"]),
                                Location = reader["Location"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            return items;
        }

        public List<InventoryItem> SortItems(string sortBy, bool ascending = true)
        {
            var items = new List<InventoryItem>();
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                string order = ascending ? "ASC" : "DESC";
                string query = $@"
                    SELECT * FROM Inventory
                    ORDER BY {sortBy} {order}";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new InventoryItem
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                FullName = reader["FullName"].ToString(),
                                InventoryNumber = reader["InventoryNumber"].ToString(),
                                EquipmentType = reader["EquipmentType"].ToString(),
                                EquipmentCost = Convert.ToDecimal(reader["EquipmentCost"]),
                                Location = reader["Location"].ToString(),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"])
                            });
                        }
                    }
                }
            }
            return items;
        }
    }
}