using ComUnity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions
{
    internal class EventFullException : BusinessRuleException
    {
        public EventFullException() : base("This event is already full.") { }
    }
}