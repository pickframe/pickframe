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
        private static UploadVideoHandler UploadVideoHandlerUseCase(bool result, bool throwsException)
        {
            var logger = new Mock<ILogger<UploadVideoHandler>>().Object;

            var storageService = new StorageServiceBuilder();

            if (throwsException)
            {
                storageService.SetupUploadInvalidPath();
            }
            else
            {
                storageService.SetupUploadFileAsync(result);
            }

            return new UploadVideoHandler(logger, storageService.Instancia());
        }

        [Fact]
        public async Task UploadVideo_AnyVideo_ReturnsSucessAndVideoIdAndVideoPath()
        {
            // Arrange
            var request = UploadVideoRequestBuilder.Instancia();
            var useCase = UploadVideoHandlerUseCase(true, false);

            // Act
            var result = await useCase.Handle(request, default);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.Id.Should().NotBeNullOrEmpty();
            result.Path.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UploadVideo_AnyVideo_ReturnsErrorToUploadVideo()
        {
            // Arrange
            var request = UploadVideoRequestBuilder.Instancia();
            var useCase = UploadVideoHandlerUseCase(false, false);

            // Act
            var result = await useCase.Handle(request, default);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Success.Should().BeFalse();
        }

        [Fact]
        public async Task UploadVideo_AnyVideoWithInvalidPath_ReturnsException()
        {
            // Arrange
            var request = UploadVideoRequestBuilder.Instancia();
            var useCase = UploadVideoHandlerUseCase(true, true);

            // Act
            Func<Task> acao = async () => await useCase.Handle(request, default);
            
            // Assert
            var result = await acao.Should().ThrowAsync<Exception>();
        }
    }
}
