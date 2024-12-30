using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace Frontoffice.Config.Database
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;
        private SqlConnection? _connection;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection GetConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
            }

            if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }

        // Méthode utilitaire pour exécuter une commande sans retour
        public int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteNonQuery();
            }
        }

        // Méthode utilitaire pour exécuter une commande avec retour d'un scalar
        public object? ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            using (var connection = GetConnection())
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar();
            }
        }

        // Méthode utilitaire pour exécuter une commande avec retour d'un reader
        public SqlDataReader ExecuteReader(string query, params SqlParameter[] parameters)
        {
            var connection = GetConnection();
            var command = new SqlCommand(query, connection);
            
            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }
            
            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
    }
}
