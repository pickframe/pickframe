using Application.Services;
using Domain.Entities.Process;
using Moq;

namespace Tests.Builders;

public class ProcessRepositoryBuilder
{
    private readonly Mock<IProcessRepository> service;

    public ProcessRepositoryBuilder() => service = new Mock<IProcessRepository>();

    public ProcessRepositoryBuilder SetupGetById(Process process)
    {
        service.Setup(e => e.GetById(It.IsAny<string>())).ReturnsAsync(process);
        return this;
    }

    public ProcessRepositoryBuilder SetupSave(bool result)
    {
        service.Setup(e => e.Save(It.IsAny<Process>())).ReturnsAsync(result);
        return this;
    }

    public ProcessRepositoryBuilder SetupUpdateStatus(bool result)
    {
        service.Setup(e => e.UpdateStatus(It.IsAny<string>(), It.IsAny<ProcessStatus>())).ReturnsAsync(result);
        return this;
    }

    public ProcessRepositoryBuilder SetupInsertOutputPath(bool result)
    {
        service.Setup(e => e.InsertOutputPath(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(result);
        return this;
    }

    public IProcessRepository Instancia() => service.Object;
}
