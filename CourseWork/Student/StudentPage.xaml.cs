using CourseWork.Group;
using CourseWork.Speciality;
using CourseWork.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for StudentPage.xaml
    /// </summary>
    public partial class StudentPage : Page
    {
        private readonly StudentService service;

        private string searchText = "";
        private int? groupId = null;
        private int? specialityId = null;

        public StudentPage()
        {
            InitializeComponent();

            this.service = new StudentService();

            GetItemList();

            GetSpecialities();

            GetGroups();

            WeakEventManager<EventBus, EventArgs>.AddHandler(EventBus.Instance, nameof(EventBus.SpecialitiesUpdated), OnSpecialitiesUpdated);
            WeakEventManager<EventBus, EventArgs>.AddHandler(EventBus.Instance, nameof(EventBus.GroupUpdated), OnGroupUpdated);
        }

        private void GetItemList()
        {
            try
            {
                var itemList = this.service.GetItemList(this.searchText, this.groupId, this.specialityId);
                DataGrid.ItemsSource = itemList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSpecialitiesUpdated(object sender, EventArgs e)
        {
            GetSpecialities();

            var selectedItem = ComboBoxSpeciality.SelectedItem as SpecialityListItemData;
            this.specialityId = selectedItem != null ? selectedItem.Id : null;
            GetItemList();
        }
        private void GetSpecialities()
        {
            try
            {
                SpecialityService specialityService = new SpecialityService();
                var list = specialityService.GetItemList();
                ComboBoxSpeciality.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnGroupUpdated(object sender, EventArgs e)
        {
            GetGroups();

            var selectedItem = ComboBoxGroup.SelectedItem as GroupListItemData;
            this.groupId = selectedItem != null ? selectedItem.Id : null;
            GetItemList();
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

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = TextBoxSearch.Text;

            var selectedGroupItem = ComboBoxGroup.SelectedItem as GroupListItemData;
            this.groupId = selectedGroupItem != null ? selectedGroupItem.Id : null;

            var selectedSpecialityItem = ComboBoxSpeciality.SelectedItem as SpecialityListItemData;
            this.specialityId = selectedSpecialityItem != null ? selectedSpecialityItem.Id : null;

            GetItemList();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = "";
            TextBoxSearch.Text = "";

            this.groupId = null;
            ComboBoxGroup.SelectedValue = "";

            this.specialityId = null;
            ComboBoxSpeciality.SelectedValue = "";

            GetItemList();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new StudentCreateUpdateWindow(WindowModeEnum.CREATE);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            GetItemList();
            EventBus.Instance.OnStudentUpdated();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (StudentListItemData)DataGrid.SelectedItem;

            var createUpdateWindow = new StudentCreateUpdateWindow(WindowModeEnum.UPDATE, selectedItem);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            GetItemList();
            EventBus.Instance.OnStudentUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (StudentListItemData)DataGrid.SelectedItem;
                this.service.DeleteItem(selectedItem.Id);

                GetItemList();
                EventBus.Instance.OnStudentUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonView_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (StudentListItemData)DataGrid.SelectedItem;
            this.NavigationService.Navigate(new StudentOverviewPage(selectedItem));
        }
    }
}
