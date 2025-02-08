using Application.Services;
using Application.UseCases.CreateProcess;
using Application.UseCases.UploadVideo;
using Domain.Entities.Process;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Builders;
using Xunit;

namespace Tests.UseCases;

public class CreateProcessTest
{
    private static CreateProcessHandler CreateProcessHandlerUseCase(bool uploadResult, bool saveResult, bool throwException = false)
    {
        var logger = new Mock<ILogger<CreateProcessHandler>>();
        var enqueuService = new Mock<IEnqueuService>();

        string videoId = Guid.NewGuid().ToString();
        var response = new UploadVideoResponse()
        {
            Id = videoId,
            Success = uploadResult,
            Path = $"media/{videoId}"
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(e => e.Send(It.IsAny<UploadVideoRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var repository = new Mock<IProcessRepository>();

        if (throwException)
        {
            repository.Setup(i => i.Save(It.IsAny<Process>())).Throws(new Exception("test"));
        }
        else
        {
            repository.Setup(i => i.Save(It.IsAny<Process>())).ReturnsAsync(saveResult);
        }

        return new CreateProcessHandler(logger.Object, enqueuService.Object, mediator.Object, repository.Object);
    }

    [Fact]
    public async Task CreateProcess_AnyProcess_ReturnsSucessAndProcessId()
    {
        // Arrange
        var request = CreateProcessRequestBuilder.Instancia();
        var useCase = CreateProcessHandlerUseCase(true, true);

        // Act
        var result = await useCase.Handle(request, default);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateProcess_AnyProcess_ReturnsFailToUploadVideo()
    {
        // Arrange
        var request = CreateProcessRequestBuilder.Instancia();
        var useCase = CreateProcessHandlerUseCase(false, true);

        // Act
        var result = await useCase.Handle(request, default);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Id.Should().BeEmpty();
        result.Message.Should().Be("Error uploading video");
    }

    [Fact]
    public async Task CreateProcess_AnyProcess_ReturnsFailToSaveVideo()
    {
        // Arrange
        var request = CreateProcessRequestBuilder.Instancia();
        var useCase = CreateProcessHandlerUseCase(true, false);

        // Act
        var result = await useCase.Handle(request, default);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Id.Should().NotBeNullOrEmpty();
        result.Message.Should().Be("Error saving video to database");
    }

    [Fact]
    public async Task CreateProcess_AnyProcess_ReturnsException()
    {
        // Arrange
        var request = CreateProcessRequestBuilder.Instancia();
        var useCase = CreateProcessHandlerUseCase(true, false, true);

        // Act
        Func<Task> acao = async () => await useCase.Handle(request, default);

        // Assert
        var result = await acao.Should().ThrowAsync<Exception>();
    }
}
