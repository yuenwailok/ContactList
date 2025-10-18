using ContactList.Controllers;
using ContactList.Models;
using ContactList.Repositories;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework.Legacy;
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
            for (int i = 1; i < 4;i++)
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
        public async Task AddControllerTest()
        {
            var contact = new Contact
            {
                ContactID = 1,
                Address = "Test Street " + 1,
                Email = "testest@gmail.com",
                PhoneNumber = "444444" + 1,
                FirstName = "Test",
                LastName = "Test",
                Notes = "Hello World"
            };
            int fakeId = 1;
            var mock = new Mock<IContactRepository>();
            var controller = new ContactsController(mock.Object);
            var addcontacts = await controller.AddAsync(contact);
            mock.Verify(r => r.AddAsync(contact));
            var result = ((CreatedAtActionResult)addcontacts);
            int status_code = 201;
            Assert.That(result.StatusCode, Is.EqualTo(status_code));
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




    }
}
