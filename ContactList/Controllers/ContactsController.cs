using ContactList.Models;
using ContactList.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.Resource;
using System.Net;

namespace ContactList.Controllers
{
    [Authorize]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read",
            RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Read"
        )]
        public async Task<ActionResult<List<Contact>>> GetAll()
        {

            var contacts = await _contactRepository.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Read",
            RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Read"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
            RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
        )]
        public async Task<ActionResult> AddAsync(Contact contact)
        {
            var ownerIdOfTodo = IsAppMakingRequest() ? new Guid() : GetUserId();
            contact.Notes = contact.Notes + ownerIdOfTodo.ToString();
            await _contactRepository.AddAsync(contact);
            return CreatedAtAction(nameof(GetById), new {id = contact.ContactID}, contact);
        }

        [HttpPut("{id}")]
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
            RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
        )]
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
        [RequiredScopeOrAppPermission(
            RequiredScopesConfigurationKey = "AzureAD:Scopes:Write",
            RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:Write"
        )]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await _contactRepository.GetByIdAsync(id);
            if (exist == null)
                return NotFound("Contact not found");

            await _contactRepository.DeleteAsync(id);
            return NoContent();
        }

        private bool IsAppMakingRequest()
        {
            if (HttpContext.User.Claims.Any(c => c.Type == "idtyp"))
            {
                return HttpContext.User.Claims.Any(c => c.Type == "idtyp" && c.Value == "app");
            }
            else
            {
                return HttpContext.User.Claims.Any(c => c.Type == "roles") && !HttpContext.User.Claims.Any(c => c.Type == "scp");
            }
        }

        private bool RequestCanAccessToDo(Guid userId)
        {
            return IsAppMakingRequest() || (userId == GetUserId());
        }

        private Guid GetUserId()
        {
            Guid userId;
            if (!Guid.TryParse(HttpContext.User.GetObjectId(), out userId))
            {
                throw new Exception("User ID is not valid.");
            }
            return userId;
        }

    }
}
