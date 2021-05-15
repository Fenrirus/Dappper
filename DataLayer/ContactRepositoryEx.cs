using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace DataLayer
{
    public class ContactRepositoryEx
    {
        private readonly IDbConnection db;

        public ContactRepositoryEx(string connString)
        {
            db = new SqlConnection(connString);
        }

        public int BulkInsertContacts(List<Contact> contacts)
        {
            var sql =
                "INSERT INTO Contacts (FirstName, LastName, Email, Company, Title) VALUES(@FirstName, @LastName, @Email, @Company, @Title); " +
                "SELECT CAST(SCOPE_IDENTITY() as int)";
            return this.db.Execute(sql, contacts);
        }

        public List<dynamic> DynymicGetById(params int[] ids)
        {
            return db.Query("Select * from Contacts Where Id in @Ids;", new { ids = ids }).ToList();
        }

        public async Task<List<Contact>> GetAllAsync()
        {
            var contact = await db.QueryAsync<Contact>("Select * from Contacts");
            return contact.ToList();
        }

        public List<Contact> GetAllContactsWithAddresses()
        {
            var sql = "SELECT * FROM Contacts AS C INNER JOIN Addresses AS A ON A.ContactId = C.Id";

            var contactDict = new Dictionary<int, Contact>();

            var contacts = this.db.Query<Contact, Address, Contact>(sql, (contact, address) =>
            {
                if (!contactDict.TryGetValue(contact.Id, out var currentContact))
                {
                    currentContact = contact;
                    contactDict.Add(currentContact.Id, currentContact);
                }

                currentContact.Addresses.Add(address);
                return currentContact;
            });
            return contacts.Distinct().ToList();
        }

        public List<Contact> GetById(params int[] ids)
        {
            return db.Query<Contact>("Select * from Contacts Where Id in @Ids;", new { ids = ids }).ToList();
        }
    }
}