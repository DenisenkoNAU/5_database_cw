using CourseWork.Discipline;
using CourseWork.Group;
using CourseWork.Speciality;
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

namespace CourseWork.Teacher
{
    /// <summary>
    /// Interaction logic for TeacherOverviewPage.xaml
    /// </summary>
    public partial class TeacherOverviewPage : Page
    {
        private readonly TeacherService service;

        private TeacherListItemData teacherData;
        private int? groupId = null;
        private int? disciplineId = null;

        public TeacherOverviewPage(TeacherListItemData teacherData)
        {
            InitializeComponent();

            this.service = new TeacherService();
            this.teacherData = teacherData;

            TextBlockFirstName.Text = teacherData.FirstName;
            TextBlockLastName.Text = teacherData.LastName;
            TextBlockSurname.Text = teacherData.Surname;
            TextBlockPosition.Text = teacherData.Position;

            GetDisciplineListByTeacher();
            GetGroups();
            GetDisciplines();
        }

        private void GetGroups()
        {
            try
            {
                GroupService groupService = new GroupService();
                var list = groupService.GetItemList();
                ComboBoxGroup.ItemsSource = list;
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
                DisciplineService service = new DisciplineService();
                var list = service.GetDisciplines();
                ComboBoxDiscipline.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetDisciplineListByTeacher()
        {
            try
            {
                var itemList = this.service.GetDisciplineListByTeacher(this.teacherData.Id, this.groupId, this.disciplineId);
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
            var createUpdateWindow = new TeacherCreateUpdateWindow(WindowModeEnum.UPDATE, this.teacherData);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            EventBus.Instance.OnTeacherUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.service.DeleteItem(this.teacherData.Id);
                EventBus.Instance.OnTeacherUpdated();
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            var selectedGroupItem = ComboBoxGroup.SelectedItem as GroupListItemData;
            this.groupId = selectedGroupItem != null ? selectedGroupItem.Id : null;

            var selectedDisciplineItem = ComboBoxDiscipline.SelectedItem as DisciplineListItemData;
            this.disciplineId = selectedDisciplineItem != null ? selectedDisciplineItem.Id : null;

            GetDisciplineListByTeacher();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.groupId = null;
            ComboBoxGroup.SelectedValue = "";

            this.disciplineId = null;
            ComboBoxDiscipline.SelectedValue = "";
            GetDisciplineListByTeacher();
        }
    }
}
