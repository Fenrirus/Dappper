using DataLayer;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Runner
{
    internal class Program
    {
        private static IConfigurationRoot config;

        private static async Task aMain(string[] args)
        {
            Initialise();
            await Get_All_Async_Should_Be_6();
        }

        private static void Bulk_insert_should_insert_4_rows()
        {
            // arrange
            var repository = CreateRepositoryEx();
            var contacts = new List<Contact>
            {
                new Contact { FirstName = "Charles", LastName = "Barkley" },
                new Contact { FirstName = "Scottie", LastName = "Pippen" },
                new Contact { FirstName = "Tim", LastName = "Duncan" },
                new Contact { FirstName = "Patrick", LastName = "Ewing" }
            };

            // act
            var rowsAffected = repository.BulkInsertContacts(contacts);

            // assert
            Console.WriteLine($"Rows inserted: {rowsAffected}");
            Debug.Assert(rowsAffected == 4);
        }

        private static IContactRepository CreateRepository()
        {
            //return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            //return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));

            return new ContactRepositorySP(config.GetConnectionString("DefaultConnection"));
        }

        private static ContactRepositoryEx CreateRepositoryEx()
        {
            return new ContactRepositoryEx(config.GetConnectionString("DefaultConnection"));
        }

        private static void Delete_Entity(int id)
        {
            var repository = CreateRepository();

            repository.Remove(id);

            var repository2 = CreateRepository();
            var deleted = repository2.Find(id);

            Debug.Assert(deleted == null);

            Console.WriteLine("***********Contract Deleted***********");
        }

        private static void DynymicSupport()
        {
            var repository = CreateRepositoryEx();

            var contacts = repository.DynymicGetById(1, 2, 3);

            Debug.Assert(contacts.Count == 3);
            contacts.Output();
        }

        private static void Find_shoud_retraive_existing_entity(int id)
        {
            var repository = CreateRepository();

            var contract = repository.FullContact(id);

            Console.WriteLine("****Get Contract****");
            contract.Output();
            Debug.Assert(contract.FirstName == "Robert");
            Debug.Assert(contract.LastName == "K");
        }

        private static async Task Get_All_Async_Should_Be_6()
        {
            var repository = CreateRepositoryEx();

            var contacts = await repository.GetAllAsync();

            Console.WriteLine($"Count {contacts.Count}");

            Debug.Assert(contacts.Count == 6);
            contacts.Output();
        }

        private static void Get_All_Should_Be_6()
        {
            var repository = CreateRepository();

            var contacts = repository.GetAll();

            Console.WriteLine($"Count {contacts.Count}");

            Debug.Assert(contacts.Count == 6);
            contacts.Output();
        }

        private static void Get_all_should_return_6_results_with_addresses()
        {
            var repository = CreateRepositoryEx();

            // act
            var contacts = repository.GetAllContactsWithAddresses();

            // assert
            Console.WriteLine($"Count: {contacts.Count}");
            contacts.Output();
            Debug.Assert(contacts.Count == 6);
            Debug.Assert(contacts.First().Addresses.Count == 2);
        }

        private static void Initialise()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetings.json", optional: true, reloadOnChange: true);
            config = builder.Build();
        }

        private static int Insert_should_assign_identity_to_new_entity()
        {
            var repository = CreateRepository();

            var contract = new Contact
            {
                FirstName = "Robert",
                LastName = "K",
                Email = "robertk@gmail.com",
                Company = "Klimor",
                Title = "Developer",
            };

            var address = new Address
            {
                AddressType = "Dom",
                StreetAddress = "Kaukazka",
                City = "Gdańsk",
                StateId = 1,
                PostalCode = "09200",
            };

            contract.Addresses.Add(address);
            //repository.Add(contract);
            repository.Save(contract);

            Debug.Assert(contract.Id != 0);
            Console.WriteLine("***********Contract Inserted***********");
            Console.WriteLine($"new id is {contract.Id}");
            return contract.Id;
        }

        private static void ListSupport()
        {
            var repository = CreateRepositoryEx();

            var contacts = repository.GetById(1, 2, 3);

            Debug.Assert(contacts.Count == 3);
            contacts.Output();
        }

        private static void Main(string[] args)
        {
            Initialise();
            //Get_All_Should_Be_6();
            var id = Insert_should_assign_identity_to_new_entity();
            Find_shoud_retraive_existing_entity(id);
            Modified_Should_Update_Entity(id);
            Delete_Entity(id);

            // var repository = CreateRepository();
            // var mj = repository.FullContact(1);
            // mj.Output();

            ListSupport();
            DynymicSupport();
            Bulk_insert_should_insert_4_rows();
            Get_all_should_return_6_results_with_addresses();
        }

        private static void Modified_Should_Update_Entity(int id)
        {
            var repository = CreateRepository();

            //var contact = repository.Find(id);
            var contact = repository.FullContact(id);
            contact.LastName = "Kruz";
            contact.Addresses[0].City = "Gdynia";
            //repository.Update(contact);
            repository.Save(contact);

            var repository2 = CreateRepository();
            //var modified = repository2.Find(id);
            var modified = repository2.FullContact(id);

            Console.WriteLine("***********Contract Modified***********");
            modified.Output();
            Debug.Assert(modified.LastName == "Kruz");
            Debug.Assert(modified.Addresses[0].City == "Gdynia");
        }
    }
}