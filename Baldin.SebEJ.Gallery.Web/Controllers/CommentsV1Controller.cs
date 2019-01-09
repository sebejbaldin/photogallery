﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baldin.SebEJ.Gallery.Data;
using Baldin.SebEJ.Gallery.Data.Models;
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

        [HttpPost]
        public async Task<IActionResult> InsertComment(Comment comment)
        {
            if (comment.Email != User.Identity.Name)
                return ValidationProblem();
            var user = await _userManager.FindByEmailAsync(comment.Email);
            comment.Author = user.Id;
            comment.InsertDate = DateTime.UtcNow;
            _dataAccess.InsertComment(comment);
            return Ok();
        }
    }
}