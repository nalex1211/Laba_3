using Laba_3.Controllers;
using Laba_3.Dto;
using Laba_3.Interfaces;
using Laba_3.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Xml.Linq;

namespace Tests;

[TestFixture]
public class Tests
{
    private Mock<IDiscussionRepository> _mockRepository;
    private DiscussionsController _controller;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IDiscussionRepository>();
        _controller = new DiscussionsController(_mockRepository.Object);

    }

    [Test]
    public async Task GetDiscussions_ReturnsAllDiscussions()
    {
        var comments = new List<Comment>() { new Comment { Id = 1, Text = "Some comment text", DiscussionId = 1 } };
        var discussions = new List<Discussion> { new Discussion { Id = 1, Title = "Test", Comments = comments } };
        _mockRepository.Setup(repo => repo.GetAllDiscussionsAsync()).ReturnsAsync(discussions);

        var result = await _controller.GetDiscussions();

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        var returnedDiscussions = okResult.Value as IEnumerable<DiscussionDto>;
        Assert.That(returnedDiscussions, Is.Not.Null);
        Assert.That(returnedDiscussions.Count(), Is.EqualTo(1));
        Assert.That(returnedDiscussions.First().Title, Is.EqualTo("Test"));
        _mockRepository.Verify(repo => repo.GetAllDiscussionsAsync(), Times.Once);
    }

    [Test]
    public async Task GetDiscussion_ThrowsException_WhenNotFound()
    {

        _mockRepository.Setup(repo => repo.GetDiscussionByIdAsync(It.IsAny<int>())).ReturnsAsync((Discussion)null);

        var result = await _controller.GetDiscussion(0);
        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        _mockRepository.Verify(repo => repo.GetDiscussionByIdAsync(It.IsAny<int>()), Times.Once);
    }


    [Test]
    public async Task CreateDiscussion_VerifiesMethodCall_ThrowsExceptionOnSave()
    {
        var discussionDto = new DiscussionDto { Title = "New Discussion", Description = "New discussion description" };
        _mockRepository.Setup(repo => repo.AddDiscussionAsync(It.IsAny<Discussion>())).ThrowsAsync(new Exception("Database error"));

        var ex = Assert.ThrowsAsync<Exception>(() => _controller.Create(discussionDto));
        Assert.That(ex.Message, Is.EqualTo("Database error"));
        _mockRepository.Verify(repo => repo.AddDiscussionAsync(It.IsAny<Discussion>()), Times.Once);
    }

    [Test]
    public async Task UpdateDiscussion_ReturnsUpdatedDiscussion()
    {
        var originalDiscussion = new Discussion { Id = 1, Title = "Original Title", Description = "Original Description" };
        var updatedDiscussionDto = new DiscussionDto { Title = "Updated Title", Description = "Updated Description" };
        _mockRepository.Setup(repo => repo.GetDiscussionByIdAsync(1)).ReturnsAsync(originalDiscussion);
        _mockRepository.Setup(repo => repo.UpdateDiscussionAsync(It.IsAny<Discussion>())).Returns(Task.CompletedTask).Verifiable();

        var result = await _controller.UpdateDiscussion(1, updatedDiscussionDto);

        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _mockRepository.Verify(repo => repo.UpdateDiscussionAsync(It.IsAny<Discussion>()), Times.Once);
        var okResult = result as OkObjectResult;
        var returnedDiscussion = okResult.Value as DiscussionDto;
        Assert.That(returnedDiscussion.Title, Is.EqualTo(updatedDiscussionDto.Title));
        Assert.That(returnedDiscussion.Description, Is.EqualTo(updatedDiscussionDto.Description));
    }

    [Test]
    public async Task UpdateDiscussion_ReturnsNotFound_ForNonexistentId()
    {
        _mockRepository.Setup(repo => repo.GetDiscussionByIdAsync(It.IsAny<int>())).ReturnsAsync((Discussion)null);


        var result = await _controller.UpdateDiscussion(99, new DiscussionDto());

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task DeleteDiscussion_ReturnsNoContent_WhenSuccessful()
    {
        _mockRepository.Setup(repo => repo.GetDiscussionByIdAsync(1)).ReturnsAsync(new Discussion());
        _mockRepository.Setup(repo => repo.DeleteDiscussionAsync(1)).Returns(Task.CompletedTask);

        var result = await _controller.DeleteDiscussion(1);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
        _mockRepository.Verify(repo => repo.DeleteDiscussionAsync(1), Times.Once);
    }

    [Test]
    public async Task DeleteDiscussion_ReturnsNotFound_WhenDiscussionDoesNotExist()
    {
        _mockRepository.Setup(repo => repo.GetDiscussionByIdAsync(It.IsAny<int>())).ReturnsAsync((Discussion)null);

        var result = await _controller.DeleteDiscussion(99);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetDiscussions_ReturnsInternalServerError_OnException()
    {
        _mockRepository.Setup(repo => repo.GetAllDiscussionsAsync()).ThrowsAsync(new Exception("Internal server error"));

        var result = await _controller.GetDiscussions();

        Assert.That(result.Result, Is.InstanceOf<ObjectResult>());
        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult.StatusCode, Is.EqualTo(500));
    }

    [Test]
    public async Task GetDiscussion_ReturnsNotFound_WhenDiscussionDoesNotExist()
    {
        var result = await _controller.GetDiscussion(1);

        Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        _mockRepository.Verify(repo => repo.GetDiscussionByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task CreateDiscussion_ReturnsCreatedDiscussion_WithValidData()
    {
        var newDiscussionDto = new DiscussionDto { Title = "New Title", Description = "New Description", CreatedDate = DateTime.Now };
        var newDiscussion = new Discussion { Id = 1, Title = newDiscussionDto.Title, Description = newDiscussionDto.Description };

        var mockRepository = new Mock<IDiscussionRepository>() { CallBase = true };
        mockRepository.Setup(repo => repo.AddDiscussionAsync(It.IsAny<Discussion>()))
                      .Callback<Discussion>(discussion =>
                      {
                          discussion.Id = 1;
                      })
                      .Returns(Task.CompletedTask);

        var controller = new DiscussionsController(mockRepository.Object);

        var result = await controller.Create(newDiscussionDto);

        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        var returnedDiscussion = okResult.Value as DiscussionDto;
        Assert.That(returnedDiscussion.Title, Is.EqualTo(newDiscussionDto.Title));
        Assert.That(returnedDiscussion.Description, Is.EqualTo(newDiscussionDto.Description));
        Assert.That(returnedDiscussion.CreatedDate, Is.EqualTo(newDiscussionDto.CreatedDate));
    }
}