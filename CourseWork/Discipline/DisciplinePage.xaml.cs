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

namespace CourseWork.Discipline
{
    /// <summary>
    /// Interaction logic for DisciplinePage.xaml
    /// </summary>
    public partial class DisciplinePage : Page
    {
        private readonly DisciplineService disciplineService;

        private string searchText = "";

        public DisciplinePage()
        {
            InitializeComponent();

            this.disciplineService = new DisciplineService();

            LoadDisciplines();
        }

        private void LoadDisciplines()
        {
            try
            {
                var disciplines = disciplineService.GetDisciplines(this.searchText);
                DataGrid.ItemsSource = disciplines;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ButtonSearch_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = TextBoxSearch.Text;
            LoadDisciplines();
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            this.searchText = "";
            TextBoxSearch.Text = "";
            LoadDisciplines();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            var createUpdateWindow = new DisciplineCreateUpdateWindow(WindowModeEnum.CREATE, null);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            LoadDisciplines();
            EventBus.Instance.OnDisciplineUpdated();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedDiscipline = (DisciplineListItemData)DataGrid.SelectedItem;

            var createUpdateWindow = new DisciplineCreateUpdateWindow(WindowModeEnum.UPDATE, selectedDiscipline);
            createUpdateWindow.Owner = Window.GetWindow(this);
            createUpdateWindow.ShowDialog();

            LoadDisciplines();
            EventBus.Instance.OnDisciplineUpdated();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedDiscipline = (DisciplineListItemData)DataGrid.SelectedItem;
                disciplineService.DeleteDiscipline(selectedDiscipline.Id);
                LoadDisciplines();
                EventBus.Instance.OnDisciplineUpdated();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
