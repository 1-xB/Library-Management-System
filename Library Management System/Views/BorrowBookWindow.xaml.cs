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

namespace Library_Management_System.Views
{
    /// <summary>
    /// Interaction logic for BorrowBookWindow.xaml
    /// </summary>
    public partial class BorrowBookWindow : Window
    {

        private string _searchedClient;
        private string _searchedBook;
        private string _selectedClient;
        private string _selectedBook;
        public BorrowBookWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            LoadClients();
            LoadBooks();
        }

        private void LoadClients()
        {
            using (var db = new LibraryContext())
            {
                var clients = db.Clients.ToList();
                ClientsListBox.Items.Clear();
                for (int i = 0; i < clients.Count; i++)
                {
                    var client = clients[i];
                    ClientsListBox.Items.Add(client.FullName);
                }

            }
        }

        private void LoadBooks()
        {
            using (var db = new LibraryContext())
            {
                var books = db.Books.ToList();
                var notAvailableBooks = db.Loan.AsEnumerable().Where(l => l.IsReturned == false).Select(l => l.book).ToList();
                books = books.Except(notAvailableBooks).ToList();
                BooksListBox.Items.Clear();
                for (int i = 0; i < books.Count; i++)
                {
                    var book = books[i];
                    BooksListBox.Items.Add(book.Display);
                }
            }

        }

        private void OnClientsSearchChanged(object sender, TextChangedEventArgs e)
        {
            using (var db = new LibraryContext())
            {
                _searchedClient = SearchClientTextBox.Text.ToLower();
                var clients = db.Clients.AsEnumerable().Where(c => c.FullName.ToLower().Contains(_searchedClient)).ToList();
                ClientsListBox.Items.Clear();
                for (int i = 0; i < clients.Count; i++)
                {
                    var client = clients[i];
                    ClientsListBox.Items.Add(client.FullName);
                }

            }
        }

        private void OnBooksSearchChanged(object sender, TextChangedEventArgs e)
        {
            using (var db = new LibraryContext())
            {
                _searchedBook = SearchBookTextBox.Text.ToLower();
                var books = db.Books.AsEnumerable().Where(b => b.Display.ToLower().Contains(_searchedBook)).ToList();
                var notAvailableBooks = db.Loan.AsEnumerable().Where(l => l.IsReturned == false).Select(l => l.book).ToList();
                books = books.Except(notAvailableBooks).ToList();
                BooksListBox.Items.Clear();
                for (int i = 0; i < books.Count; i++)
                {
                    var book = books[i];
                    BooksListBox.Items.Add(book.Display);
                }
            }
        }



        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
        {
            if (_selectedClient == null || _selectedBook == null)
            {
                MessageBox.Show("Please select client and book");
                return;
            }

            using (var db = new LibraryContext())
            {
                var client = db.Clients.AsEnumerable().FirstOrDefault(c => c.FullName == _selectedClient);
                var book = db.Books.AsEnumerable().FirstOrDefault(b => b.Display == _selectedBook);
                var clientLoans = db.Loan.AsEnumerable().Where(l => l.ClientId == client.Id && l.IsReturned == false).ToList();
                if (clientLoans.Count >= 3)
                {
                    MessageBox.Show("Client already has 3 books borrowed");
                    return;
                }
                List<Loan> loans = db.Loan.AsEnumerable().Where(l => l.BookId == book.Id).ToList();
                var loan = new Loan
                {
                    client = client,
                    book = book,
                    LoanDate = DateTime.Now,
                    ReturnDate = DateTime.Now.AddDays(14)
                };
                db.Loan.Add(loan);
                db.SaveChanges();
            }
            MessageBox.Show("Book borrowed successfully");
            Close();
        }

        private void SelectBookButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedBook = BooksListBox.SelectedItem.ToString();
            SelectedBookTextBlock.Text = "Selected book: " + _selectedBook;
        }

        private void SelectClientButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedClient = ClientsListBox.SelectedItem.ToString();
            SelectedClientTextBlock.Text = "Selected client: " + _selectedClient;
        }
    }
}
