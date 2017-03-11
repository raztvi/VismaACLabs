using CloudStorage.Controllers;
using Xunit;
using NSubstitute;
using CloudStorage.Services;
using CloudStorage.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CloudStorage.Tests.Controllers
{
    public class AboutControllerTests
    {
        private const string GreeterMessage = "Hello from a test";

        private AboutController GetControllerForTesting()
        {
            var mockGreeter = Substitute.For<IGreeter>();
            mockGreeter.GetGreeting().ReturnsForAnyArgs(GreeterMessage);
            var controller = new AboutController(mockGreeter);
            return controller;
        }

        [Fact]
        public void PhoneTest()
        {
            var controller = GetControllerForTesting();
            Assert.Equal("123-456-789", controller.Phone());
        }

        [Fact]
        public void AddressTest()
        {
            var controller = GetControllerForTesting();
            Assert.Equal("Timisoara, Romania", controller.Address());
        }

        [Fact]
        public void TestMeTest()
        {
            var controller = GetControllerForTesting();

            var result = controller.TestMe();

            Assert.NotNull(result);

            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsAssignableFrom<TestingViewModel>(viewResult.ViewData.Model);

            Assert.Equal(GreeterMessage, model.Greeting);
            Assert.Equal(DateTime.UtcNow.Date, model.CurrentDate.Date);
        }
    }
}