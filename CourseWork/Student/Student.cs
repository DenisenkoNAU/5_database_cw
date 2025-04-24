using CourseWork.Teacher;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace CourseWork.Student
{
    public class Grade
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public string? Name { get; set; }

        public override string ToString()
        {
            return Name ?? "";
        }
    }

    public class StudentListItemData
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? StudentCard { get; set; }
        public int? GroupId { get; set; }
        public string? Group { get; set; }
        public int? SpecialityId { get; set; }
        public string? Speciality { get; set; }
    }

    public class StudentDisciplineListItemData
    {
        public int? Grade { get; set; }
        public int DisciplineId { get; set; }
        public string? Discipline { get; set; }
    }

    public class StudentService
    {
        private readonly string connectionString;

        public StudentService()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;
        }

        public List<StudentDisciplineListItemData> GetDisciplineListByStudent(int studentId, int? disciplineId, Grade? grade)
        {
            List<StudentDisciplineListItemData> list = new List<StudentDisciplineListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT DISTINCT d.id, d.name, ISNULL(sd.grade, NULL) AS grade
                        FROM discipline d
                        JOIN student_group sg ON sg.student_id = @studentId
                        JOIN group_discipline gd ON gd.discipline_id = d.id AND gd.group_id = sg.group_id
                        LEFT JOIN student_discipline sd ON sd.discipline_id = d.id AND sd.student_id = @studentId
                        WHERE 1=1";

                    if (disciplineId != null)
                    {
                        query += " AND d.id = @disciplineId";
                    }

                    int? minGrade = grade?.Min ?? null;
                    int? maxGrade = grade?.Max ?? null;
                    if (minGrade != null && maxGrade != null)
                    {
                        query += " AND sd.grade BETWEEN @minGrade AND @maxGrade";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);

                        if (disciplineId != null)
                        {
                            command.Parameters.AddWithValue("@disciplineId", disciplineId);
                        }

                       
                        if (minGrade != null && maxGrade != null)
                        {
                            command.Parameters.AddWithValue("@minGrade", minGrade);
                            command.Parameters.AddWithValue("@maxGrade", maxGrade);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new StudentDisciplineListItemData()
                                {
                                    Grade = reader["grade"] != DBNull.Value ? (int?)reader["grade"] : null,
                                    DisciplineId = (int)reader["id"],
                                    Discipline = reader["name"].ToString()
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

        public double GetAVGGrade(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"SELECT AVG(grade) from student_discipline
                                    WHERE student_id = @studentId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        var result = command.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void CreateDisciplineGradeByStudent(int studentId, int disciplineId, int grade)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"INSERT INTO student_discipline (student_id, discipline_id, grade) 
                                    VALUES (@studentId, @disciplineId, @grade)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        command.Parameters.AddWithValue("@disciplineId", disciplineId);
                        command.Parameters.AddWithValue("@grade", grade);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void UpdateDisciplineGradeByStudent(int studentId, int disciplineId, int grade)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"UPDATE student_discipline SET grade = @grade 
                                    WHERE student_id = @studentId AND discipline_id = @disciplineId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@studentId", studentId);
                        command.Parameters.AddWithValue("@disciplineId", disciplineId);
                        command.Parameters.AddWithValue("@grade", grade);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public List<StudentListItemData> GetItemList(string searchTerm = "", int? groupId = null, int? specialityId = null)
        {
            List<StudentListItemData> list = new List<StudentListItemData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"
                        SELECT s.*,
                        g.id AS group_id, g.code AS group_code,
                        sp.id AS speciality_id, sp.code AS speciality_code, sp.name AS speciality_name
                        FROM student s
                        JOIN student_group sg ON s.id = sg.student_id
                        JOIN [group] g ON sg.group_id = g.id
                        JOIN speciality sp ON g.speciality_id = sp.id
                        WHERE 1=1";

                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        query += @" AND (s.first_name LIKE @searchTerm
                                 OR s.last_name LIKE @searchTerm
                                 OR s.student_card LIKE @searchTerm)"
                        ;
                    }

                    if (groupId.HasValue)
                    {
                        query += " AND g.id = @groupId";
                    }

                    if (specialityId.HasValue)
                    {
                        query += " AND sp.id = @specialityId";
                    }

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        }

                        if (groupId.HasValue)
                        {
                            command.Parameters.AddWithValue("@groupId", groupId);
                        }

                        if (specialityId.HasValue)
                        {
                            command.Parameters.AddWithValue("@specialityId", specialityId);
                        }

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new StudentListItemData()
                                {
                                    Id = (int)reader["id"],
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    Surname = reader["surname"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Email = reader["email"].ToString(),
                                    StudentCard = reader["student_card"].ToString(),
                                    GroupId = (int)reader["group_id"],
                                    Group = reader["group_code"].ToString(),
                                    SpecialityId = (int)reader["speciality_id"],
                                    Speciality = reader["speciality_code"].ToString() + " " + reader["speciality_name"].ToString()
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

        public void CreateItem(string firstName, string lastName, string surname, string phone, string email, string studentCard, int groupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"INSERT INTO student (first_name, last_name, surname,  phone, email, student_card) 
                                    VALUES (@firstName, @lastName, @surname, @phone, @email, @studentCard)
                                    SELECT SCOPE_IDENTITY();";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@surname", surname);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@studentCard", studentCard);

                        int studentId = Convert.ToInt32(command.ExecuteScalar());

                        string groupQuery = @"INSERT INTO student_group (student_id, group_id) 
                                      VALUES (@studentId, @groupId)";

                        using (SqlCommand groupCommand = new SqlCommand(groupQuery, connection))
                        {
                            groupCommand.Parameters.AddWithValue("@studentId", studentId);
                            groupCommand.Parameters.AddWithValue("@groupId", groupId);
                            groupCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Помилка при створенні: " + ex.Message);
                }
            }
        }

        public void UpdateItem(int id, string firstName, string lastName, string surname, string phone, string email, int groupId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string query = @"UPDATE student
                        SET first_name = @firstName, last_name = @lastName, surname = @surname, phone = @phone, email = @email
                        WHERE id = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@surname", surname);
                        command.Parameters.AddWithValue("@phone", phone);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }

                    string groupQuery = @"UPDATE student_group
                        SET group_id = @groupId
                        WHERE student_id = @id";

                    using (SqlCommand groupCommand = new SqlCommand(groupQuery, connection))
                    {
                        groupCommand.Parameters.AddWithValue("@groupId", groupId);
                        groupCommand.Parameters.AddWithValue("@id", id);
                        groupCommand.ExecuteNonQuery();
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
                    string stGroupQuery = "DELETE FROM student_group WHERE student_id = @id";
                    using (SqlCommand stGroupCommand = new SqlCommand(stGroupQuery, connection))
                    {
                        stGroupCommand.Parameters.AddWithValue("@id", id);
                        stGroupCommand.ExecuteNonQuery();
                    }

                    string stDisciplineQuery = "DELETE FROM student_discipline WHERE student_id = @id";
                    using (SqlCommand stDisciplineCommand = new SqlCommand(stDisciplineQuery, connection))
                    {
                        stDisciplineCommand.Parameters.AddWithValue("@id", id);
                        stDisciplineCommand.ExecuteNonQuery();
                    }

                    string query = "DELETE FROM student WHERE id = @id";
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
