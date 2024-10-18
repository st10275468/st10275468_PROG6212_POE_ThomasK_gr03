using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using st10275468_PROG6212_POE_ThomasK_gr03.Controllers;
using st10275468_PROG6212_POE_ThomasK_gr03.Data;
using st10275468_PROG6212_POE_ThomasK_gr03.Models;

namespace st10275468_PROG6212_POE_ThomasK_gr03_Tests.Controller_Unit_Tests
{
    public class UserControllerTests
    {
        private readonly UserController _controller;
        private readonly ContractManagementContext _context;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<ContractManagementContext>()
                .UseInMemoryDatabase(databaseName: "TestingDatabase")
                .Options;

            _context = new ContractManagementContext(options);
            _controller = new UserController(_context);
        }

    }
}
