using CourseWork.Group;
using CourseWork.Speciality;
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
using System.Windows.Shapes;

namespace CourseWork.Student
{
    /// <summary>
    /// Interaction logic for StudentCreateUpdateWindow.xaml
    /// </summary>
    public partial class StudentCreateUpdateWindow : Window
    {
        private readonly StudentService service;

        private WindowModeEnum windowMode;
        private StudentListItemData? itemData;

        public StudentCreateUpdateWindow(WindowModeEnum windowMode, StudentListItemData? itemData = null)
        {
            InitializeComponent();

            this.service = new StudentService();
            this.windowMode = windowMode;
            this.itemData = itemData;

            if (this.windowMode == WindowModeEnum.CREATE)
            {
                TextBlockTitle.Text = "Створити студента";
                ButtonAction.Content = "Створити";
            }
            else
            {
                TextBlockTitle.Text = "Редагувати студента";
                ButtonAction.Content = "Редагувати";

                TextBoxFirstName.Text = itemData?.FirstName ?? "";
                TextBoxLastName.Text = itemData?.LastName ?? "";
                TextBoxSurName.Text = itemData?.Surname ?? "";
                TextBoxPhone.Text = itemData?.Phone ?? "";
                TextBoxEmail.Text = itemData?.Email ?? "";

                ComboBoxGroup.SelectedValuePath = "Id";
                ComboBoxGroup.SelectedValue = itemData?.GroupId ?? null;
            }

            GetGroups();
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

        private void ButtonAction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string firstName = TextBoxFirstName.Text ?? "";
                string lastName = TextBoxLastName.Text ?? "";
                string surname = TextBoxSurName.Text ?? "";
                string phone = TextBoxPhone.Text ?? "";
                string email = TextBoxEmail.Text ?? "";
                string studentCard = $"{DateTime.Now:yyyyMMdd_HHmmss}";

                GroupListItemData? groupListItemData = ComboBoxGroup.SelectedItem as GroupListItemData;

                if (groupListItemData == null ||  string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
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
                    this.service.CreateItem(firstName, lastName, surname, phone, email, studentCard, groupListItemData.Id);
                    MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (itemData != null)
                    {
                        this.service.UpdateItem(itemData.Id, firstName, lastName, surname, phone, email, groupListItemData.Id);
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
