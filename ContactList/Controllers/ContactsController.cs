using ContactList.Models;
using ContactList.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ContactList.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contact>>> GetAll()
        {

            var contacts = await _contactRepository.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> GetById(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound("This contact does not exist");
            }
            return Ok(contact);
        }

        [HttpPost]
        public async Task<ActionResult> AddAsync(Contact contact)
        {
            await _contactRepository.AddAsync(contact);
            return CreatedAtAction(nameof(GetById), new {id = contact.ContactID}, contact);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Contact contact)
        {
            var exist = await _contactRepository.GetByIdAsync(id);
            if (exist == null)
                return NotFound("Contact not found");

            contact.ContactID = id;
            await _contactRepository.UpdateAsync(contact);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await _contactRepository.GetByIdAsync(id);
            if (exist == null)
                return NotFound("Contact not found");

            await _contactRepository.DeleteAsync(id);
            return NoContent();
        }

    }
}
