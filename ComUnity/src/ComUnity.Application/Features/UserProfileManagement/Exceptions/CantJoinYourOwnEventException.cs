using ComUnity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions
{
    internal class CantJoinYourOwnEventException : BusinessRuleException
    {
        public CantJoinYourOwnEventException() : base("You can't join an event you are hosting.") { }
    }
}