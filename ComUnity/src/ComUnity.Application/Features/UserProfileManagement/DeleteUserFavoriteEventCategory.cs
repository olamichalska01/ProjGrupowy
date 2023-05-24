using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ComUnity.Application.Features.ManagingEvents.DeleteEventCategoryController;
using static ComUnity.Application.Features.UserProfileManagement.AddUserFavoriteEventCategory;

namespace ComUnity.Application.Features.UserProfileManagement;

    public class DeleteUserFavoriteEventCategory : ApiControllerBase
    {
        [HttpDelete("/api/favorite")]
        public async Task<ActionResult> DeleteFavoriteCategory([FromBody] DeleteFavoriteCategoryCommand deleteFavoriteCategoryCommand )
        {
            await Mediator.Send(deleteFavoriteCategoryCommand);
            return NoContent();
        }

    public record DeleteFavoriteCategoryCommand(
        Guid userId,
        Guid categoryId)
        : IRequest<Unit>;

    internal class DeleteFavoriteCategoryCommandHandler : IRequestHandler<DeleteFavoriteCategoryCommand, Unit>
    {
        private readonly ComUnityContext _context;

        public DeleteFavoriteCategoryCommandHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteFavoriteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Set<UserFavoriteEventCategory>().FirstOrDefaultAsync(e => (e.EventCategoryId == request.categoryId
            && e.UserId == request.userId), cancellationToken);

            if (category == null)
            {
                throw new NotFoundException(nameof(UserFavoriteEventCategory));
            }

            _context.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
