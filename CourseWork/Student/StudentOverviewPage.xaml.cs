using CourseWork.Discipline;
using CourseWork.Group;
using CourseWork.Teacher;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseWork.Student
{
    /// <summary>
    /// Interaction logic for StudentOverviewPage.xaml
    /// </summary>
    public partial class StudentOverviewPage : Page
    {
        private readonly StudentService service;

        private StudentListItemData studentData;
        private int? disciplineId = null;
        private Grade? grade = null;

        public StudentOverviewPage(StudentListItemData studentData)
        {
            InitializeComponent();

            this.service = new StudentService();
            this.studentData = studentData;

            TextBlockFirstName.Text = studentData.FirstName;
            TextBlockLastName.Text = studentData.LastName;
            TextBlockSurname.Text = studentData.Surname;
            TextBlockStudentCard.Text = studentData.StudentCard;
            TextBlockStudentGroup.Text = studentData.Group;

            GetAVGGrade();
            GetDisciplineListByStudent();

            GetGrade();
            GetDisciplines();
        }

        private void GetGrade()
        {
            var list = new List<Grade>()
            {
                new Grade {
                    Min = 90,
                    Max = 100,
                    Name = "A",
                },
                new Grade {
                    Min = 82,
                    Max = 89,
                    Name = "B",
                },
                new Grade {
                    Min = 64,
                    Max = 81,
                    Name = "D",
                },
                new Grade {
                    Min = 60,
                    Max = 63,
                    Name = "E",
                },
                new Grade {
                    Min = 35,
                    Max = 59,
                    Name = "FX",
                },
                new Grade {
                    Min = 0,
                    Max = 34,
                    Name = "F",
                }
            };
            ComboBoxGrade.ItemsSource = list;
        }

        private void GetAVGGrade()
        {
            try
            {
                DisciplineService groupService = new DisciplineService();
                var grade = this.service.GetAVGGrade(this.studentData.Id);
                TextBlockStudentAvarageGrade.Text = grade.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetDisciplines()
        {
            try
            {
                DisciplineService groupService = new DisciplineService();
                var list = groupService.GetDisciplines();
                ComboBoxDiscipline.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetDisciplineListByStudent()
        {
            try
            {
                var itemList = this.service.GetDisciplineListByStudent(this.studentData.Id, this.disciplineId, this.grade);
                DataGrid.ItemsSource = itemList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new StudentCreateUpdateWindow(WindowModeEnum.UPDATE, this.studentData);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();
            EventBus.Instance.OnStudentUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.service.DeleteItem(this.studentData.Id);
                EventBus.Instance.OnStudentUpdated();
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var selectedDisciplineItem = ComboBoxDiscipline.SelectedItem as DisciplineListItemData;
            this.disciplineId = selectedDisciplineItem != null ? selectedDisciplineItem.Id : null;

            var selectedGradeItem = ComboBoxGrade.SelectedItem as Grade;
            this.grade = selectedGradeItem != null ? selectedGradeItem : null;

            GetDisciplineListByStudent();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.disciplineId = null;
            ComboBoxGrade.SelectedValue = "";

            this.grade = null;
            ComboBoxDiscipline.SelectedValue = "";

            GetDisciplineListByStudent();
        }

        private void ButtonEditGrade_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (StudentDisciplineListItemData)DataGrid.SelectedItem;
            var createUpdateWindow = new StudentGradeUpdateWindow(studentData.Id, selectedItem);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();
            GetDisciplineListByStudent();
            GetAVGGrade();
        }
    }
}
