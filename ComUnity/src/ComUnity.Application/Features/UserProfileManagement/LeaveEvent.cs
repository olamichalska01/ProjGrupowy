using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Features.UserProfileManagement.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement
{
    public class LeaveEvent : ApiControllerBase
    {
        [HttpPatch("/api/user/leave")]
        [ProducesResponseType(typeof(LeaveEventResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<LeaveEventResponse>> LeaveEventTask([FromBody] LeaveEventCommand command)
        {
            return await Mediator.Send(command);
        }

        public record LeaveEventCommand(Guid UserId, Guid EventId) : MediatR.IRequest<LeaveEventResponse>;
        public record LeaveEventResponse(Guid userId, Guid eventId) : IRequest<LeaveEventResponse>;

        public class LeaveEventValidator : AbstractValidator<LeaveEventCommand>
        {
            public LeaveEventValidator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.EventId).NotEmpty();
            }
        }

        internal class LeaveEventHandler : IRequestHandler<LeaveEventCommand, LeaveEventResponse>
        {
            private readonly ComUnityContext _context;

            public LeaveEventHandler(ComUnityContext context)
            {
                _context = context;
            }

            public async Task<LeaveEventResponse> Handle(LeaveEventCommand request, CancellationToken cancellationToken)
            {
                var e = await _context.Set<Event>().Include(x => x.Participants).FirstOrDefaultAsync(ev => ev.Id == request.EventId, cancellationToken);

                var u = await _context.Set<UserProfile>().Include(x => x.UserEvents).FirstOrDefaultAsync(a => a.UserId == request.UserId && a.UserEvents.Contains(e), cancellationToken);

                if (u == null)
                {
                    throw new UserHasntJoindThisEventException();
                }

                if (e.OwnerId == u.UserId)
                {
                    throw new CantLeaveYourOwnEventException();
                }

                e.Participants.Remove(u);
                e.TakenPlacesAmount--;
                u.UserEvents.Remove(e);

                await _context.SaveChangesAsync(cancellationToken);

                return new LeaveEventResponse(u.UserId, e.Id);
            }
        }
    }
}