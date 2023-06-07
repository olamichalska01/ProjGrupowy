using ComUnity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions
{
    internal class CantLeaveYourOwnEventException : BusinessRuleException
    {
        public CantLeaveYourOwnEventException() : base("You can't leave an event you are hosting.") { }
    }
}