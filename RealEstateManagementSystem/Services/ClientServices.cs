using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RealEstateManagementSystem.Models;
using System.Data;
using System;

namespace RealEstateManagementSystem.Services
{
    public class ClientService
    {
        private readonly string _connectionString;

        public ClientService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Client> GetAll()
        {
            var clients = new List<Client>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, FullName, PhoneNumber, Email, Address FROM Clients", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clients.Add(new Client
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Address = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }
                }
            }
            return clients;
        }

        public void Add(Client client)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO Clients (FullName, PhoneNumber, Email, Address) VALUES (@FullName, @PhoneNumber, @Email, @Address)", connection);
                command.Parameters.AddWithValue("@FullName", (object)client.FullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@PhoneNumber", (object)client.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)client.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@Address", (object)client.Address ?? DBNull.Value);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Client GetById(int id)
        {
            Client client = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, FullName, PhoneNumber, Email, Address FROM Clients WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        client = new Client
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            PhoneNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Address = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                    }
                }
            }
            return client;
        }
    }
}
