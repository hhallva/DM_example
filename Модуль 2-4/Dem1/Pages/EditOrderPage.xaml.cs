using Dem1.Contexts;
using Dem1.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace Dem1.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditOrderPage.xaml
    /// </summary>
    public partial class EditOrderPage : Page
    {
        private Frame _frame;
        private Order _order;


        private AppDbConntext db = new AppDbConntext();
        private int _id;

        public EditOrderPage(Frame frame, int id)
        {
            InitializeComponent();
            _frame = frame;
            _id = id;

            try
            {
                LoadLists();

                if (id != 0)
                {
                    LoadOrder(id);
                    Title = "Редактирование заказа";
                }
                else
                {
                    _order = new Order();
                    Title = "Добавление заказа";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о заказе. Проверьте соединение с интернетом или повторите позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLists()
        {
            PickupPointComboBox.ItemsSource = db.PickupPoints.ToList();
            ProductsComboBox.ItemsSource = db.Products.ToList();
        }

        private void LoadOrder(int id)
        {
            //try
            //{
            _order = db.Orders
            .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
            .Include(o => o.PickupPoint)
            .First(o => o.Id == id);

            CodeTextBox.Text = "";


            

            switch (_order.Status.Trim())
            {
                case "Новый": StatusComboBox.SelectedIndex = 0; break;
                case "Завершен": StatusComboBox.SelectedIndex = 1; break;
            }

            PickupPointComboBox.Text = _order.PickupPoint.Address;
            DateDatePicker.SelectedDate = _order.Date;

            IEnumerable<OrderProduct> products = _order.OrderProducts.ToList();
            ProductListView.ItemsSource = products;
            LoadActicle();

            //}
            //    catch (Exception)
            //    {
            //        throw;
            //       // MessageBox.Show("Произошла ошибка при загрузке данных о заказе. Проверьте соединение с интернетом или повторите позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
        }

        private void LoadActicle()
        {
            IEnumerable<OrderProduct> products = _order.OrderProducts.ToList();
            CodeTextBox.Text = string.Join(";", products.Select(p => p.Product.Code));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
            => _frame.GoBack();

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult dialog = MessageBox.Show("Вы уверены что хотите удалить данный заказ?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog == MessageBoxResult.Yes)
                {
                    db.Orders.Remove(_order);
                    db.SaveChanges();
                    _frame.GoBack();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при удалении данных о заказе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            if(ProductsComboBox.SelectedItem != null && 
                ProductsComboBox.SelectedItem is Product product){
                var newProduct = new OrderProduct
                {
                    OrderId = _order.Id,
                    ProductId = product.Id,
                    Amount = int.Parse(AmountTextBox.Text),
                };


                _order.OrderProducts.Add(newProduct);
                
                LoadActicle();
            }

        }
    }
}
