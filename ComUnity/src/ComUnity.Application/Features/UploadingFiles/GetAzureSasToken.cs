using ComUnity.Application.Common;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComUnity.Application.Features.UploadingFiles;

public class GetAzureSasTokenController : ApiControllerBase
{
    [HttpGet("/api/files/sas-token")]
    [ProducesResponseType(typeof(GetAzureSasTokenResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAzureSasTokenResponse>> GetAzureSasToken(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetAzureSasTokenQuery(), cancellationToken);
    }
}

public record GetAzureSasTokenQuery() : IRequest<GetAzureSasTokenResponse>;
public record GetAzureSasTokenResponse(string Token);

public class GetAzureSasTokenQueryHandler : IRequestHandler<GetAzureSasTokenQuery, GetAzureSasTokenResponse>
{
    private readonly IAzureStorageService _azureStorageService;

    public GetAzureSasTokenQueryHandler(IAzureStorageService azureStorageService)
    {
        _azureStorageService = azureStorageService;
    }

    public async Task<GetAzureSasTokenResponse> Handle(GetAzureSasTokenQuery request, CancellationToken cancellationToken)
        => new GetAzureSasTokenResponse(await _azureStorageService.GenerateNewWriteToken());
}