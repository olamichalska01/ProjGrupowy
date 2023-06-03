using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.UserProfileManagement;


public class AddUserFavoriteEventCategory : ApiControllerBase
    {
    [HttpPost("/api/user/favorite")]
    [ProducesResponseType(typeof(AddFavoriteCategoryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AddFavoriteCategoryResponse>> AddUserFavoriteCategory([FromBody] AddFavoriteCategoryCommand command)
    {
        return await Mediator.Send(command);
    }

    public record AddFavoriteCategoryCommand(
    Guid UserId,
    Guid EventCategoryId)
    : MediatR.IRequest<AddFavoriteCategoryResponse>;
    public record AddFavoriteCategoryResponse(
    Guid userId,
    Guid categoryId)
    : IRequest<AddFavoriteCategoryResponse>;

    public class AddFavoriteCategoryValidator : AbstractValidator<AddFavoriteCategoryCommand>
    {
        public AddFavoriteCategoryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.EventCategoryId).NotEmpty();
        }
    }

    internal class AddFavoriteCategoryHandler : IRequestHandler<AddFavoriteCategoryCommand, AddFavoriteCategoryResponse>
    {
        private readonly ComUnityContext _context;

        public AddFavoriteCategoryHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<AddFavoriteCategoryResponse> Handle(AddFavoriteCategoryCommand request, CancellationToken cancellationToken)
        {
            var eCategory = await _context.Set<UserFavoriteEventCategory>().FirstOrDefaultAsync(ec => ec.EventCategoryId == request.EventCategoryId && ec.UserId == request.UserId, cancellationToken);

            if (eCategory != null)
            {
                throw new EventCategoryDoesNotExistException(request.EventCategoryId.ToString());
            }


            var e = new UserFavoriteEventCategory(
                request.UserId,
                request.EventCategoryId);

            await _context.AddAsync(e, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddFavoriteCategoryResponse(
                e.UserId,
                e.EventCategoryId);
        }
    }
}

