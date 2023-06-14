using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComUnity.Application.Features.UserProfileManagement.Dtos;

namespace ComUnity.Application.Features.ManagingEvents.Dtos;

public record PostDto(
    Guid Id,
    string AuthorName,
    string PostName,
    DateTime PublishedDate,
    string PostText);
