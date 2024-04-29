using Laba_3.Models;

namespace Laba_3.Dto;

public class DiscussionDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<CommentDto> Comments { get; set; }
}
