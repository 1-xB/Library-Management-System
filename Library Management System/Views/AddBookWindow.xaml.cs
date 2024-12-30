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
    /// Interaction logic for AddBookWindow.xaml
    /// </summary>
    public partial class AddBookWindow : Window
    {
        public AddBookWindow()
        {
            InitializeComponent();
        }

        private void OnSaveButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TitleTextBox.Text) || string.IsNullOrEmpty(AuthorTextBox.Text))
            {
                MessageBox.Show("Please fill all the fields", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            using (var db = new LibraryContext())
            {
                db.Books.Where(b => b.Title == TitleTextBox.Text && b.Author == AuthorTextBox.Text).FirstOrDefault();
                if (db.Books.Any())
                {
                    MessageBox.Show("Book already exists. Add index after Title, please", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Books book = new Books
                {
                    Title = TitleTextBox.Text,
                    Author = AuthorTextBox.Text
                };
                db.Books.Add(book);
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
