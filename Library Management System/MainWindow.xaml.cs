using Library_Management_System.Entity;
using Library_Management_System.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
        private string SearchFullName { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClientsClick(object sender, RoutedEventArgs e)
        {
            LoadClients();
        }

        private void OnBooksClick(object sender, RoutedEventArgs e)
        {
            LoadAllBooks();
        }

        private async Task<List<Books>> GetBooksAsync()
        {
            using (var context = new LibraryContext())
            {
                return await context.Books.ToListAsync();
            }
        }
        private async void LoadAllBooks()
        {
            ClientsGrid.Visibility = Visibility.Collapsed;
            BooksGrid.Visibility = Visibility.Collapsed;
            LoadingGrid.Visibility = Visibility.Visible;

            AllBooksListBox.Items.Clear();
            // Załadowanie książek z bazy danych asynchronicznie
            var books = await GetBooksAsync();

            LoadingGrid.Visibility = Visibility.Collapsed;
            BooksGrid.Visibility = Visibility.Visible;

            if (!books.Any())
            {
                AllBooksListBox.Items.Add("No books in the library");
                return;
            }
            foreach (var book in books)
            {
                AllBooksListBox.Items.Add(book.Display);
            }
        }

        private async Task<List<Clients>> GetClientsAsync()
        {
            using (var context = new LibraryContext())
            {
                return await context.Clients.ToListAsync();
            }
        }
        private async void LoadClients()
        {
            if (SearchTextBox.Text.Length > 0)
            {
                SearchClientsByName();
                return;
            }
            BooksGrid.Visibility = Visibility.Collapsed;
            ClientsGrid.Visibility = Visibility.Collapsed;
            LoadingGrid.Visibility = Visibility.Visible;

            ClientsListBox.Items.Clear();

            // Załadowanie klientów z bazy danych asynchronicznie
            var clients = await GetClientsAsync();

            LoadingGrid.Visibility = Visibility.Collapsed;
            ClientsGrid.Visibility = Visibility.Visible;

            foreach (var client in clients)
            {
                ClientsListBox.Items.Add(client.FullName);
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

        private void OnEditBookClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var context = new LibraryContext())
                {
                    if (AllBooksListBox.SelectedItem == null) return;
                    string selectedBook = AllBooksListBox.SelectedItem.ToString();
                    var book = FindBook(selectedBook);
                    if (book == null) return;
                    EditBookWindow editBookWindow = new EditBookWindow(book);
                    editBookWindow.ShowDialog();
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
                    if (loan.IsReturned) continue;
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

        private void OnClientsSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            SearchClientsByName();
        }

        private async void SearchClientsByName()
        {
            using (var context = new LibraryContext())
            {
                SearchFullName = SearchTextBox.Text.ToLower(); // Zmiana na małe litery dla ułatwienia wyszukiwania
                var clients = await context.Clients
                                           .Where(c => c.FullName.ToLower().Contains(SearchFullName))
                                           .ToListAsync();
                ClientsListBox.Items.Clear();
                AddressListBox.Items.Clear();
                BorrowedBooksListBox.Items.Clear();
                foreach (var client in clients)
                {
                    ClientsListBox.Items.Add(client.FullName);
                }
            }
        }

        private void OnBorrowClick(object sender, RoutedEventArgs e)
        {
            BorrowBookWindow borrowBookWindow = new BorrowBookWindow();
            borrowBookWindow.ShowDialog();
        }

        private void OnReturnButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (BorrowedBooksListBox.SelectedItem == null) return;
                string[] borrowedBook = BorrowedBooksListBox.SelectedItem.ToString().Split(' ');

                int lastByIndex = Array.LastIndexOf(borrowedBook, "by");
                borrowedBook = borrowedBook.Take(lastByIndex).ToArray(); // Take - usuwa elementy od podanego indeksu do końca tablicy
                string title = string.Join(" ", borrowedBook);

                using (var context = new LibraryContext())
                {
                    var book = context.Books.FirstOrDefault(b => b.Title == title);
                    if (book == null) return;
                    var loan = context.Loan.FirstOrDefault(l => l.BookId == book.Id && l.IsReturned == false);
                    if (loan == null) return;
                    loan.IsReturned = true;
                    MessageBox.Show("Book returned " + book.Display);
                    context.Loan.Update(loan);
                    context.SaveChanges();
                    LoadClientDetails(ClientsListBox.SelectedItem.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
