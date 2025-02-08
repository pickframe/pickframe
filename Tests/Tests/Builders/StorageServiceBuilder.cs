using Application.Services;
using Moq;

namespace Tests.Builders;

public class StorageServiceBuilder
{
    private readonly Mock<IStorageService> service;

    public StorageServiceBuilder() => service = new Mock<IStorageService>();

    public StorageServiceBuilder SetupUploadFileAsync(bool resultado)
    {
        service.Setup(e => e.UploadFileAsync(It.IsAny<string>(), It.IsAny<MemoryStream>())).ReturnsAsync(resultado);
        return this;
    }

    public StorageServiceBuilder SetupUploadInvalidPath()
    {
        service.Setup(e => e.UploadFileAsync(It.IsAny<string>(), It.IsAny<MemoryStream>())).Throws(new Exception());
        return this;
    }

    public StorageServiceBuilder SetupDownloadFileAsync(bool result)
    {
        service.Setup(e => e.DownloadFileAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(result);
        return this;
    }

    public StorageServiceBuilder SetupGetPublicURL(string result)
    {
        service.Setup(e => e.GetPublicURL(It.IsAny<string>())).ReturnsAsync(result);
        return this;
    }

    public IStorageService Instancia() => service.Object;
}
