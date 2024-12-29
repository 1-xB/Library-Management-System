using Library_Management_System.Entity;
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
            try
            {
                using (var context = new LibraryContext())
                {
                    string selectedClient = ClientsListBox.SelectedItem.ToString();
                    if (selectedClient == null) return;

                    // Load Client
                    var client = FindClient(selectedClient);

                    // Load Address
                    var address = context.Address.Find(client.AddressId);
                    AddressListBox.Items.Clear();
                    AddressListBox.Items.Add($"Address : {address.City}, {address.Street}, {address.Number}");

                    // Load Loans
                    var loans = context.Loan.Where(l => l.ClientId == client.Id).ToList();
                    BorrowedBooksListBox.Items.Clear();
                    foreach (var loan in loans)
                    {
                        var book = context.Books.Find(loan.BookId);
                        BorrowedBooksListBox.Items.Add($"{book.Title} by {book.Author}. Book Borrowed {loan.LoanDate.ToString()}, will be returned {loan.ReturnDate}");
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private Clients FindClient(string selectedClient)
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    string[] selectedClientArray = selectedClient.Split(' ');
                    int selectedClientId = int.Parse(selectedClientArray[0]);
                    return context.Clients.Find(selectedClientId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

        }

        private void OnAddClientClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddClientWindow addClientWindow = new AddClientWindow();
                addClientWindow.ShowDialog();
                LoadClients();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    if (ClientsListBox.SelectedItem == null) return;
                    string selectedClient = ClientsListBox.SelectedItem.ToString();

                    var client = FindClient(selectedClient);
                    if (client == null) return;

                    context.Clients.Remove(client);
                    var address = context.Address.Find(client.AddressId);
                    if (address != null)
                    {
                        context.Address.Remove(address);
                    }

                    var loans = context.Loan.Where(l => l.ClientId == client.Id).ToList();
                    if (loans.Any())
                    {
                        context.Loan.RemoveRange(loans);
                    }

                    context.SaveChanges();
                    LoadClients();
                }
            }
            catch (Exception ex)
            {
                
            }
          

        }

    }
}
