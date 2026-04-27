using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RealEstateManagementSystem.Models;
using System.Data;
using System;

namespace RealEstateManagementSystem.Services
{
    public class AgentService
    {
        private readonly string _connectionString;

        public AgentService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public List<Agent> GetAll()
        {
            var agents = new List<Agent>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, FullName, LicenseNumber, Email, PhoneNumber FROM Agents", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        agents.Add(new Agent
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            LicenseNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }
                }
            }
            return agents;
        }

        public void Add(Agent agent)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO Agents (FullName, LicenseNumber, Email, PhoneNumber) VALUES (@FullName, @LicenseNumber, @Email, @PhoneNumber)", connection);
                command.Parameters.AddWithValue("@FullName", (object)agent.FullName ?? DBNull.Value);
                command.Parameters.AddWithValue("@LicenseNumber", (object)agent.LicenseNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object)agent.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@PhoneNumber", (object)agent.PhoneNumber ?? DBNull.Value);
                
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public Agent GetById(int id)
        {
            Agent agent = null;
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, FullName, LicenseNumber, Email, PhoneNumber FROM Agents WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        agent = new Agent
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.IsDBNull(1) ? null : reader.GetString(1),
                            LicenseNumber = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                            PhoneNumber = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                    }
                }
            }
            return agent;
        }
    }
}
