using CourseWork.Speciality;
using CourseWork.Student;
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

namespace CourseWork.Group
{
    /// <summary>
    /// Interaction logic for GroupOverviewPage.xaml
    /// </summary>
    public partial class GroupOverviewPage : Page
    {
        private GroupService groupService;

        private GroupListItemData groupData;
        private string searchTextStudent = "";
        private string searchTextDiscipline = "";
        private int? teacherIdDiscipline = null;

        public GroupOverviewPage(GroupListItemData groupData)
        {
            InitializeComponent();

            this.groupService = new GroupService();
            this.groupData = groupData;

            TextBlockCode.Text = groupData.Code ?? "";
            TextBlockSpeciality.Text = groupData.Speciality ?? "";

            GetDisciplineItemList();
            GetDisciplineTeachers();

            GetStudentItemList();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new GroupCreateUpdateWindow(WindowModeEnum.UPDATE, this.groupData);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();
            EventBus.Instance.OnGroupUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.groupService.DeleteItem(this.groupData.Id);
                EventBus.Instance.OnGroupUpdated();
                this.NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetDisciplineTeachers()
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

        private void GetDisciplineItemList()
        {
            try
            {
                var itemList = groupService.GetGroupDiscipline(groupData.Id, this.searchTextDiscipline, this.teacherIdDiscipline);
                DataGridDiscipline.ItemsSource = itemList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSearchDiscipline_Click(object sender, RoutedEventArgs e)
        {
            this.searchTextDiscipline = TextBoxSearchDiscipline.Text;

            var teacherData = ComboBoxTeacher.SelectedItem as TeacherListItemData;
            this.teacherIdDiscipline = teacherData != null ? teacherData.Id : null;

            GetDisciplineItemList();
        }

        private void ButtonClearSearchDiscipline_Click(object sender, RoutedEventArgs e)
        {
            this.searchTextDiscipline = "";
            TextBoxSearchDiscipline.Text = "";

            this.teacherIdDiscipline = null;
            ComboBoxTeacher.SelectedValue = "";

            GetDisciplineItemList();
        }

        private void ButtonAddDiscipline_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new GroupAddDisciplineWindow(groupData);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();
            GetDisciplineItemList();
        }

        private void ButtonDeleteDiscipline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (GroupDisciplineListItemData)DataGridDiscipline.SelectedItem;
                this.groupService.DeleteGroupDiscipline(selectedItem.Id);
                GetDisciplineItemList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetStudentItemList()
        {
            try
            {
                StudentService studentService = new StudentService();
                var itemList = studentService.GetItemList(this.searchTextStudent, groupData.Id);
                DataGridStudent.ItemsSource = itemList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
