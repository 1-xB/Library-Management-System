using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Entity
{
    internal class Books
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }

        [NotMapped]
        public string Display => $"{Id} {Title} by {Author}";


    }
}
