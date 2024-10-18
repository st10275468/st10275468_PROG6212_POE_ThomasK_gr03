/*  OpenAI.2024. Chat-GPT(Version 3.5).[Large language model]. Available at: https://chat.openai.com/[Accessed: 17 October 2024].
 *  Microsoft. (n.d.). Session Management in ASP.NET Core. Available at: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0#session [Accessed: 17 October 2024].
 Microsoft. (n.d.). Testing ASP.NET Core Services in Multi-Container Microservice .NET Applications. Available at: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps [Accessed: 17 October 2024].*/


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Infrastructure;
using Microsoft.EntityFrameworkCore;
using st10275468_PROG6212_POE_ThomasK_gr03.Controllers;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using Moq;


namespace st10275468_PROG6212_POE_ThomasK_gr03_TestingProject.Controller_tests
{
    [TestClass]
    public class UserControllerTests
    {
        private ContractManagementContext _context;
        private UserController _controller;

        /// <summary>
        /// Setting up the testing environment so that the tests work
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ContractManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ContractManagementContext(options);
            _controller = new UserController(_context);

            var context = new DefaultHttpContext();
            var session = new Mock<ISession>();
            context.Session = session.Object;
            _controller.ControllerContext.HttpContext = context;

            var tempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted(); 
        }


        /// <summary>
        /// Test method created to test the user Registration method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Register_ValidUser_RedirectsToIndex()
        {
            var user = new User
            {
                userID = 1,
                email = "John@gmail.com",
                password = "12345",
                name = "John",
                surname = "Dale",
                role = "Lecturer"
            };

            var result = await _controller.Register(user) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Account created successfully.", _controller.TempData["SuccessMessage"]);

            var addedUser = await _context.Users.FindAsync(user.userID);
            Assert.IsNotNull(addedUser);
        
        }


        /// <summary>
        /// Test method created to test the user login method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Login_ValidUser_RedirectsToIndex()
        {
            var user = new User
            {
                userID = 1,
                email = "JohnD@gmail.com",
                password = "12345",
                name = "John",
                surname = "Dale",
                role = "Lecturer"
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var loginUser = new User
            {
                email = "JohnD@gmail.com",
                password = "12345"
            };
            var result = await _controller.Login(loginUser) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Welcome back John!", _controller.TempData["SuccessMessage"].ToString().Trim());
        }
    }
    }
