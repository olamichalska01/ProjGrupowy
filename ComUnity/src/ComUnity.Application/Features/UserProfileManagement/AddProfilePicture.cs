using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComUnity.Application.Features.UserProfileManagement;

public class AddProfilePictureController : ApiControllerBase
{
    [HttpPut("/api/profile/profile-picture")]
    [ProducesResponseType(typeof(AddProfilePictureResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AddProfilePictureResponse>> AddProfilePicture([FromBody] AddProfilePictureCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }
}

public record AddProfilePictureCommand(Guid PictureId) : IRequest<AddProfilePictureResponse>;

public record AddProfilePictureResponse(string PictureUrl);

internal class AddProfilePictureCommandHandler : IRequestHandler<AddProfilePictureCommand, AddProfilePictureResponse>
{
    private readonly ComUnityContext _context;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public AddProfilePictureCommandHandler(ComUnityContext context, IAzureStorageService azureStorageService, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _azureStorageService = azureStorageService;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<AddProfilePictureResponse> Handle(AddProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var user = await _authenticatedUserProvider.GetUserProfile(cancellationToken);

        if(!_azureStorageService.PictureExists(request.PictureId))
        {
            throw new Exception();
        }

        user.ChangeProfilePicture(request.PictureId);
        await _context.SaveChangesAsync(cancellationToken);

        var pictureUrl = _azureStorageService.GetReadFileToken(request.PictureId);

        return new AddProfilePictureResponse(pictureUrl);
    }
}