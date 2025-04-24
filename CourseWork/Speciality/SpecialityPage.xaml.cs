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

namespace CourseWork.Speciality
{
    /// <summary>
    /// Interaction logic for SpecialityPage.xaml
    /// </summary>
    public partial class SpecialityPage : Page
    {
        private readonly SpecialityService service;

        private string searchText = "";

        public SpecialityPage()
        {
            InitializeComponent();

            this.service = new SpecialityService();
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
            var createUpdateWindow = new SpecialityCreateUpdateWindow(WindowModeEnum.CREATE, null);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            GetItemList();
            EventBus.Instance.OnSpecialitiesUpdated();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedDiscipline = (SpecialityListItemData)DataGrid.SelectedItem;

            var createUpdateWindow = new SpecialityCreateUpdateWindow(WindowModeEnum.UPDATE, selectedDiscipline);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            GetItemList();
            EventBus.Instance.OnSpecialitiesUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedDiscipline = (SpecialityListItemData)DataGrid.SelectedItem;
                this.service.DeleteItem(selectedDiscipline.Id);

                GetItemList();
                EventBus.Instance.OnSpecialitiesUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
