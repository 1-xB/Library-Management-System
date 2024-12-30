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
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        private Books _book;
        public EditBookWindow(Books book)
        {
            InitializeComponent();
            _book = book;
            LoadBook();
        }

        private void LoadBook()
        {
            if (_book != null)
            {
                TitleTextBox.Text = _book.Title ?? string.Empty;
                AuthorTextBox.Text = _book.Author ?? string.Empty;
            }
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TitleTextBox.Text) || string.IsNullOrEmpty(AuthorTextBox.Text))
            {
                MessageBox.Show("Please enter all the fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (var db = new LibraryContext())
            {
                _book.Title = TitleTextBox.Text;
                _book.Author = AuthorTextBox.Text;
                db.Books.Update(_book);
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
