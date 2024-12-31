using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Entity
{
    public class Loan
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int BookId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public bool IsReturned { get; set; }

        public Clients client { get; set; }
        public Books book { get; set; }


    }
}
