using Application.UseCases.GetStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Builders;

public class GetStatusRequestBuilder
{
    public static GetStatusRequest Instancia()
    {
        return new()
        {
            Id = Guid.NewGuid().ToString()
        };
    }
}
