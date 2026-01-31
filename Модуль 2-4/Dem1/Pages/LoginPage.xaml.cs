using Dem1.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace Dem1.Pages
{
    public partial class LoginPage : Page
    {
        private Frame _frame;

        public LoginPage(Frame frame)
        {
            InitializeComponent();
            _frame = frame;

            LoginTextBox.Text = "94d5ous@gmail.com";
            PasswordTextBox.Password = "uzWC67";
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new AppDbConntext();

                var user = db.Users
                    .Include(u => u.Role)
                    .FirstOrDefault(u =>
                        u.Login == LoginTextBox.Text &&
                        u.Password == PasswordTextBox.Password);

                if (user is null)
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _frame.Navigate(new ProductPage(_frame, user));
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось авторизоваться. Проверьте подключение к интернету и/или проверьте учетные данные.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Guest_Click(object sender, RoutedEventArgs e)
        {
            _frame.Navigate(new ProductPage(_frame, null));
        }
    }
}
