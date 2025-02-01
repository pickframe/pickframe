using Domain.Entities.Process;
using Dapper;

namespace Postgresql.Repositories;

public class ProcessRepository : IProcessRepository
{
    private DbSession _dbSession;

    public ProcessRepository(DbSession dbSession)
    {
        _dbSession = dbSession;    
    }

    public async Task<Process> GetById(string id)
    {
        using var con = _dbSession.Connection;
        var result = await con.QuerySingleAsync<Process>(ProcessSQL.SELECT_BY_ID, new { Id = id });
        return result;
    }

    public async Task<bool> Save(Process process)
    {
        using var con = _dbSession.Connection;

        var obj = new
        { 
            process.Id, 
            Status = process.Status.ToText(),
            process.InputMediaPath,
            process.OutputMediaPath 
        };

        var result = await con.ExecuteAsync(ProcessSQL.INSERT_PROCESS, obj);
        return result == 1;
    }

    public async Task<bool> UpdateStatus(string id, ProcessStatus status)
    {
        using var con = _dbSession.Connection;
        var result = await con.ExecuteAsync(ProcessSQL.UPDATE_STATUS, new { Id = id, Status = status.ToText() });
        return result == 1;
    }

    public async Task<bool> InsertOutputPath(string id, string outuptmediapath)
    {
        using var con = _dbSession.Connection;
        var result = await con.ExecuteAsync(ProcessSQL.UPDATE_OUTPUMEDIATPATH, new { Id = id, OutputMediaPath = outuptmediapath });
        return result == 1;
    }
}

public static class ProcessSQL
{
    public const string SELECT_BY_ID = 
        @"SELECT id, status, inputmediapath, outuptmediapath
        FROM processes WHERE id = @Id";

    public const string INSERT_PROCESS =
        @"INSERT INTO processes (id, status, inputmediapath, outuptmediapath)
        VALUES (@Id, @Status, @InputMediaPath, @OutputMediaPath)";

    public const string UPDATE_STATUS =
        @"UPDATE processes SET status = @Status
        WHERE id = @Id;";

    public const string UPDATE_OUTPUMEDIATPATH =
        @"UPDATE processes SET outuptmediapath = @OutputMediaPath
        WHERE id = @Id;";
}