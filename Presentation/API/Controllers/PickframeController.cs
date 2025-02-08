using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Application.Services;
using Application.UseCases.CreateProcess;
using Application.UseCases.GetStatus;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("[controller]")]
[ApiController]
[ExcludeFromCodeCoverage]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class PickframeController : ControllerBase
{
    private readonly ILogger<PickframeController> _logger;
    private readonly IMediator _mediatorService;

    public PickframeController(ILogger<PickframeController> logger, IMediator mediatorService)
    {
        _logger = logger;
        _mediatorService = mediatorService;
    }

    /// <summary>
    /// Enviar o vídeo para processamento.
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetStatus([FromRoute] string id)
    {
        GetStatusRequest getStatusRequest = new() { Id = id };

        var getStatusResponse = await _mediatorService.Send(getStatusRequest);
        return Ok(getStatusResponse);
    }
}