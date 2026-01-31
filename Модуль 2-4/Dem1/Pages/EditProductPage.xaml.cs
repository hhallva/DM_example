using Dem1.Contexts;
using Dem1.Models;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Dem1.Pages
{
    /// <summary>
    /// Логика взаимодействия для EditProductPage.xaml
    /// </summary>
    public partial class EditProductPage : Page
    {
        private Frame _frame;
        private Product _product;
        private string _newImagePath;

        private AppDbConntext db = new AppDbConntext();

        public EditProductPage(Frame frame, int id)
        {
            InitializeComponent();
            _frame = frame;

            try
            {
                LoadLists();

                if (id != 0)
                {
                    LoadProduct(id);
                    Title = "Редактирование товара";
                }
                else
                {
                    _product = new Product();
                    Title = "Добавление товара";
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о товаре. Проверьте соединение с интернетом или повторите позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLists()
        {

            CategoryComboBox.ItemsSource = db.Categories.ToList();
            ManufacturerComboBox.ItemsSource = db.Manufacturers.ToList();
            ProviderComboBox.ItemsSource = db.Providers.ToList();

        }

        private void LoadProduct(int id)
        {
            try
            {
                _product = db.Products
                    .First(p => p.Id == id);

                CodeTextBox.Text = _product.Code;
                NameTextBox.Text = _product.Name;
                DescriptionTextBox.Text = _product.Desctription;
                PriceTextBox.Text = _product.Price.ToString();
                UnitTextBox.Text = _product.Unit;
                DiscountTextBox.Text = _product.Discount.ToString();
                AmountTextBox.Text = _product.Amount.ToString();

                CategoryComboBox.SelectedItem = _product.Category;
                ManufacturerComboBox.SelectedItem = _product.Manufacturer;
                ProviderComboBox.SelectedItem = _product.Provider;

                ProductImage.Source = new BitmapImage(new Uri(_product.PhotoPath));

            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о товаре. Проверьте соединение с интернетом или повторите позже.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Image|*.png;*.jpg";

                if (dialog.ShowDialog() is not true)
                    return;

                var image = new BitmapImage(new Uri(dialog.FileName));

                if (image.PixelWidth > 300 || image.PixelHeight > 200)
                {
                    MessageBox.Show("Максимальный размер изображения - 300х200 пикселей", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var folder = Path.Combine(Environment.CurrentDirectory, "Images");
                Directory.CreateDirectory(folder);

                _newImagePath = Path.Combine(folder, Path.GetFileName(dialog.FileName));

                File.Copy(dialog.FileName, _newImagePath, true); //System.IO.IOException: "The process cannot access the file 'C:\Users\221\students\ispp-21\DM_example\Модуль 2-4\Dem1\bin\Debug\net8.0-windows\Images\photo.jpg' because it is being used by another process."
                ProductImage.Source = image;

            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при выборе изображения", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
            => _frame.GoBack();

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            _product.Code = CodeTextBox.Text;
            _product.Name = NameTextBox.Text;
            _product.Desctription = DescriptionTextBox.Text;
            _product.Price = decimal.Parse(PriceTextBox.Text);
            if (_product.Price < 0)
            {
                MessageBox.Show("Цена не должна быть отрицательной", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product.Unit = UnitTextBox.Text;
            _product.Amount = int.Parse(AmountTextBox.Text);
            if (_product.Amount < 0)
            {
                MessageBox.Show("Количество не должно быть отрицательным", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product.Discount = int.Parse(DiscountTextBox.Text);
            if (_product.Discount < 0 || _product.Discount > 100)
            {
                MessageBox.Show("Скидка должна быть в промежутке от 0 до 100", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            _product.Provider = ProviderComboBox.SelectedItem as Provider;
            _product.Manufacturer = ManufacturerComboBox.SelectedItem as Manufacturer;
            _product.Category = CategoryComboBox.SelectedItem as Category;

            if (!String.IsNullOrWhiteSpace(_newImagePath))
                _product.Photo = Path.GetFileName(_newImagePath);

            db.Products.Update(_product);
            db.SaveChanges();

            _frame.GoBack();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (db.OrderProducts.Any(p => p.ProductId == _product.Id))
                {
                    MessageBox.Show("Товар нельзя удалить так как он присутствует в заказе", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBoxResult dialog = MessageBox.Show("Вы уверены что хотите удалить данный товар?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dialog == MessageBoxResult.Yes)
                {
                    db.Products.Remove(_product);
                    db.SaveChanges();
                    _frame.GoBack();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Произошла ошибка при удалении данных о товаре", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
