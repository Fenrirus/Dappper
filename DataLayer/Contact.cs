using Dapper.Contrib.Extensions;
using System.Collections.Generic;

namespace DataLayer
{
    public class Contact
    {
        [Write(false)]
        public List<Address> Addresses { get; } = new List<Address>();

        public string Company { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public int Id { get; set; }

        [Computed]
        public bool IsNew => this.Id == default(int);

        public string LastName { get; set; }
        public string Title { get; set; }
    }
}