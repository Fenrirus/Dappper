using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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

        public List<Contact> GetById(params int[] ids)
        {
            return db.Query<Contact>("Select * from Contacts Where Id in @Ids;", new { ids = ids }).ToList();
        }
    }
}