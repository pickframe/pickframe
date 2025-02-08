using Application.UseCases.CreateProcess;
using Application.UseCases.GetStatus;
using Domain.Entities.Process;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Builders;
using Xunit;

namespace Tests.UseCases;

public class GetStatusTest
{
    private static GetStatusHandler getStatusHandlerUseCase()
    {
        var logger = new Mock<ILogger<GetStatusHandler>>();
        
        var fakeId = Guid.NewGuid().ToString();
        var fakeOutputPath = $"result/{fakeId}";
        var fakeProcess = new Process(fakeId, ProcessStatus.processed, $"media/{fakeId}", fakeOutputPath);

        var storageService = new StorageServiceBuilder();
        storageService.SetupGetPublicURL(fakeOutputPath);

        var repository = new ProcessRepositoryBuilder();
        repository.SetupGetById(fakeProcess);

        return new GetStatusHandler(logger.Object, storageService.Instancia(), repository.Instancia());
    }

    [Fact]
    public async Task GetStatus_AnyProces_ReturnSucessAndPublicURL()
    {
        // Arrange
        var command = GetStatusRequestBuilder.Instancia();
        var useCase = getStatusHandlerUseCase();

        // Act
        var result = await useCase.Handle(command, default);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(ProcessStatus.processed.ToText());
        result.OutputMediaPath.Should().NotBeNullOrEmpty();
    }

}
