using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Moq;
using st10275468_PROG6212_POE_ThomasK_gr03.Controllers;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace st10275468_PROG6212_POE_ThomasK_gr03_TestingProject.Controller_tests
{
    [TestClass]
    public class ClaimControllerTests
    {
        private ContractManagementContext _context;
        private ClaimController _controller;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ContractManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ContractManagementContext(options);
            _controller = new ClaimController(_context);

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
            _controller.ControllerContext.HttpContext = context;

            var tempData = new TempDataDictionary(context, Mock.Of<ITempDataProvider>());
            _controller.TempData = tempData;
        }


        [TestMethod]
        public async Task SubmitClaim_ValidClaim_RedirectsToSubmitClaims()
        {
            var claimMonth = DateTime.Now;
            var hoursWorked = 30;
            var hourlyRate = 50;

            var supportingDocumentMock = new Mock<IFormFile>();
            supportingDocumentMock.Setup(file => file.Length).Returns(1);
            supportingDocumentMock.Setup(file => file.FileName).Returns("file.txt");
            var formFiles = new FormFileCollection { supportingDocumentMock.Object };

            var result = await _controller.SubmitClaim(claimMonth, hoursWorked, hourlyRate, formFiles) as RedirectToActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("SubmitClaims", result.ActionName);
            Assert.AreEqual("Your claim has been submitted successfully", _controller.TempData["SuccessMessage"]);
        }



    }
}
