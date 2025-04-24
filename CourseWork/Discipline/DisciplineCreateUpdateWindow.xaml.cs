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

namespace CourseWork.Discipline
{
    /// <summary>
    /// Interaction logic for DisciplineCreateUpdateWindow.xaml
    /// </summary>
    public partial class DisciplineCreateUpdateWindow : Window
    {
        private readonly DisciplineService disciplineService;

        private WindowModeEnum windowMode;
        private DisciplineListItemData? itemData;

        public DisciplineCreateUpdateWindow(WindowModeEnum windowMode, DisciplineListItemData? itemData)
        {
            InitializeComponent();

            this.disciplineService = new DisciplineService();
            this.windowMode = windowMode;
            this.itemData = itemData;

            if (this.windowMode == WindowModeEnum.CREATE)
            {
                TextBlockTitle.Text = "Створити дисципліну";
                ButtonAction.Content = "Створити";
                ButtonAction.Content = "Створити";
            }
            else
            {
                TextBlockTitle.Text = "Редагувати дисципліну";
                ButtonAction.Content = "Редагувати";

                TextBoxName.Text = itemData?.Name ?? "";
            }
        }

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = TextBoxName.Text ?? "";
                if (string.IsNullOrEmpty(name))
                {
                    if (this.windowMode == WindowModeEnum.CREATE)
                    {
                        throw new Exception("Не передані дані для створення.");
                    }
                    else
                    {
                        throw new Exception("Не передані дані для редагування.");
                    }
                }

                if (this.windowMode == WindowModeEnum.CREATE)
                {
                    disciplineService.CreateDiscipline(name);
                    MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (itemData != null)
                    {
                        disciplineService.UpdateDiscipline(itemData.Id, name);
                        MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        throw new Exception("Не передані дані для редагування.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
