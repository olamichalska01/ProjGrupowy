using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
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
    public class JoinEvent : ApiControllerBase
    {
        [HttpPatch("/api/user/join")]
        [ProducesResponseType(typeof(JoinEventResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<JoinEventResponse>> JoinEventTask([FromBody] JoinEventCommand command)
        {
            return await Mediator.Send(command);
        }

        public record JoinEventCommand(Guid UserId, Guid EventId) : MediatR.IRequest<JoinEventResponse>;
        public record JoinEventResponse(Guid userId, Guid eventId) : IRequest<JoinEventResponse>;

        public class JoinEventValidator : AbstractValidator<JoinEventCommand>
        {
            public JoinEventValidator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.EventId).NotEmpty();
            }
        }

        internal class JoinEventHandler : IRequestHandler<JoinEventCommand, JoinEventResponse>
        {
            private readonly ComUnityContext _context;

            public JoinEventHandler(ComUnityContext context)
            {
                _context = context;
            }

            public async Task<JoinEventResponse> Handle(JoinEventCommand request, CancellationToken cancellationToken)
            {
                var e = await _context.Set<Event>().FirstOrDefaultAsync(ev => ev.Id == request.EventId, cancellationToken);
                var u = await _context.Set<UserProfile>().FirstOrDefaultAsync(up => up.UserId == request.UserId, cancellationToken);

                var ue = await _context.Set<UserProfile>().FirstOrDefaultAsync(ue => ue.UserId == request.UserId && ue.UserEvents.Contains(e), cancellationToken);

                if(ue != null)
                {
                    throw new UserAlreadyJoinedThisEventException();
                }

                if(e.Participants.Count() == e.MaxAmountOfPeople)
                {
                    throw new EventFullException();
                }

                e.Participants.Add(u);
                e.TakenPlacesAmount++;
                u.UserEvents.Add(e);

                await _context.SaveChangesAsync(cancellationToken);

                return new JoinEventResponse(
                    e.Id,
                    u.UserId);
            }
        }
    }
}
