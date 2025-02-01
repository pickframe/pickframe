using System.ComponentModel.DataAnnotations;
using Application.UseCases.CreateProcess;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class VideoController : ControllerBase
{
    private readonly ILogger<VideoController> _logger;
    private readonly IMediator _mediatorService;

    public VideoController(ILogger<VideoController> logger, IMediator mediatorService)
    {
        _logger = logger;
        _mediatorService = mediatorService;
    }

    /// <summary>
    /// Processes the uploaded video and extracts frames at the specified interval.
    /// </summary>
    /// <param name="video">The video file to be processed.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost]
    public async Task<IActionResult> ProcessVideo([Required] IFormFile video)
    {
        _logger.LogDebug("ProcessVideo called");

        MemoryStream videoStream = new();
        video.CopyTo(videoStream);

        CreateProcessRequest createProcessRequest = new()
        {
            VideoStream = videoStream
        };

        var createProcessResponse = await _mediatorService.Send(createProcessRequest);

        if (!createProcessResponse.Success)
        {
            return BadRequest(createProcessResponse);
        }

        return Created("/video", createProcessResponse);
    }
}