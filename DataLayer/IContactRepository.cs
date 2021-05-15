using System.Collections.Generic;

namespace DataLayer
{
    public interface IContactRepository
    {
        Contact Add(Contact contact);

        Contact Find(int id);

        Contact FullContact(int id);

        List<Contact> GetAll();

        void Remove(int id);

        void Save(Contact contact);

        Contact Update(Contact contact);
    }
}