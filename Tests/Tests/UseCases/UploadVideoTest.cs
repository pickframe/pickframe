using Application.UseCases.UploadVideo;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Builders;
using Xunit;

namespace Tests.UseCases
{
    public class UploadVideoTest
    {
        private static UploadVideoHandler UploadVideoHandlerUseCase(bool result)
        {
            var logger = new Mock<ILogger<UploadVideoHandler>>().Object;

            var storageService = new StorageServiceBuilder()
                .SetupUploadFileAsync(result)
                .Instancia();

            return new UploadVideoHandler(logger, storageService);
        }

        [Fact]
        public async Task UploadVideo_AnyVideo_ReturnsSucessAndVideoIdAndVideoPath()
        {
            // Arrange
            var request = UploadVideoRequestBuilder.Instancia();
            var useCase = UploadVideoHandlerUseCase(true);

            // Act
            var result = await useCase.Handle(request, default);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Id.Should().NotBeNullOrEmpty();
            result.Path.Should().NotBeNullOrEmpty();
        }
    }
}
