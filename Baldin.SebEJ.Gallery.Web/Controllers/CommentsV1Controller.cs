using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
using Baldin.SebEJ.Gallery.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Baldin.SebEJ.Gallery.Web.Controllers
{
    [Route("api/v1/comments")]
    [ApiController]
    public class CommentsV1Controller : ControllerBase
    {
        private IDataAccess _dataAccess;
        private UserManager<IdentityUser> _userManager;

        public CommentsV1Controller(IDataAccess dataAccess, UserManager<IdentityUser> userManager)
        {
            _dataAccess = dataAccess;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InsertComment(Comment comment)
        {
            if (comment.Email != User.Identity.Name)
                return ValidationProblem();
            var userId = User.FindFirst("userId");
            comment.Author = userId.Value;
            comment.InsertDate = DateTime.UtcNow;
            var id = await _dataAccess.InsertCommentAsync(comment);
            return Ok(id);
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var comment = await _dataAccess.GetCommentAsync(commentId);
            if (comment == null)
                return NotFound();
            if (comment.Email != User.Identity.Name)
                return Forbid();
            //var user = await _userManager.FindByEmailAsync(comment.Email);

            await _dataAccess.DeleteCommentAsync(commentId);
            return Ok();
        }

        [Authorize]
        [HttpPatch("{commentId}")]
        //I made the class test because with it the binding works, i have tried many things but none of them have worked
        //for example (int commentId, [FromBody] string textBody)
        public async Task<IActionResult> UpdateComment(int commentId, ThisShouldNotExist textBody)
        {
            var comment = await _dataAccess.GetCommentAsync(commentId);
            if (comment == null)
                return NotFound();
            if (comment.Email != User.Identity.Name)
                return Forbid();
            if (textBody == null)
                return BadRequest();
            
            comment.Text = textBody.TextBody;

            await _dataAccess.UpdateCommentAsync(comment);
            return NoContent();
        }
    }
}