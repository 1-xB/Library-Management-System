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

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClientsClick(object sender, RoutedEventArgs e)
        {
            BooksGrid.Visibility = Visibility.Collapsed;
            ClientsGrid.Visibility = Visibility.Visible;
            LoadClients();
        }

        private void OnBooksClick(object sender, RoutedEventArgs e)
        {
            ClientsGrid.Visibility = Visibility.Collapsed;
            BooksGrid.Visibility = Visibility.Visible;
        }

        private void LoadClients()
        {
            using (var context = new LibraryContext())
            {
                ClientsListBox.Items.Clear();
                for (int i = 0; i < context.Clients.Count(); i++)
                {
                    ClientsListBox.Items.Add(context.Clients.ToList()[i].FullName);
                }
            }
        }

        private void OnSelectedClientChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var context = new LibraryContext())
            {
                string selectedClient = ClientsListBox.SelectedItem.ToString();
                string[] selectedClientArray = selectedClient.Split(' ');
                int selectedClientId = int.Parse(selectedClientArray[0]);
                var client = context.Clients.Find(selectedClientId);
                var address = context.Address.Find(client.AddressId);
                // TODO : zmienić na jeden do wielu adresów, i dodać możliwość kilku adresów
                AddressListBox.Items.Clear();
                AddressListBox.Items.Add($"Address : {address.City}, {address.Street}, {address.Number}");

            }
        }
    }
}
