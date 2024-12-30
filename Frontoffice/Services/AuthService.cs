using Frontoffice.Config.Database;
using Microsoft.Data.SqlClient;
using Shared.Models;
using BCrypt.Net;
using Frontoffice.Services.Interfaces;

namespace Frontoffice.Services
{
    public class AuthService: IAuthService
    {
        private readonly DatabaseConnection _db;

        public AuthService(DatabaseConnection db)
        {
            _db = db;
        }

        public User? ValidateUser(string email, string password)
        {
            var query = "SELECT Id, Email, Password, Name, RoleId FROM Users WHERE Email = @Email";
            var parameter = new SqlParameter("@Email", email);

            using (var reader = _db.ExecuteReader(query, parameter))
            {
                if (reader.Read())
                {
                    var hashedPassword = reader.GetString(reader.GetOrdinal("Password"));
                    
                    // Vérifier le mot de passe avec BCrypt
                    if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                    {
                        return new User
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Name = !reader.IsDBNull(reader.GetOrdinal("Name")) ? reader.GetString(reader.GetOrdinal("Name")) : null,
                            RoleId = !reader.IsDBNull(reader.GetOrdinal("RoleId")) ? reader.GetInt32(reader.GetOrdinal("RoleId")) : null,
                            Password = hashedPassword // Normalement, on ne devrait pas renvoyer le mot de passe, mais ici c'est requis par le modèle
                        };
                    }
                }
            }

            return null;
        }
    }
} 