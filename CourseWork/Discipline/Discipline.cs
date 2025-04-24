using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace CourseWork.Discipline
{
    public class DisciplineListItemData
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public override string ToString()
        {
            return $"{Name}";
        }
    }

    public class DisciplineService
    {
        private readonly string connectionString;

        public DisciplineService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }

        public List<DisciplineListItemData> GetDisciplines(string searchTerm = "")
        {
            List<DisciplineListItemData> disciplines = new List<DisciplineListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = string.IsNullOrEmpty(searchTerm)
                        ? "SELECT * FROM discipline"
                        : "SELECT * FROM discipline WHERE name LIKE @searchTerm";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                disciplines.Add(new DisciplineListItemData()
                                {
                                    Id = (int)reader["id"],
                                    Name = reader["name"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при отриманні списку дисциплін: " + ex.Message);
                }
            }

            return disciplines;
        }

        public void CreateDiscipline(string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO discipline (name) VALUES (@name)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні дисципліни: " + ex.Message);
                }
            }
        }

        public void UpdateDiscipline(int id, string name)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE discipline SET name = @name WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", name);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при оновленні дисципліни: " + ex.Message);
                }
            }
        }

        public void DeleteDiscipline(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM discipline WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при видаленні дисципліни: " + ex.Message);
                }
            }
        }
    }
}
