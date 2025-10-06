using ContactList.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace ContactList.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly IConfiguration _configuration;

        public ContactRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            var cs = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(cs);
        }

        async Task<List<Contact>> IContactRepository.GetAllAsync()
        {
            using var connection = GetConnection();
            var contacts = await connection.QueryAsync<Contact>("SELECT * FROM Contacts");
            return contacts.ToList();
        }

        async Task<Contact> IContactRepository.GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            var contact = await connection.
                QueryFirstOrDefaultAsync<Contact>("SELECT * FROM Contacts WHERE ContactID = @Id", new { Id = id });
            return contact;
        }

        async Task IContactRepository.AddAsync(Contact contact)
        {
            using var connection = GetConnection();
            string query = "INSERT INTO Contacts VALUES " +
            "(@FirstName,@LastName,@PhoneNumber,@Email,@Address,@Notes);" +
            "SELECT CAST(SCOPE_IDENTITY() as int);";

            var result = await connection.QuerySingleAsync<int>(query, contact);

            int newId = result;

            contact.ContactID = newId;

        }

        async Task IContactRepository.UpdateAsync(Contact contact)
        {
            using var connection = GetConnection();
            await connection.
                ExecuteAsync("UPDATE Contacts SET FirstName = @FirstName,LastName = @LastName,PhoneNumber = @PhoneNumber,Email = @Email,Address = @Address,Notes = Notes WHERE ContactId = @ContactId", contact);
        }

        async Task IContactRepository.DeleteAsync(int id)
        {
            using var connection = GetConnection();
            await connection.ExecuteAsync("DELETE FROM Contacts WHERE ContactID = @Id", new { Id = id });
        }
    }
}
