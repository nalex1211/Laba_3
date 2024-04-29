namespace Laba_3.Models;

public class Comment
{
    public int Id { get; set; }
    public string Text { get; set; }
    public int DiscussionId { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public Discussion Discussion { get; set; }
}
