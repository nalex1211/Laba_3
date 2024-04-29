using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laba_3.Data;
using Laba_3.Dto;
using Laba_3.Models;
using System.Xml.Linq;

namespace Laba_3.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public CommentsController(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments()
    {
        var comments = _mapper.Map<List<CommentDto>>(await _db.Comments.ToListAsync());

        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CommentDto>> GetComment(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }
        var comment = await _db.Comments.FindAsync(id);
        if (comment is null)
        {
            return BadRequest();
        }
        var result = _mapper.Map<CommentDto>(comment);
        return Ok(result);
    }
    [HttpPost("{discussionId}")]
    public async Task<ActionResult<CommentDto>> Create([FromBody] CommentDto commentDto, int discussionId)
    {
        var discussion = await _db.Discussions.FirstOrDefaultAsync(d => d.Id == discussionId);
        if (discussion == null)
        {
            return NotFound();
        }

        var comment = new Comment
        {
            Text = commentDto.Text,
            CreationDate = DateTime.Now,
            Discussion = discussion
        };

        await _db.Comments.AddAsync(comment);
        await _db.SaveChangesAsync();

        return Ok(commentDto);
    }
    
}
