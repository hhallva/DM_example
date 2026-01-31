using Dem1.Contexts;
using Dem1.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace Dem1.Pages
{
    public partial class ProductPage : Page
    {
        private readonly Frame _frame;
        private readonly User _user;
        private List<Product> _products;
        private string _oldImagePath;

        public ProductPage(Frame frame, User user)
        {
            InitializeComponent();

            _frame = frame;
            _user = user;

            _frame.Navigated += Frame_Navigated;

            try
            {
                LoadUser();
                LoadProducts();
                LoadProviders();

                ProviderComboBox.SelectedIndex = 0;
                SortComboBox.SelectedIndex = 0;
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных. Проверьте соединение с интернетом или повторите позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Frame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            LoadProducts();
            FiterChanged(null, null);
        }

        private void LoadProviders()
        {
            using var db = new AppDbConntext();

            ProviderComboBox.Items.Clear();
            ProviderComboBox.Items.Add("Все поставщики");

            var providers = db.Providers.Select(p => p.Name);

            foreach (var provider in providers)
                ProviderComboBox.Items.Add(provider);
        }

        private void LoadUser()
        {
            FullNameTextBlock.Text = _user is null ? "Гость" : _user.FullName;

            FilterStackPanel.Visibility =
                (_user is not null && (_user.Role.Name == "Администратор" || _user.Role.Name == "Менеджер"))
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            AddProduct.Visibility =
                (_user is not null && (_user.Role.Name == "Администратор"))
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            Orders.Visibility =
               (_user is not null && (_user.Role.Name == "Администратор" || _user.Role.Name == "Менеджер"))
                   ? Visibility.Visible
                   : Visibility.Collapsed;
        }

        private void LoadProducts()
        {
            using var db = new AppDbConntext();

            _products = [.. db.Products
                 .Include(p => p.Manufacturer)
                 .Include(p => p.Provider)
                 .Include(p => p.Category)];

            ProductListView.ItemsSource = _products;
        }

        private void FiterChanged(object sender, EventArgs e)
        {
            try
            {
                IEnumerable<Product> result = _products;

                string search = SearchTextBox.Text;

                if (!string.IsNullOrEmpty(search))
                {
                    result = result.Where(p =>
                        p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Desctription.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Manufacturer.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Provider.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        p.Code.Contains(search, StringComparison.OrdinalIgnoreCase));
                }

                if (ProviderComboBox.SelectedIndex > 0)
                {
                    string provider = ProviderComboBox.SelectedItem.ToString();
                    result = result.Where(p => p.Provider.Name == provider);
                }

                switch (SortComboBox.SelectedIndex)
                {
                    case 1: result = result.OrderBy(p => p.Amount); break;
                    case 2: result = result.OrderByDescending(p => p.Amount); break;
                }

                ProductListView.ItemsSource = result.ToList();
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось отобразить данные. Проверьте подключение к интернету.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                if (_user is not null &&
                    _user.Role.Name == "Администратор" &&
                    ProductListView.SelectedItem is Product product)
                    _frame.Navigate(new EditProductPage(_frame, product.Id));
            }
            catch (Exception)
            {
                MessageBox.Show("Не удалось перейти к странице реактирования товара. Проверьте подключение к интернету и/или проверьте выбанный товар.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
            => _frame.Navigate(new EditProductPage(_frame, 0));

        private void Logout_Click(object sender, RoutedEventArgs e)
            => _frame.GoBack();

        private void Orders_Click(object sender, RoutedEventArgs e)
            => _frame.Navigate(new OrdersPage(_frame, _user));
        
    }
}