using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.UserProfileManagement.Dtos;

public record UserDto(
    Guid UserId,
    string UserName,
    string? ProfilePicture
    );