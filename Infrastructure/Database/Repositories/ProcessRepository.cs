using Domain.Entities.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Repositories;

[ExcludeFromCodeCoverage]
public class ProcessRepository : IProcessRepository
{
    private readonly DatabaseContext _context;
    private readonly ILogger<ProcessRepository> logger;

    public ProcessRepository(DatabaseContext context, ILogger<ProcessRepository> logger)
    {
        _context = context;
        this.logger = logger;
    }

    public async Task<Process?> GetById(string id)
    {
        return await _context.Processes.FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<bool> InsertOutputPath(string id, string outuptmediapath)
    {
        var p = await this.GetById(id);

        if (p is not null)
        {
            p.OutputMediaPath = outuptmediapath;
            _context.Processes.Update(p);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> Save(Process process)
    {
        try
        {
            await _context.AddAsync(process);
            await _context.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            logger.LogError("Error: {ErrorMessage}", e.Message);
            return false;
        }
    }

    public async Task<bool> Update(Process process)
    {
        try
        {
            _context.Update(process);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            logger.LogError("Error: {ErrorMessage}", e.Message);
            return false;
        }
    }

    public async Task<bool> UpdateStatus(string id, ProcessStatus status)
    {
        var p = await this.GetById(id);

        if (p is not null)
        {
            p.Status = status;
            _context.Processes.Update(p);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
