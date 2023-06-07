using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComUnity.Application.Features.UserProfileManagement;

public class UpdateUserProfileController : ApiControllerBase
{
    [HttpPut("/api/users/profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public record UpdateUserProfileCommand(string? AboutMe, string? City) : IRequest<Unit>;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.AboutMe)
            .MinimumLength(1)
            .MaximumLength(2000);

        RuleFor(x => x.City)
            .MinimumLength(2)
            .MaximumLength(128);
    }
}

internal class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public UpdateUserProfileCommandHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _authenticatedUserProvider.GetUserProfile(cancellationToken);

        user.UpdateBasicProfileInformation(request.AboutMe, request.City);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
