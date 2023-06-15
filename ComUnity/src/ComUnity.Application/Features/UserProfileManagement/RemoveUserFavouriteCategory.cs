using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using ComUnity.Application.Features.UserProfileManagement.Entities;
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
    public class RemoveUserFavouriteCategory : ApiControllerBase
    {
        [HttpDelete("/api/user/favorite-delete")]
        [ProducesResponseType(typeof(RemoveFavoriteCategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<RemoveFavoriteCategoryResponse>> RemoveUserFavouriteEventCategory([FromBody] RemoveFavoriteCategoryCommand command)
        {
            return await Mediator.Send(command);
        }

        public record RemoveFavoriteCategoryCommand(
        Guid UserId,
        Guid EventCategoryId)
        : MediatR.IRequest<RemoveFavoriteCategoryResponse>;
        public record RemoveFavoriteCategoryResponse(
        Guid userId,
        Guid categoryId)
        : IRequest<RemoveFavoriteCategoryResponse>;

        public class RemoveFavoriteCategoryValidator : AbstractValidator<RemoveFavoriteCategoryCommand>
        {
            public RemoveFavoriteCategoryValidator()
            {
                RuleFor(x => x.UserId).NotEmpty();
                RuleFor(x => x.EventCategoryId).NotEmpty();
            }
        }

        internal class RemoveFavoriteCategoryHandler : IRequestHandler<RemoveFavoriteCategoryCommand, RemoveFavoriteCategoryResponse>
        {
            private readonly ComUnityContext _context;

            public RemoveFavoriteCategoryHandler(ComUnityContext context)
            {
                _context = context;
            }

            public async Task<RemoveFavoriteCategoryResponse> Handle(RemoveFavoriteCategoryCommand request, CancellationToken cancellationToken)
            {
                var eCategory = await _context.Set<UserFavoriteEventCategory>().FirstOrDefaultAsync(ec => ec.EventCategoryId == request.EventCategoryId && ec.UserId == request.UserId, cancellationToken);

                if (eCategory != null)
                {
                    throw new EventCategoryDoesNotExistException(request.EventCategoryId.ToString());
                }


                var e = new UserFavoriteEventCategory(
                    request.UserId,
                    request.EventCategoryId);

                _context.Remove(e);
                await _context.SaveChangesAsync(cancellationToken);

                return new RemoveFavoriteCategoryResponse(
                    e.UserId,
                    e.EventCategoryId);
            }
        }
    }
}

