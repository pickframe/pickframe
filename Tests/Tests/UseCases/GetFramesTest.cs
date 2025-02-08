using Application.Exceptions;
using Application.Services;
using Application.UseCases.GetFrames;
using Domain.Entities.Process;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Builders;
using Xunit;

namespace Tests.UseCases;

public class GetFramesTest
{
    private static GetFramesHandler GetFramesHandlerUseCase(Process? process, bool download, bool upload, bool pickFramesResult)
    {
        var logger = new Mock<ILogger<GetFramesHandler>>();
        var videoProcessor = new Mock<IVideoProcessorService>();

        videoProcessor
            .Setup(e => e.PickFramesAt(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
            .Returns(pickFramesResult);

        var repository = new ProcessRepositoryBuilder()
            .SetupGetById(process)
            .SetupInsertOutputPath(true)
            .SetupUpdateStatus(true)
            .Instancia();

        var storage = new StorageServiceBuilder()
            .SetupUploadFileAsync(upload)
            .SetupDownloadFileAsync(download)
            .Instancia();


        return new GetFramesHandler(logger.Object, repository, storage, videoProcessor.Object);
    }

    [Fact]
    public async Task GetFrames_AnyProcess_ReturnsSucess()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var fakeProcess = new Process(id, ProcessStatus.queued, $"media/{id}");
        var command = new GetFramesRequest() { Id = id };
        var useCase = GetFramesHandlerUseCase(download: true, upload: true, process: fakeProcess, pickFramesResult: true);

        // Act 
        var result = await useCase.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeTrue();
    }

    [Fact]
    public async Task GetFrames_AnyProcess_ReturnFailDownloading()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var fakeProcess = new Process(id, ProcessStatus.queued, $"media/{id}");
        var command = new GetFramesRequest() { Id = id };
        var useCase = GetFramesHandlerUseCase(download: false, upload: true, process: fakeProcess, pickFramesResult: true);

        // Act 
        var result = await useCase.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Sucess.Should().BeFalse();
    }

    [Fact]
    public async Task GetFrames_InvalidProcess_ReturnProcessNotFound()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var command = new GetFramesRequest() { Id = id };
        var useCase = GetFramesHandlerUseCase(download: true, upload: true, process: null, pickFramesResult: true);

        // Act 
        Func<Task> acao = async () => await useCase.Handle(command, default);

        // Assert
        var result = await acao.Should().ThrowAsync<ProcessNotFoundException>();
    }
}
