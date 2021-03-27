using System.Collections.Generic;

namespace DataLayer
{
    public interface IContactRepository
    {
        Contact Add(Contact contact);

        Contact Find(int id);

        List<Contact> GetAll();

        void Remove(int id);

        Contact Update(Contact contact);
    }
}