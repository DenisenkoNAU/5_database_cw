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

namespace CourseWork.Teacher
{
    /// <summary>
    /// Interaction logic for TeacherCreateUpdateWindow.xaml
    /// </summary>
    public partial class TeacherCreateUpdateWindow : Window
    {
        private readonly TeacherService service;

        private WindowModeEnum windowMode;
        private TeacherListItemData? itemData;

        public TeacherCreateUpdateWindow(WindowModeEnum windowMode, TeacherListItemData? itemData = null)
        {
            InitializeComponent();

            this.service = new TeacherService();
            this.windowMode = windowMode;
            this.itemData = itemData;

            if (this.windowMode == WindowModeEnum.CREATE)
            {
                TextBlockTitle.Text = "Створити вчителя";
                ButtonAction.Content = "Створити";
            }
            else
            {
                TextBlockTitle.Text = "Редагувати вчителя";
                ButtonAction.Content = "Редагувати";

                TextBoxFirstName.Text = itemData?.FirstName ?? "";
                TextBoxLastName.Text = itemData?.LastName ?? "";
                TextBoxSurName.Text = itemData?.Surname ?? "";
                TextBoxPhone.Text = itemData?.Phone ?? "";
                TextBoxEmail.Text = itemData?.Email ?? "";
                TextBoxPosition.Text = itemData?.Position ?? "";
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
                string position = TextBoxPosition.Text ?? "";

                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
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
                    this.service.CreateItem(firstName, lastName, surname, phone, email, position);
                    MessageBox.Show("Успішно!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    if (itemData != null)
                    {
                        this.service.UpdateItem(itemData.Id, firstName, lastName, surname, phone, email, position);
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
