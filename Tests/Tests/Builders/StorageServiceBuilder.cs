using Application.Services;
using Moq;

namespace Tests.Builders;

public class StorageServiceBuilder
{
    private readonly Mock<IStorageService> service;

    public StorageServiceBuilder() => service = new Mock<IStorageService>();

    public StorageServiceBuilder SetupUploadFileAsync(bool resultado)
    {
        service.Setup(e => e.UploadFileAsync(It.IsAny<string>(), It.IsAny<MemoryStream>())).ReturnsAsync(true);
        return this;
    }

    public IStorageService Instancia() => service.Object;
}
