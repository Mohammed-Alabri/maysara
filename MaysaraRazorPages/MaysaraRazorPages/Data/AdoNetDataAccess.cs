using System.Data;
using Microsoft.Data.SqlClient;

namespace MaysaraRazorPages.Data
{
    /// <summary>
    /// ADO.NET Data Access Helper Class
    /// Provides methods for direct database access using parameterized queries
    /// </summary>
    public class AdoNetDataAccess
    {
        private readonly string _connectionString;
        private readonly ILogger<AdoNetDataAccess> _logger;

        public AdoNetDataAccess(IConfiguration configuration, ILogger<AdoNetDataAccess> logger)
        {
            _connectionString = configuration.GetConnectionString("MaysaraConnection")
                ?? throw new InvalidOperationException("Connection string 'MaysaraConnection' not found.");
            _logger = logger;
        }

        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Error executing query: {Query}", query);
                throw new Exception($"Database error occurred: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing query: {Query}", query);
                throw;
            }

            return dataTable;
        }

        /// <summary>
        /// Executes INSERT, UPDATE, DELETE commands
        /// Returns the number of rows affected
        /// </summary>
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            int rowsAffected = 0;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        connection.Open();
                        rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Error executing non-query: {Query}", query);
                throw new Exception($"Database error occurred: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing non-query: {Query}", query);
                throw;
            }

            return rowsAffected;
        }

        /// <summary>
        /// Executes a scalar query (COUNT, SUM, AVG, etc.)
        /// Returns a single value
        /// </summary>
        public object? ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            object? result = null;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add parameters to prevent SQL injection
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        connection.Open();
                        result = command.ExecuteScalar();
                    }
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Error executing scalar: {Query}", query);
                throw new Exception($"Database error occurred: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scalar: {Query}", query);
                throw;
            }

            return result;
        }
    }
}
