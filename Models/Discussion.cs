namespace Laba_3.Models;

public class Discussion
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; } = DateTime.Now;
    public List<Comment>? Comments { get; set; }
}
