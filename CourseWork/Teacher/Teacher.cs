using CourseWork.Discipline;
using CourseWork.Speciality;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Xml.Linq;

namespace CourseWork.Teacher
{
    public class TeacherListItemData
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Position { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName} {Surname}";
        }
    }

    public class TeacherDisciplineListItemData
    {
        public int? GroupId { get; set; }
        public string? Group { get; set; }
        public int? DisciplineId { get; set; }
        public string? Discipline { get; set; }
    }

    public class TeacherService
    {
        private readonly string connectionString;

        public TeacherService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }

        public List<TeacherDisciplineListItemData> GetDisciplineListByTeacher(int teacherId, int? groupId = null, int? disciplineId = null)
        {
            List<TeacherDisciplineListItemData> list = new List<TeacherDisciplineListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                    SELECT d.id AS discipline_id, d.name AS discipline_name, gr.id AS group_id, gr.code AS group_code
                    FROM teacher t
                    JOIN discipline_assigment da ON da.teacher_id = t.id
                    JOIN group_discipline gd ON gd.id = da.group_discipline_id
                    JOIN [group] gr ON gr.id = gd.group_id
                    JOIN discipline d ON d.id = gd.discipline_id
                    WHERE t.id = @teacherId";

                    if (groupId.HasValue)
                    {
                        query += " AND gr.id = @groupId";
                    }

                    if (disciplineId.HasValue)
                    {
                        query += " AND d.id = @disciplineId";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@teacherId", teacherId);

                        if (groupId.HasValue)
                        {
                            command.Parameters.AddWithValue("@groupId", groupId);
                        }

                        if (disciplineId.HasValue)
                        {
                            command.Parameters.AddWithValue("@disciplineId", disciplineId);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new TeacherDisciplineListItemData()
                                {
                                    GroupId = (int)reader["group_id"],
                                    Group = reader["group_code"].ToString(),
                                    DisciplineId = (int)reader["discipline_id"],
                                    Discipline = reader["discipline_name"].ToString()
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

        public List<TeacherListItemData> GetItemList(string searchTerm = "")
        {
            List<TeacherListItemData> list = new List<TeacherListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = string.IsNullOrEmpty(searchTerm)
                        ? "SELECT * FROM teacher"
                        : @"SELECT * FROM teacher WHERE
                            first_name LIKE @searchTerm
                            OR last_name LIKE @searchTerm
                            OR phone LIKE @searchTerm";

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
                                list.Add(new TeacherListItemData()
                                {
                                    Id = (int)reader["id"],
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    Surname = reader["surname"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Position = reader["position"].ToString()
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

        public void CreateItem(string firstName, string lastName, string surname, string phone, string email, string position)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"INSERT INTO teacher (first_name, last_name, surname, phone, email, position) 
                                    VALUES (@firstName, @lastName, @surname,  @phone, @email, @position)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@surname", surname);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@position", position);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void UpdateItem(int id, string firstName, string lastName, string surname, string phone, string email, string position)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"UPDATE teacher
                        SET first_name = @firstName, last_name = @lastName, surname = @surname,  phone = @phone, email = @email, position = @position
                        WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@surname", surname);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@position", position);
                        command.Parameters.AddWithValue("@id", id);
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
                    string query = "DELETE FROM teacher WHERE id = @id";
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
