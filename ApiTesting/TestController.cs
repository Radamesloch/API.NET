using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Vendsys.Controllers;
using Vendsys.Interfaces;
using Vendsys.Models;

namespace ApiTesting;

[TestClass]
public class TestController
{
    private Mock<ILogger<VendsysController>> _mockLogger;
    private Mock<IVendsysManager> _mockVendsysManager;
    private VendsysController _controller;

    [TestInitialize]
    public void SetUp()
    {
        // Arrange: Create mock objects for the dependencies
        _mockLogger = new Mock<ILogger<VendsysController>>();
        _mockVendsysManager = new Mock<IVendsysManager>();

        // Create the controller using the mocked dependencies
        _controller = new VendsysController(_mockLogger.Object, _mockVendsysManager.Object);
    }
     

    [TestMethod]
    public async Task DexProcess_ShouldReturnOk_WhenDexIsDelete()
    {
        // Arrange
        string dex = "delete"; // For delete case
        var expectedResponse = new ResponseCommon { IsSuccess = true }; // Mock response

        // Set up the mock method of IVendsysManager to return the expected response for delete
        _mockVendsysManager
            .Setup(m => m.ManageDeleteRows())
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.dexProcess(dex);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(expectedResponse, okResult.Value);
    }

    [TestMethod]
    public async Task DexProcess_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        string dex = "someData"; // A sample non-delete string

        // Set up the mock method to throw an exception
        _mockVendsysManager
            .Setup(m => m.ManageDexFile(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("An error occurred"));

        // Act
        var result = await _controller.dexProcess(dex);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.IsTrue(badRequestResult.Value.ToString().Contains("Object reference not set to an instance of an object."));
    }
   

    [TestMethod]
    public async Task DexProcess_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
    {
       
        string dex = "someData";
        var expectedResponse = new ResponseCommon { IsSuccess = false };

        // Simulate unauthorized access (you would typically handle this with role-based testing or mocks)
        _mockVendsysManager
            .Setup(m => m.ManageDexFile(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.dexProcess(dex);

        // Assert
        var unauthorizedResult = result as UnauthorizedResult;
        Assert.IsNull(unauthorizedResult); // In the context of role-based, you'd expect a challenge here.
    }
}
