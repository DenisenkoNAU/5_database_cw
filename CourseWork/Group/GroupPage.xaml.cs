using CourseWork.Speciality;
using CourseWork.Student;
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
    /// Interaction logic for GroupPage.xaml
    /// </summary>
    public partial class GroupPage : Page
    {
        private readonly GroupService service;
        private string searchText = "";
        private int? specialityId = null;

        public GroupPage()
        {
            InitializeComponent();

            this.service = new GroupService();

            GetItemList();
            GetSpecialities();

            WeakEventManager<EventBus, EventArgs>.AddHandler(EventBus.Instance, nameof(EventBus.SpecialitiesUpdated), OnSpecialitiesUpdated);
        }

        private void GetItemList()
        {
            try
            {
                var itemList = this.service.GetItemList(this.searchText, specialityId);
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

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = "";
            this.specialityId = null;
            TextBoxSearch.Text = "";
            ComboBoxSpeciality.SelectedValue = "";
            GetItemList();
        }
        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = TextBoxSearch.Text;
            var selectedItem = ComboBoxSpeciality.SelectedItem as SpecialityListItemData;
            this.specialityId = selectedItem != null ? selectedItem.Id : null;
            GetItemList();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new GroupCreateUpdateWindow(WindowModeEnum.CREATE);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();
            GetItemList();
            EventBus.Instance.OnGroupUpdated();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GroupListItemData)DataGrid.SelectedItem;

            var createUpdateWindow = new GroupCreateUpdateWindow(WindowModeEnum.UPDATE, selectedItem);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();
            GetItemList();
            EventBus.Instance.OnGroupUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (GroupListItemData)DataGrid.SelectedItem;
                this.service.DeleteItem(selectedItem.Id);
                GetItemList();
                EventBus.Instance.OnGroupUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonView_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (GroupListItemData)DataGrid.SelectedItem;
            this.NavigationService.Navigate(new GroupOverviewPage(selectedItem));
        }
    }
}
