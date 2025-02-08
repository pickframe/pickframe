namespace Domain.Entities.Process;

public interface IProcessRepository
{
    public Task<Process?> GetById(string id);
    public Task<bool> Save(Process process);
    public Task<bool> Update(Process process);
    public Task<bool> UpdateStatus(string id, ProcessStatus status);
    public Task<bool> InsertOutputPath(string id, string outuptmediapath);
}