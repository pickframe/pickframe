using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.GetStatus;

public class GetStatusRequest : IRequest<GetStatusResponse>
{
    public string Id { get; set; } = string.Empty;
}
