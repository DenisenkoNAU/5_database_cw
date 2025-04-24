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
using System.Xml.Linq;

namespace CourseWork.Group
{
    public class GroupListItemData
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public int? SpecialityId { get; set; }
        public string? Speciality { get; set; }
        public override string ToString()
        {
            return $"{Code}";
        }
    }

    public class GroupStudentListItemData
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? StudentCard { get; set; }
    }

    public class GroupDisciplineListItemData
    {
        public int Id { get; set; }
        public int DisciplineId { get; set; }
        public string? Discipline { get; set; }
        public int TeacherId { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherLastName { get; set; }
        public string? TeacherSurname { get; set; }
        public string? Teacher {
            get
            {
                return $"{this.TeacherFirstName} {this.TeacherLastName} {this.TeacherSurname}";
            }
        }
    }

    public class GroupService
    {
        private readonly string connectionString;

        public GroupService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }

        public List<GroupDisciplineListItemData> GetGroupDiscipline(int groupId, string? searchTerm = "", int? teacherId = null)
        {
            List<GroupDisciplineListItemData> list = new List<GroupDisciplineListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                                    SELECT gd.id AS gd_id, 
                                        d.id AS discipline_id, d.name AS discipline_name,
                                        t.id AS t_id, t.first_name AS t_first_name, t.last_name AS t_last_name, t.surname AS t_surname 
                                    FROM group_discipline gd
                                    JOIN discipline_assigment da ON da.group_discipline_id = gd.id
                                    JOIN discipline d ON d.id = gd.discipline_id
                                    JOIN teacher t ON t.id = da.teacher_id
                                    WHERE gd.group_id = @groupId";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += " AND d.name LIKE @searchTerm";
                    }

                    if (teacherId != null)
                    {
                        query += " AND t.id = @teacherId";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@groupId", groupId);

                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        if (teacherId != null)
                        {
                            command.Parameters.AddWithValue("@teacherId", teacherId);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new GroupDisciplineListItemData()
                                {
                                    Id = (int)reader["gd_id"],
                                    DisciplineId = (int)reader["discipline_id"],
                                    Discipline = reader["discipline_name"].ToString(),
                                    TeacherId = (int)reader["t_id"],
                                    TeacherFirstName = reader["t_first_name"].ToString(),
                                    TeacherLastName = reader["t_last_name"].ToString(),
                                    TeacherSurname = reader["t_surname"].ToString()
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

        public void CreateGroupDiscipline(int groupId, int disciplineId, int teacherId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = @"
                        INSERT INTO group_discipline (discipline_id, group_id)
                        VALUES (@disciplineId, @groupId)
                        SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@disciplineId", disciplineId);
                        command.Parameters.AddWithValue("@groupId", groupId);
                       

                        int groupDisciplineId = Convert.ToInt32(command.ExecuteScalar());

                        string daQuery = @"INSERT INTO discipline_assigment (teacher_id, group_discipline_id) 
                                      VALUES (@teacherId, @groupDisciplineId)";

                        using (SqlCommand daCommand = new SqlCommand(daQuery, connection))
                        {
                            daCommand.Parameters.AddWithValue("@teacherId", teacherId);
                            daCommand.Parameters.AddWithValue("@groupDisciplineId", groupDisciplineId);
                            daCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void UpdateGroupDiscipline(int groupDisciplineId, int disciplineId, int teacherId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string gdQuery = @"
                        UPDATE group_discipline SET discipline_id = @disciplineId WHERE id = @groupDisciplineId";

                    using (SqlCommand command = new SqlCommand(gdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@disciplineId", disciplineId);
                        command.Parameters.AddWithValue("@groupDisciplineId", groupDisciplineId);
                    }

                    string daQuery = @"
                        UPDATE discipline_assigment SET teacher_id = @teacherId WHERE group_discipline_id = @groupDisciplineId";

                    using (SqlCommand command = new SqlCommand(daQuery, connection))
                    {
                        command.Parameters.AddWithValue("@teacherId", teacherId);
                        command.Parameters.AddWithValue("@groupDisciplineId", groupDisciplineId);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void DeleteGroupDiscipline(int groupDisciplineId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string daQuery = "DELETE FROM discipline_assigment WHERE group_discipline_id = @groupDisciplineId";
                    using (SqlCommand command = new SqlCommand(daQuery, connection))
                    {
                        command.Parameters.AddWithValue("@groupDisciplineId", groupDisciplineId);
                        command.ExecuteNonQuery();

                    }
                    string gdQuery = "DELETE FROM group_discipline WHERE id = @groupDisciplineId";
                    using (SqlCommand command = new SqlCommand(gdQuery, connection))
                    {
                        command.Parameters.AddWithValue("@groupDisciplineId", groupDisciplineId);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при видаленні: " + ex.Message);
                }
            }
        }

        public List<GroupListItemData> GetItemList(string searchTerm = "", int? specialityId = null)
        {
            List<GroupListItemData> list = new List<GroupListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT g.*, s.id as s_id, s.name as s_name, s.code as s_code  
                        FROM [group] g
                        JOIN speciality s ON g.speciality_id = s.id";

                    if (!string.IsNullOrEmpty(searchTerm) || specialityId.HasValue)
                    {
                        query += " WHERE";
                        bool isFirstCondition = true;

                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query += " g.code LIKE @searchTerm";
                            isFirstCondition = false;
                        }

                        if (specialityId.HasValue)
                        {
                            query += isFirstCondition ? " " : " AND ";
                            query += "g.speciality_id = @specialityId";
                        }
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }
                        if (specialityId.HasValue)
                        {
                            command.Parameters.AddWithValue("@specialityId", specialityId);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new GroupListItemData()
                                {
                                    Id = (int)reader["id"],
                                    Code = reader["code"].ToString(),
                                    SpecialityId = (int)reader["s_id"],
                                    Speciality = $"{reader["s_code"].ToString()} {reader["s_name"].ToString()}"
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

        public void CreateItem(string code, int specialityId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "INSERT INTO [group] (code, speciality_id) VALUES (@code, @speciality_id)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@code", code);
                        command.Parameters.AddWithValue("@speciality_id", specialityId);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void UpdateItem(int id, string code, int specialityId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "UPDATE [group] SET code = @code, speciality_id = @speciality_id WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@code", code);
                        command.Parameters.AddWithValue("@speciality_id", specialityId);
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
                    string query = "DELETE FROM [group] WHERE id = @id";
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
