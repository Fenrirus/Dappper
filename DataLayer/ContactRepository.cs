using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace DataLayer
{
    public class ContactRepository : IContactRepository
    {
        private readonly IDbConnection db;

        public ContactRepository(string connString)
        {
            db = new SqlConnection(connString);
        }

        public Contact Add(Contact contact)
        {
            var sql = "INSERT INTO Contacts (FirstName, LastName, Email, Company, Title) VALUES (@FirstName, @LastName, @Email, @Company, @Title);"
                + "Select cast(SCOPE_IDENTITY() as int)";
            var id = db.Query<int>(sql, contact).Single();
            contact.Id = id;
            return contact;
        }

        public Address Add(Address address)
        {
            var sql = "INSERT INTO Addresses (ContactId,AddressType,StreetAddress,City,StateId,PostalCode) VALUES (@ContactId,@AddressType,@StreetAddress,@City,@StateId,@PostalCode);"
                + "Select cast(SCOPE_IDENTITY() as int)";
            var id = db.Query<int>(sql, address).Single();
            address.Id = id;
            return address;
        }

        public Contact Find(int id)
        {
            return db.Query<Contact>("Select * from Contacts where id = @id", new { id }).SingleOrDefault();
        }

        public Contact FullContact(int id)
        {
            var sql = "Select * from Contacts Where Id = @Id;" +
                "Select * from Addresses Where ContactId = @Id";
            using (var multiple = db.QueryMultiple(sql, new { Id = id }))
            {
                var contact = multiple.Read<Contact>().SingleOrDefault();

                var adresses = multiple.Read<Address>().ToList();

                if (contact != null && adresses != null)
                {
                    contact.Addresses.AddRange(adresses);
                }

                return contact;
            }
        }

        public List<Contact> GetAll()
        {
            return db.Query<Contact>("Select * from Contacts").ToList();
        }

        public void Remove(int id)
        {
            db.Execute("DELETE FROM Contacts WHERE Id = @Id", new { id });
        }

        public void Save(Contact contact)
        {
            using var txscope = new TransactionScope();
            if (contact.IsNew)
            {
                Add(contact);
            }
            else
            {
                Update(contact);
            }

            foreach (var addr in contact.Addresses.Where(w => !w.IsDeleted))
            {
                addr.ContactId = contact.Id;
                if (addr.IsNew)
                {
                    Add(addr);
                }
                else
                {
                    Update(addr);
                }
;
            }

            foreach (var addr in contact.Addresses.Where(w => w.IsDeleted))
            {
                db.Execute("DELETE FROM Addresses WHERE Id = @Id", new { addr.Id });
            }

            txscope.Complete();
        }

        public Contact Update(Contact contact)
        {
            var sql = "UPDATE Contacts " +
            "SET[FirstName] = @FirstName" +
            ",[LastName] = @LastName" +
            ",[Email] = @Email" +
            ",[Company] = @company" +
            ",[Title] = @Title " +
            "Where id = @Id";

            db.Execute(sql, contact);
            return contact;
        }

        public Address Update(Address address)
        {
            var sql = "UPDATE Addresses " +
            "SET[AddressType] = @AddressType" +
            ",[StreetAddress]  = @StreetAddress" +
            ",[City]  = @City" +
            ",[StateId] = @StateId" +
            ",[PostalCode] = @PostalCode " +
            "Where id = @Id";

            db.Execute(sql, address);
            return address;
        }
    }
}