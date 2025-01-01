using Library_Management_System.Entity;
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

namespace Library_Management_System.Views
{
    /// <summary>
    /// Interaction logic for EditClientWindow.xaml
    /// </summary>
    public partial class EditClientWindow : Window
    {
        private Clients _client;
        private Address _address;   
        public EditClientWindow(Clients clients)
        {
            InitializeComponent();
            _client = clients;
            LoadClient();
        }

        private void LoadClient()
        {
            if (_client != null)
            {
                NameTextBox.Text = _client.Name ?? string.Empty;
                SurnameTextBox.Text = _client.Surname ?? string.Empty;
                PhoneTextBox.Text = _client.Phone ?? string.Empty;
                using (var context = new LibraryContext())
                {
                    _address = context.Address.FirstOrDefault(a => a.Id == _client.AddressId);
                }
                if (_address != null)
                {
                    CityTextBox.Text = _address.City ?? string.Empty;
                    StreetTextBox.Text = _address.Street ?? string.Empty;
                    HouseNumberTextBox.Text = _address.Number ?? string.Empty;
                }
                else
                {
                    CityTextBox.Text = string.Empty;
                    StreetTextBox.Text = string.Empty;
                    HouseNumberTextBox.Text = string.Empty;
                }
            }
        }

        private void OnSaveClientButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(SurnameTextBox.Text) || string.IsNullOrEmpty(PhoneTextBox.Text) || string.IsNullOrEmpty(CityTextBox.Text) || string.IsNullOrEmpty(StreetTextBox.Text) || string.IsNullOrEmpty(HouseNumberTextBox.Text))
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            _client.Name = NameTextBox.Text;
            _client.Surname = SurnameTextBox.Text;
            _client.Phone = PhoneTextBox.Text;
            _address.City = CityTextBox.Text;
            _address.Street = StreetTextBox.Text;
            _address.Number = HouseNumberTextBox.Text;

            using (var context = new LibraryContext())
            {
                context.Clients.Update(_client);
                context.Address.Update(_address);
                context.SaveChanges();
            }

            Close();
        }

        private void OnCancelClientButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
