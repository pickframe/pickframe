using Application.Services;
using Application.UseCases.UploadVideo;
using Domain.Entities.Process;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.CreateProcess
{
    public class CreateProcessHandler : IRequestHandler<CreateProcessRequest, CreateProcessResponse>
    {   
        private ILogger<CreateProcessHandler> Logger { get; set; }
        private IEnqueuService EnqueuService { get; set; }
        private IMediator MediatorService { get; set; }
        private IProcessRepository _processRepository;

        public CreateProcessHandler(ILogger<CreateProcessHandler> logger, IEnqueuService enqueuService, IMediator mediatorService, IProcessRepository processRepository)
        {
            Logger = logger;
            EnqueuService = enqueuService;
            MediatorService = mediatorService;
            _processRepository = processRepository;
        }

        public async Task<CreateProcessResponse> Handle(CreateProcessRequest request, CancellationToken cancellationToken)
        {

            try
            {
                CreateProcessResponse response = new();
                
                UploadVideoRequest uploadVideoRequest = new()
                {
                    VideoStream = request.VideoStream
                };

                var uploadResult = await MediatorService.Send(uploadVideoRequest, cancellationToken);

                if (uploadResult.Success)
                {
                    response.Id = uploadResult.Id;

                    Process process = new(uploadResult.Id, ProcessStatus.queued, uploadResult.Path);

                    if (await _processRepository.Save(process))
                    {
                        Logger.LogInformation("Video saved to database");
                        await EnqueuService.EnqueueProcess(uploadResult.Id);
                        response.Success = true;
                        response.Message = "Process saved and enqueued";
                    }
                    else
                    {
                        Logger.LogError("Error saving video to database");
                        response.Message = "Error saving video to database";
                    }

                }
                else
                {
                    Logger.LogError("Error uploading video");
                    response.Message = "Error uploading video";
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error on enqueue process");
                throw;
            }
        }
    }
}