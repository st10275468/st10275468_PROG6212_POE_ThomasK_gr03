/*  OpenAI.2024. Chat-GPT(Version 3.5).[Large language model]. Available at: https://chat.openai.com/[Accessed: 17 October 2024].
 *  Microsoft. (n.d.). Session Management in ASP.NET Core. Available at: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-7.0#session [Accessed: 17 October 2024].
 Microsoft. (n.d.). Testing ASP.NET Core Services in Multi-Container Microservice .NET Applications. Available at: https://learn.microsoft.com/en-us/dotnet/architecture/microservices/multi-container-microservice-net-applications/test-aspnet-core-services-web-apps [Accessed: 17 October 2024].*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Moq;
using st10275468_PROG6212_POE_ThomasK_gr03.Controllers;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace st10275468_PROG6212_POE_ThomasK_gr03_TestingProject.Controller_tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private ContractManagementContext _context;
        private HomeController _controller;

        /// <summary>
        /// Setting up the testing environment so that the tests will work
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ContractManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ContractManagementContext(options);
            _controller = new HomeController(_context);

            var context = new DefaultHttpContext();
            var session = new Mock<ISession>();
            var userID = 1;

            session.Setup(s => s.TryGetValue("userID", out It.Ref<byte[]>.IsAny)).Returns((string key, out byte[] value) =>
            {
                value = BitConverter.GetBytes(userID);
                return true;
            });

            session.Setup(s => s.Set("userID", It.IsAny<byte[]>())).Callback<string, byte[]>((key, value) => { });

            context.Session = session.Object;

            var tempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
            _controller.ControllerContext = new ControllerContext { HttpContext = context };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
        }

        /// <summary>
        /// Test method created to test the approve claim method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ApproveClaim_Valid_UpdatesClaimStatus()
        {
            
            var claim = new Claim
            {
                claimID = 1,
                claimStatus = "Pending",
                userID = 1
            };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

           
            var result = _controller.ApproveClaim(claim.claimID) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Privacy", result.ActionName);

            var updatedClaim = await _context.Claims.FindAsync(claim.claimID);
            Assert.IsNotNull(updatedClaim);
            Assert.AreEqual("Approved", updatedClaim.claimStatus);
        }


        /// <summary>
        /// Test method created to test the Deny claim method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DenyClaim_Valid_UpdatesClaimStatus()
        {
           var claim = new Claim
            {
                claimID = 1,
                claimStatus = "Pending",
                userID = 1
            };
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();

            var result = _controller.DenyClaim(claim.claimID) as RedirectToActionResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Privacy", result.ActionName);

            var updatedClaim = await _context.Claims.FindAsync(claim.claimID);
            Assert.IsNotNull(updatedClaim);
            Assert.AreEqual("Denied", updatedClaim.claimStatus);
        }

        /// <summary>
        /// Test method created to test the Download document method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task DownloadDocument_Valid_ReturnsFileDownload()
        {
             var document = new Document
            {
                documentID = 1,
                path = "document.txt"
            };

            var documentsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Documents");
            Directory.CreateDirectory(documentsDirectory);

            var filePath = Path.Combine(documentsDirectory, document.path);
            await File.WriteAllTextAsync(filePath, "Test document.");

            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            var result = _controller.DownloadDocument(document.documentID) as FileResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("application/octet-stream", result.ContentType);
            Assert.AreEqual(document.path, result.FileDownloadName);

            File.Delete(filePath);
        }

        /// <summary>
        /// Test method created to test the Submit claims method
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task SubmitClaims_UserNotLoggedIn_RedirectsToIndex()
        {
            
            var context = new DefaultHttpContext();
            var session = new Mock<ISession>();
            context.Session = session.Object;
            _controller.ControllerContext.HttpContext = context;

            var result = _controller.SubmitClaims() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("You must be logged in to access this page.", _controller.TempData["ErrorMessage"]);
        }


        /// <summary>
        /// Test method created to test if the user is logged in
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task Privacy_UserNotLoggedIn_RedirectsToIndex()
        {
            var context = new DefaultHttpContext();
            var session = new Mock<ISession>();
            context.Session = session.Object;
            _controller.ControllerContext.HttpContext = context;

            var result = await _controller.Privacy() as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("You must be logged in to access this page.", _controller.TempData["ErrorMessage"]);
        }

        [TestMethod]
        public void Index_ReturnsViewResult()
        {
            var result = _controller.Index();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        

    }
}
