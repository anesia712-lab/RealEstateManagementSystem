using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RealEstateManagementSystem.Models;
using System.Data;
using System;

namespace RealEstateManagementSystem.Services
{
    public class PropertyService
    {
        private readonly string _connectionString;

        public PropertyService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Property> GetAll()
        {
            var properties = new List<Property>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, Address, Price, Bedrooms, Bathrooms FROM Properties", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        properties.Add(new Property
                        {
                            Id = reader.GetInt32(0),
                            Address = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Bedrooms = reader.GetInt32(3),
                            Bathrooms = reader.GetInt32(4)
                        });
                    }
                }
            }
            return properties;
        }

        public void Add(Property property)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO Properties (Address, Price, Bedrooms, Bathrooms) VALUES (@Address, @Price, @Bedrooms, @Bathrooms)", connection);
                command.Parameters.AddWithValue("@Address", (object)property.Address ?? DBNull.Value);
                command.Parameters.AddWithValue("@Price", property.Price);
                command.Parameters.AddWithValue("@Bedrooms", property.Bedrooms);
                command.Parameters.AddWithValue("@Bathrooms", property.Bathrooms);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Property GetById(int id)
        {
            Property property = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, Address, Price, Bedrooms, Bathrooms FROM Properties WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        property = new Property
                        {
                            Id = reader.GetInt32(0),
                            Address = reader.IsDBNull(1) ? null : reader.GetString(1),
                            Price = reader.GetDecimal(2),
                            Bedrooms = reader.GetInt32(3),
                            Bathrooms = reader.GetInt32(4)
                        };
                    }
                }
            }
            return property;
        }
    }
}
