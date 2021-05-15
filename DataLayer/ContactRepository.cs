using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

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

        public Contact Find(int id)
        {
            return db.Query<Contact>("Select * from Contacts where id = @id", new { id }).SingleOrDefault();
        }

        public List<Contact> GetAll()
        {
            return db.Query<Contact>("Select * from Contacts").ToList();
        }

        public void Remove(int id)
        {
            db.Execute("DELETE FROM Contacts WHERE Id = @Id", new { id });
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
    }
}