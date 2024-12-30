using Library_Management_System.Entity;
using Library_Management_System.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
            LoadAllBooks();
        }

        private void LoadAllBooks()
        {
            using (var context = new LibraryContext())
            {
                AllBooksListBox.Items.Clear();
                for (int i = 0; i < context.Books.Count(); i++)
                {
                    AllBooksListBox.Items.Add(context.Books.ToList()[i].Display);
                }
            }
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


        private void OnSelectedBookChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (AllBooksListBox.SelectedItem == null) return;
                string selectedBook = AllBooksListBox.SelectedItem.ToString();
                LoadAllLoans(selectedBook);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadAllLoans(string selectedBook)
        {
            var book = FindBook(selectedBook);

            using (var context = new LibraryContext())
            {
                var loans = context.Loan.Where(l => l.BookId == book.Id).ToList();
                if (!loans.Any())
                {
                    CurrentLoansListBox.Items.Clear();
                    CurrentLoansListBox.Items.Add("No loans for this book");
                    return;
                };
                CurrentLoansListBox.Items.Clear();
                foreach (var loan in loans)
                {
                    var client = context.Clients.Find(loan.ClientId);
                    CurrentLoansListBox.Items.Add($"{client.FullNameWithoutId} borrowed {book.Title} by {book.Author}. Book Borrowed {loan.LoanDate.ToString()}, will be returned {loan.ReturnDate}");
                }
            }
        }

        private Books FindBook(string selectedBook)
        {
            using (var context = new LibraryContext())
            {
                string[] selectedBookArray = selectedBook.Split(' ');
                int selectedBookId = int.Parse(selectedBookArray[0]);
                return context.Books.Find(selectedBookId);
            }
        }

        private void OnAddBookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                AddBookWindow addBookWindow = new AddBookWindow();
                addBookWindow.ShowDialog();
                LoadAllBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OnRemoveBookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    if (AllBooksListBox.SelectedItem == null) return;
                    string selectedBook = AllBooksListBox.SelectedItem.ToString();
                    var book = FindBook(selectedBook);
                    if (book == null) return;
                    context.Books.Remove(book);
                    var loans = context.Loan.Where(l => l.BookId == book.Id).ToList();
                    if (loans.Any())
                    {
                        context.Loan.RemoveRange(loans);
                    }
                    context.SaveChanges();
                    LoadAllBooks();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void OnSelectedClientChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ClientsListBox.SelectedItem == null) return;
                string selectedClient = ClientsListBox.SelectedItem.ToString();
                LoadClientDetails(selectedClient);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void LoadClientDetails(string selectedClient)
        {
            using (var context = new LibraryContext())
            {
                // Load Client
                var client = FindClient(selectedClient);

                // Load Address
                var address = context.Address.Find(client.AddressId);
                AddressListBox.Items.Clear();
                AddressListBox.Items.Add($"Address : {address.City}, {address.Street}, {address.Number}");

                // Load Loans
                var loans = context.Loan.Where(l => l.ClientId == client.Id).ToList();
                BorrowedBooksListBox.Items.Clear();
                if (!loans.Any())
                {
                    BorrowedBooksListBox.Items.Add("No books borrowed");
                    return;
                }
                foreach (var loan in loans)
                {
                    var book = context.Books.Find(loan.BookId);
                    BorrowedBooksListBox.Items.Add($"{book.Title} by {book.Author}. Book Borrowed {loan.LoanDate.ToString()}, will be returned {loan.ReturnDate}");
                }

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

        private void OnEditButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    if (ClientsListBox.SelectedItem == null) return;
                    string selectedClient = ClientsListBox.SelectedItem.ToString();

                    var client = FindClient(selectedClient);
                    if (client == null) return;

                    EditClientWindow editClientWindow = new EditClientWindow(client);
                    editClientWindow.ShowDialog();
                    LoadClients();
                    LoadClientDetails(selectedClient);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
