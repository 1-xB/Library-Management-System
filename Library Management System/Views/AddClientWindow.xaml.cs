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
using Library_Management_System.Entity;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for AddClientWindow.xaml
    /// </summary>
    public partial class AddClientWindow : Window
    {
        public AddClientWindow()
        {
            InitializeComponent();
        }

        private void OnAddClientButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddClient()
        {
            string name = NameTextBox.Text;
            string surname = SurnameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string city = CityTextBox.Text;
            string street = StreetTextBox.Text;
            string number = HouseNumberTextBox.Text;

            if (name.Length == 0 || surname.Length == 0 || phone.Length == 0 || city.Length == 0 || street.Length == 0 || number.Length == 0)
            {
                MessageBox.Show("Please fill all fields");
                return;
            }

            using (var db = new LibraryContext())
            {
                var address = new Address
                {
                    City = city,
                    Street = street,
                    Number = number
                };
                db.Address.Add(address);
                db.SaveChanges();
                var client = new Clients
                {
                    Name = name,
                    Surname = surname,
                    Phone = phone,
                    AddressId = address.Id
                };
                db.Clients.Add(client);
                db.SaveChanges();
            }
            Close();



        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
