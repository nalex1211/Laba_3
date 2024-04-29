using Laba_3.Models;

namespace Laba_3.Dto;

public class CommentDto
{
    public string Text { get; set; }
    public DateTime CreatedDate { get; set; }
    public int DiscussionId { get; set; }
}
