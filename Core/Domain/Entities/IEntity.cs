using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Notification;

namespace Domain.Entities
{
    public interface IEntity
    {
        public ErrorList Validate();
    }
}