using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Entity
{
    public class Clients
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Phone { get; set; }
        public int AddressId { get; set; }

        public Address Address { get; set; }
        public ICollection<Loan> Loans { get; set; }


        [NotMapped]
        public string FullName => $"{Id} {Name} {Surname}";
        [NotMapped]
        public string FullNameWithoutId => $"{Name} {Surname}";

        

    }
}
