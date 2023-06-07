using ComUnity.Application.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions
{
    internal class ThisEventIsForOwnersFriendsOnlyException : BusinessRuleException
    {
        public ThisEventIsForOwnersFriendsOnlyException() : base("This event is only private. To join you have to be friends with the host.") { }
    }
}