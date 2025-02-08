using Application.Services;
using Domain.Entities.Process;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.GetStatus;

public class GetStatusHandler : IRequestHandler<GetStatusRequest, GetStatusResponse>
{
    public GetStatusHandler(ILogger<GetStatusHandler> logger, IStorageService storageService, IProcessRepository processRepository)
    {
        Logger = logger;
        StorageService = storageService;
        ProcessRepository = processRepository;
    }

    public ILogger<GetStatusHandler> Logger { get; }
    public IStorageService StorageService { get; }
    public IProcessRepository ProcessRepository { get; }

    public async Task<GetStatusResponse> Handle(GetStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var process = await ProcessRepository.GetById(request.Id);

            GetStatusResponse response = new()
            {
                Id = process!.Id,
                Status = process.Status.ToText()
            };

            if (process!.Status == ProcessStatus.processed)
            {
                response.OutputMediaPath = await StorageService.GetPublicURL(process.OutputMediaPath!);
            }

            return response;
        }
        catch(Exception ex)
        {
            Logger.LogError("Erro: {Mensagem}", ex.Message);
            throw;
        }
    }
}
