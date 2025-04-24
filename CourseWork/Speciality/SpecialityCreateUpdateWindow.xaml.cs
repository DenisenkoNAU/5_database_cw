using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseWork.Speciality
{
    /// <summary>
    /// Interaction logic for SpecialityCreateUpdateWindow.xaml
    /// </summary>
    public partial class SpecialityCreateUpdateWindow : Window
    {
        private readonly SpecialityService service;

        private WindowModeEnum windowMode;
        private SpecialityListItemData? itemData;

        public SpecialityCreateUpdateWindow(WindowModeEnum windowMode, SpecialityListItemData? itemData)
        {
            InitializeComponent();

            this.service = new SpecialityService();
            this.windowMode = windowMode;
            this.itemData = itemData;

            if (this.windowMode == WindowModeEnum.CREATE)
            {
                TextBlockTitle.Text = "Створити спеціальність";
                ButtonAction.Content = "Створити";
            }
            else
            {
                TextBlockTitle.Text = "Редагувати спеціальність";
                ButtonAction.Content = "Редагувати";

                TextBoxName.Text = itemData?.Name ?? "";
                TextBoxCode.Text = itemData?.Code ?? "";
            }
        }

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = TextBoxName.Text ?? "";
                string code = TextBoxCode.Text ?? "";
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(code))
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
                    this.service.CreateItem(name, code);
                    MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (itemData != null)
                    {
                        this.service.UpdateItem(itemData.Id, name, code);
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
