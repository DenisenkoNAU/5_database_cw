using CourseWork.Speciality;
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

namespace CourseWork.Group
{
    /// <summary>
    /// Interaction logic for GroupCreateUpdateWindow.xaml
    /// </summary>
    public partial class GroupCreateUpdateWindow : Window
    {
        private readonly GroupService service;

        private WindowModeEnum windowMode;
        private GroupListItemData? itemData;

        public GroupCreateUpdateWindow(WindowModeEnum windowMode, GroupListItemData? itemData = null)
        {
            InitializeComponent();

            this.service = new GroupService();
            this.windowMode = windowMode;
            this.itemData = itemData;

            if (this.windowMode == WindowModeEnum.CREATE)
            {
                TextBlockTitle.Text = "Створити групу";
                ButtonAction.Content = "Створити";
            }
            else
            {
                TextBlockTitle.Text = "Редагувати групу";
                ButtonAction.Content = "Редагувати";

                TextBoxCode.Text = itemData?.Code ?? "";
                ComboBoxSpeciality.SelectedValuePath = "Id";
                ComboBoxSpeciality.SelectedValue = itemData?.SpecialityId ?? null;
            }

            GetSpecialities();
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

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string code = TextBoxCode.Text;
                SpecialityListItemData? specialityListItemData = ComboBoxSpeciality.SelectedItem as SpecialityListItemData;

                if (specialityListItemData == null || string.IsNullOrEmpty(code))
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
                    this.service.CreateItem(code, specialityListItemData.Id);
                    MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (itemData != null)
                    {
                        this.service.UpdateItem(itemData.Id, code, specialityListItemData.Id);
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
