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
    private static CreateProcessHandler CreateProcessHandlerUseCase()
    {
        var logger = new Mock<ILogger<CreateProcessHandler>>();
        var enqueuService = new Mock<IEnqueuService>();

        string videoId = Guid.NewGuid().ToString();
        var response = new UploadVideoResponse()
        {
            Id = videoId,
            Success = true,
            Path = $"media/{videoId}"
        };

        var mediator = new Mock<IMediator>();
        mediator.Setup(e => e.Send(It.IsAny<UploadVideoRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        var repository = new Mock<IProcessRepository>();
        repository.Setup(i => i.Save(It.IsAny<Process>())).ReturnsAsync(true);

        return new CreateProcessHandler(logger.Object, enqueuService.Object, mediator.Object, repository.Object);
    }

    [Fact]
    public async Task CreateProcess_AnyProcess_ReturnsSucessAndProcessId()
    {
        // Arrange
        var request = CreateProcessRequestBuilder.Instancia();
        var useCase = CreateProcessHandlerUseCase();

        // Act
        var result = await useCase.Handle(request, default);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Id.Should().NotBeNullOrEmpty();
    }
}
