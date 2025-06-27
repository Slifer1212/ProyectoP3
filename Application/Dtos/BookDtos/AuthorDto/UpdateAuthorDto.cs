using Application.Dtos.BaseDto;

namespace Application.Dtos.AuthorDto;

public class UpdateAuthorDto : CreateAuthorDto
{
    public Guid Id { get; set; }
}