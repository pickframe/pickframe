using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.GetStatus;

public class GetStatusResponse : IRequest
{
    public string? Id { get; set; }
    public string? Status { get; set; }
    public string? OutputMediaPath { get; set; }
}
