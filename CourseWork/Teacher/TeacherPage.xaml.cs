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
    /// Interaction logic for TeacherPage.xaml
    /// </summary>
    public partial class TeacherPage : Page
    {
        private readonly TeacherService service;

        private string searchText = "";

        public TeacherPage()
        {
            InitializeComponent();

            this.service = new TeacherService();
            GetItemList();
        }

        private void GetItemList()
        {
            try
            {
                var itemList = this.service.GetItemList(this.searchText);
                DataGrid.ItemsSource = itemList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = TextBoxSearch.Text;
            GetItemList();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = "";
            TextBoxSearch.Text = "";
            GetItemList();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new TeacherCreateUpdateWindow(WindowModeEnum.CREATE);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            GetItemList();
            EventBus.Instance.OnTeacherUpdated();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TeacherListItemData)DataGrid.SelectedItem;

            var createUpdateWindow = new TeacherCreateUpdateWindow(WindowModeEnum.UPDATE, selectedItem);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            GetItemList();
            EventBus.Instance.OnTeacherUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedItem = (TeacherListItemData)DataGrid.SelectedItem;
                this.service.DeleteItem(selectedItem.Id);

                GetItemList();
                EventBus.Instance.OnTeacherUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonView_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (TeacherListItemData)DataGrid.SelectedItem;
            this.NavigationService.Navigate(new TeacherOverviewPage(selectedItem));
        }
    }
}
