using Xunit;
using CloudStorage.Helpers;

namespace CloudStorage.Tests.Helpers
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("hello")]
        [InlineData("TestControler")]
        [InlineData("Test2Controller")]
        [InlineData("M")]
        [InlineData("TestController2")]
        [InlineData("Etc")]
        [InlineData("Controller")]
        public void GetControllerName(string testData)
        {
            const string controllerSuffix = "Controller";
            if (testData.EndsWith(controllerSuffix))
            {
                var result = testData.GetControllerName();
                Assert.Equal(testData.Length - controllerSuffix.Length, result.Length);
                Assert.False(result.Contains(controllerSuffix));
            }
            else
            {
                var result = testData.GetControllerName();
                Assert.Equal(string.Empty, result);
            }
        }

        [Fact]
        public void IsNullOrWhiteSpaceTest()
        {
            Assert.False("hello kitty".IsNullOrWhiteSpace());
        }

        [Fact]
        public void IsNullOrWhiteSpaceEmptyTest()
        {
            Assert.True("            ".IsNullOrWhiteSpace());
        }
    }
}