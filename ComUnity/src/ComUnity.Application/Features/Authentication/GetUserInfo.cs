using ComUnity.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace ComUnity.Application.Features.Authentication;

public class GetUserInfoController : ApiControllerBase
{
    [HttpGet("/api/auth/user-info")]
    [ProducesResponseType(typeof(GetUserInfoResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetUserInfoResponse>> GetUserInfo(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetUserInfoQuery(), cancellationToken);
    }

    public record GetUserInfoQuery() : IRequest<GetUserInfoResponse>;

    public record GetUserInfoResponse(string UserId, string UserEmail, string UserRole);

    internal class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, GetUserInfoResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserInfoQueryHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetUserInfoResponse> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
        {
            var claims = _httpContextAccessor.HttpContext!.User.Claims ?? _httpContextAccessor.HttpContext!.User.Identities.First().Claims;

            await Task.CompletedTask;
            return new GetUserInfoResponse(
                UserId: claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value,
                UserEmail: claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value,
                UserRole: claims.First(x => x.Type == "role").Value);
        }
    }
}
