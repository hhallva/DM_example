using Dem1.Contexts;
using Dem1.Models;
using Microsoft.EntityFrameworkCore;
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

namespace Dem1.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private readonly Frame _frame;
        private readonly User _user;
        private List<Order> _orders;

        public OrdersPage(Frame frame, User user)
        {
            InitializeComponent();
            _frame = frame;
            _user = user;

            _frame.Navigated += _frame_Navigated;

            LoadOrders();
        }

        private void LoadOrders()
        {
            using var db = new AppDbConntext();

            _orders = [.. db.Orders
                 .Include(p => p.OrderProducts)
                    .ThenInclude(op => op.Product)
                 .Include(p => p.PickupPoint)];

            OrdersListView.ItemsSource = _orders;
        }

        private void _frame_Navigated(object sender, NavigationEventArgs e)
        {
            LoadOrders();
        }

        private void AddOrder_Click(object sender, RoutedEventArgs e) 
            => _frame.Navigate(new EditOrderPage(_frame, 0));

        private void OrdersListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (_user.Role.Name == "Администратор" &&
                    OrdersListView.SelectedItem is Order order)
                    _frame.Navigate(new EditOrderPage(_frame, order.Id));
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось перейти к странице реактирования товара. Проверьте подключение к интернету и/или проверьте выбанный товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            _frame.GoBack();
        }
    }
}
