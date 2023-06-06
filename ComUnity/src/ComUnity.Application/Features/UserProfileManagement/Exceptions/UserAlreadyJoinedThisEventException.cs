using ComUnity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions
{
    public class UserAlreadyJoinedThisEventException : BusinessRuleException
    {
        public UserAlreadyJoinedThisEventException() : base($"You already joined this event.") { }
    }

}
