using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Library_Management_System
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // załadowanie bazy danych przy starcie aplikacji
            base.OnStartup(e);
            using (var context = new LibraryContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
