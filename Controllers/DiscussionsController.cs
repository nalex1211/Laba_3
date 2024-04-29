using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Laba_3.Data;
using Laba_3.Dto;
using Laba_3.Helper;
using Laba_3.Interfaces;
using Laba_3.Models;

namespace Laba_3.Controllers;
[Route("api/[controller]")]
[ApiController]
public class DiscussionsController : ControllerBase
{
    private readonly IDiscussionRepository _repository;

    public DiscussionsController(IDiscussionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscussionDto>>> GetDiscussions()
    {
        try
        {
            var discussions = await _repository.GetAllDiscussionsAsync();
            var discussionDtos = discussions.Select(d => new DiscussionDto
            {
                Title = d.Title,
                Description = d.Description,
                Comments = d.Comments.Select(c => new CommentDto
                {
                    Text = c.Text,
                    CreatedDate = c.CreationDate,
                    DiscussionId = c.DiscussionId
                }).ToList()
            }).ToList();
            return Ok(discussionDtos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<DiscussionDto>> GetDiscussion(int id)
    {
        var discussion = await _repository.GetDiscussionByIdAsync(id);

        if (discussion is null)
        {
            return NotFound();
        }

        var discussionDto = new DiscussionDto()
        {
            Title = discussion.Title,
            Description = discussion.Description,
            Comments = discussion.Comments.Select(c => new CommentDto
            {
                Text = c.Text,
                CreatedDate = c.CreationDate,
                DiscussionId = c.DiscussionId
            }).ToList()
        };

        return Ok(discussionDto);
    }



    [HttpPost]
    public async Task<ActionResult<DiscussionDto>> Create([FromBody] DiscussionDto discussionDto)
    {
        var discussion = new Discussion()
        {
            Title = discussionDto.Title,
            Description = discussionDto.Description,
            CreationDate = DateTime.Now
        };

        await _repository.AddDiscussionAsync(discussion);

        discussionDto.CreatedDate = discussion.CreationDate;

        return Ok(discussionDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDiscussion(int id, [FromBody] DiscussionDto discussionDto)
    {
        var discussion = await _repository.GetDiscussionByIdAsync(id);
        if (discussion == null)
        {
            return NotFound();
        }

        discussion.Title = discussionDto.Title;
        discussion.Description = discussionDto.Description;

        await _repository.UpdateDiscussionAsync(discussion);
        return Ok(discussionDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDiscussion(int id)
    {
        var discussion = await _repository.GetDiscussionByIdAsync(id);
        if (discussion == null)
        {
            return NotFound();
        }

        await _repository.DeleteDiscussionAsync(id);
        return NoContent();
    }
}
