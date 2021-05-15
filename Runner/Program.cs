using DataLayer;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;

namespace Runner
{
    internal class Program
    {
        private static IConfigurationRoot config;

        private static IContactRepository CreateRepository()
        {
            //return new ContactRepository(config.GetConnectionString("DefaultConnection"));
            return new ContactRepositoryContrib(config.GetConnectionString("DefaultConnection"));
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

        private static void Find_shoud_retraive_existing_entity(int id)
        {
            var repository = CreateRepository();

            var contract = repository.Find(id);

            Console.WriteLine("****Get Contract****");
            contract.Output();
            Debug.Assert(contract.FirstName == "Robert");
            Debug.Assert(contract.LastName == "K");
        }

        private static void Get_All_Should_Be_6()
        {
            var repository = CreateRepository();

            var contacts = repository.GetAll();

            Console.WriteLine($"Count {contacts.Count}");

            Debug.Assert(contacts.Count == 13);
            contacts.Output();
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

            repository.Add(contract);

            Debug.Assert(contract.Id != 0);
            Console.WriteLine("***********Contract Inserted***********");
            Console.WriteLine($"new id is {contract.Id}");
            return contract.Id;
        }

        private static void Main(string[] args)
        {
            Initialise();
            Get_All_Should_Be_6();
            var id = Insert_should_assign_identity_to_new_entity();
            Find_shoud_retraive_existing_entity(id);
            Modified_Should_Update_Entity(id);
            Delete_Entity(id);
        }

        private static void Modified_Should_Update_Entity(int id)
        {
            var repository = CreateRepository();

            var contact = repository.Find(id);
            contact.LastName = "Kruz";
            repository.Update(contact);

            var repository2 = CreateRepository();
            var modified = repository2.Find(id);

            Console.WriteLine("***********Contract Modified***********");
            modified.Output();
            Debug.Assert(modified.LastName == "Kruz");
        }
    }
}