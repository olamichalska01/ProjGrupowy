using ComUnity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions
{
    internal class UserHasntJoindThisEventException : BusinessRuleException
    {
        public UserHasntJoindThisEventException() : base($"You haven't joined this event.") { }
    }

}
