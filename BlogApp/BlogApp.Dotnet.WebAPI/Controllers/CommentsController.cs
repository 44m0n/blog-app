using BlogApp.Dotnet.ApplicationCore.DTOs;
using BlogApp.Dotnet.ApplicationCore.Interfaces;
using BlogApp.Dotnet.ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Threading.Tasks;

namespace BlogApp.Dotnet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentsService _commentsService;

        public CommentsController(ICommentsService commentsService)
        {
            _commentsService = commentsService;
        }

        // GET: api/Comments
        [HttpGet]
        public PaginatedDTO<CommentsDTO> GetComments(int postID, string search, int page = 1)
        {
            var commentDTOs = _commentsService.GetTopComments(postID, page, search ??= "");
            Log.Information("API: All Paginated Comments retrieved from database.");

            return commentDTOs;
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public PaginatedDTO<CommentsDTO> GetReplies(int id, int postID, int page = 1)
        {
            var replyDTO = _commentsService.GetReplies(postID, id, page);
            return replyDTO;
        }

        // PUT: api/Comments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(int id, [Bind("ID,UserID,Date,PostID,Content,ParentID")] CommentsDTO comment)
        {
            if (id != comment.ID || !ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await _commentsService.Update(comment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!(await CommentExists(comment)))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            Log.Information("API: The comment has been updated.");
            return NoContent();
        }

        // POST: api/Comments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment([Bind("ID,PostID,Content,ParentID")] CommentsDTO comment)
        {
            comment.UserID = "testuser"; //need to be replaced
            comment.Date = DateTime.Now;

            ModelState.Remove("UserID");
            if (ModelState.IsValid)
            {
                await _commentsService.Add(comment);
                Log.Information("API: The comment has been added to database.");
                return NoContent();
            }

            return BadRequest();
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _commentsService.Get(id);

            if (comment == null)
            {
                return NotFound();
            }

            await _commentsService.Remove(comment.ID);

            Log.Information("API: The comment has been deleted.");
            return NoContent();
        }

        private async Task<bool> CommentExists(CommentsDTO comment)
        {
            return await _commentsService.Add(comment) != null;
        }
    }
}
