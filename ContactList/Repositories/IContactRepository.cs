using ContactList.Models;

namespace ContactList.Repositories
{
    public interface IContactRepository
    {
        public Task<List<Contact>> GetAllAsync();

        public Task<Contact> GetByIdAsync(int id);

        public Task AddAsync(Contact contact);

        public Task UpdateAsync(Contact contact);

        public Task DeleteAsync(int id);
    }
}
