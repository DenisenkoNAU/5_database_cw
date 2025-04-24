using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace CourseWork.Speciality
{
    public class SpecialityListItemData
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public override string ToString()
        {
            return $"{Code} {Name}";
        }
    }

    public class SpecialityService
    {
        private readonly string connectionString;

        public SpecialityService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }

        public List<SpecialityListItemData> GetItemList(string searchTerm = "")
        {
            List<SpecialityListItemData> list = new List<SpecialityListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = string.IsNullOrEmpty(searchTerm)
                        ? "SELECT * FROM speciality"
                        : "SELECT * FROM speciality WHERE name LIKE @searchTerm OR code LIKE @searchTerm";

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
                                list.Add(new SpecialityListItemData()
                                {
                                    Id = (int)reader["id"],
                                    Code = reader["code"].ToString(),
                                    Name = reader["name"].ToString()
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при отриманні списку: " + ex.Message);
                }
            }

            return list;
        }

        public void CreateItem(string name, string code)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO speciality (name, code) VALUES (@name, @code)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@code", code);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void UpdateItem(int id, string name, string code)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE speciality SET name = @name, code = @code WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", name);
                        command.Parameters.AddWithValue("@code", code);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при оновленні: " + ex.Message);
                }
            }
        }

        public void DeleteItem(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM speciality WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при видаленні: " + ex.Message);
                }
            }
        }
    }
}
