﻿using System;
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
            LibraryContext context = new LibraryContext();
            InitializeComponent();
        }

        private void OnClientsClick(object sender, RoutedEventArgs e)
        {
            BooksGrid.Visibility = Visibility.Collapsed;
            ClientsGrid.Visibility = Visibility.Visible;
        }

        private void OnBooksClick(object sender, RoutedEventArgs e)
        {
            ClientsGrid.Visibility = Visibility.Collapsed;
            BooksGrid.Visibility = Visibility.Visible;
        }
    }
}
