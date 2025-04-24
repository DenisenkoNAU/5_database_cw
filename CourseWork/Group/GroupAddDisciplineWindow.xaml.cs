using CourseWork.Discipline;
using CourseWork.Speciality;
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
using System.Windows.Shapes;

namespace CourseWork.Group
{
    /// <summary>
    /// Interaction logic for GroupAddDisciplineWindow.xaml
    /// </summary>
    public partial class GroupAddDisciplineWindow : Window
    {
        private GroupListItemData groupData;

        public GroupAddDisciplineWindow(GroupListItemData groupData)
        {
            InitializeComponent();
            this.groupData = groupData;

            GetDisciplines();
            GetTeachers();
        }

        private void GetDisciplines()
        {
            try
            {
                DisciplineService service = new DisciplineService();
                var list = service.GetDisciplines();
                ComboBoxDiscipline.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetTeachers()
        {
            try
            {
                TeacherService service = new TeacherService();
                var list = service.GetItemList();
                ComboBoxTeacher.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DisciplineListItemData? disciplineData = ComboBoxDiscipline.SelectedItem as DisciplineListItemData;
                TeacherListItemData? teacherData = ComboBoxTeacher.SelectedItem as TeacherListItemData;

                if (disciplineData == null || teacherData == null)
                {
                    throw new Exception("Не передані дані для створення.");
                }

                GroupService service = new GroupService();
                service.CreateGroupDiscipline(groupData.Id, disciplineData.Id, teacherData.Id);
                MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
