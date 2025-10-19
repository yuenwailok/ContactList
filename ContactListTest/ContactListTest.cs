using ContactList.Controllers;
using ContactList.Models;
using ContactList.Repositories;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Legacy;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
namespace ContactListTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetAllControllerTest()
        {
            List<Contact> contacts = new List<Contact>();
            for (int i = 1; i < 4; i++)
            {
                var contact = new Contact
                {
                    ContactID = i,
                    Address = "Test Street " + i,
                    Email = "testest@gmail.com",
                    PhoneNumber = "444444" + i,
                    FirstName = "Test",
                    LastName = "Test",
                    Notes = "Hello World"
                };
                contacts.Add(contact);
            }
            var mock = new Mock<IContactRepository>();
            mock.Setup(r => r.GetAllAsync()).Returns(Task.FromResult<List<Contact>>(contacts));
            var controller = new ContactsController(mock.Object);
            var allcontacts = await controller.GetAll();
            var result = ((OkObjectResult)allcontacts.Result).Value;
            mock.Verify(r => r.GetAllAsync());
            Assert.That(contacts, Is.EqualTo(result));
        }

        [Test]
        public async Task GetByIdControllerTestNull()
        {
            int fakeId = 99;
            var mock = new Mock<IContactRepository>();
            var controller = new ContactsController(mock.Object);
            var controller_contact = await controller.GetById(fakeId);
            var result = ((NotFoundObjectResult)controller_contact.Result);
            var result_value = result.Value;
            mock.Verify(r => r.GetByIdAsync(fakeId));
            int status_code = 404;
            Assert.That(result.StatusCode, Is.EqualTo(status_code));
        }

        [Test]
        public async Task GetByIdControllerTest()
        {
            int fakeId = 1;
            var contact = new Contact
            {
                ContactID = fakeId,
                Address = "Dest Street " + fakeId,
                Email = "Destest@gmail.com",
                PhoneNumber = "444444" + fakeId,
                FirstName = "Dest",
                LastName = "Dest",
                Notes = "Hello World"
            };
            var mock = new Mock<IContactRepository>();
            mock.Setup(r => r.GetByIdAsync(fakeId)).Returns(Task.FromResult<Contact>(contact));

            var controller = new ContactsController(mock.Object);
            var controller_contact = await controller.GetById(fakeId);
            var result = ((OkObjectResult)controller_contact.Result).Value;
            mock.Verify(r => r.GetByIdAsync(fakeId));
            Assert.That(contact, Is.EqualTo(result));
        }

        [Test]
        public async Task AddControllerTest()
        {
            var contact = new Contact
            {
                Address = "Test Street " + 1,
                Email = "testest@gmail.com",
                PhoneNumber = "444444" + 1,
                FirstName = "TestAdd",
                LastName = "Test",
                Notes = "Hello World"
            };

            int fakeId = 1;
            var mock = new Mock<IContactRepository>();
            var controller = new ContactsController(mock.Object);
            var addcontacts = await controller.AddAsync(contact);
            mock.Verify(r => r.AddAsync(contact));
            var result = ((CreatedAtActionResult)addcontacts);
            var value = result.Value;
            int status_code = 201;

            Assert.That(value, Is.EqualTo(contact));
            Assert.That(result.StatusCode, Is.EqualTo(status_code));
        }

        [Test]
        public async Task UpdateControllerTest()
        {
            int wrongId = 999;
            int realId = 1;
            var contact = new Contact
            {
                ContactID = realId,
                Address = "Test Street " + 1,
                Email = "testest@gmail.com",
                PhoneNumber = "444444" + 1,
                FirstName = "TestAdd",
                LastName = "Test",
                Notes = "Hello World"
            };
            var mock = new Mock<IContactRepository>();
            var controller = new ContactsController(mock.Object);
            mock.Setup(r => r.GetByIdAsync(realId)).Returns(Task.FromResult<Contact>(contact));

            var updatecontactsnull = await controller.Update(wrongId, contact);
            var nullresult = ((NotFoundObjectResult)updatecontactsnull);
            var status_code_null = 404;
            mock.Verify(r => r.GetByIdAsync(wrongId));

            Assert.That(status_code_null, Is.EqualTo(nullresult.StatusCode));

            var updatecontacts = await controller.Update(realId, contact);
            var result = ((NoContentResult)updatecontacts);
            var status_code = 204;
            mock.Verify(r => r.GetByIdAsync(realId));
            mock.Verify(r => r.UpdateAsync(contact));

            Assert.That(status_code, Is.EqualTo(result.StatusCode));


        }

        [Test]
        public async Task DeleteControllerTest()
        {
            int wrongId = 999;
            int realId = 1;
            var contact = new Contact
            {
                ContactID = realId,
                Address = "Test Street " + 1,
                Email = "testest@gmail.com",
                PhoneNumber = "444444" + 1,
                FirstName = "TestAdd",
                LastName = "Test",
                Notes = "Hello World"
            };
            var mock = new Mock<IContactRepository>();
            var controller = new ContactsController(mock.Object);
            mock.Setup(r => r.GetByIdAsync(realId)).Returns(Task.FromResult<Contact>(contact));

            var deletecontactsnull = await controller.Delete(wrongId);
            var nullresult = ((NotFoundObjectResult)deletecontactsnull);
            var status_code_null = 404;
            mock.Verify(r => r.GetByIdAsync(wrongId));

            Assert.That(status_code_null, Is.EqualTo(nullresult.StatusCode));

            var deletecontacts = await controller.Delete(realId);
            var result = ((NoContentResult)deletecontacts);
            var status_code = 204;
            mock.Verify(r => r.GetByIdAsync(realId));
            mock.Verify(r => r.DeleteAsync(realId));

            Assert.That(status_code, Is.EqualTo(result.StatusCode));



        }
    }
}
