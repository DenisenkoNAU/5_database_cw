using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseWork.Student
{
    /// <summary>
    /// Interaction logic for StudentGradeUpdateWindow.xaml
    /// </summary>
    public partial class StudentGradeUpdateWindow : Window
    {
        private int studentId;
        private StudentDisciplineListItemData studentDisciplineData;

        public StudentGradeUpdateWindow(int studentId, StudentDisciplineListItemData studentDisciplineData)
        {
            InitializeComponent();

            this.studentId = studentId;
            this.studentDisciplineData = studentDisciplineData;

            TextBoxGrade.Text = studentDisciplineData.Grade?.ToString() ?? "";
        }

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int grade;
                bool isValid = int.TryParse(TextBoxGrade.Text, out grade);

                if (isValid)
                {
                    if (grade < 0 || grade > 100)
                    {
                        throw new Exception("Не валідні дані для редагування.");
                    }

                    StudentService studentService = new StudentService();
                    if (studentDisciplineData.Grade == null)
                    {
                        studentService.CreateDisciplineGradeByStudent(studentId, studentDisciplineData.DisciplineId, grade);
                    }
                    else
                    {
                        studentService.UpdateDisciplineGradeByStudent(studentId, studentDisciplineData.DisciplineId, grade);
                    }
                    MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception("Не валідні дані для редагування.");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
