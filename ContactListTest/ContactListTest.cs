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
