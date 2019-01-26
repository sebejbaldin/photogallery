﻿using Baldin.SebEJ.Gallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Caching
{
    public interface ICaching
    {
        Task<IEnumerable<Picture>> GetPhotosAsync();
        Task<Picture> GetPhotoAsync(int Id);
        Task<bool> InsertPhotoAsync(Picture picture);
        Task<bool> InsertPhotosAsync(IEnumerable<Picture> pictures);

        Task<IEnumerable<Vote>> GetVotesAsync();
        Task<IEnumerable<Vote>> GetVotesByUserId(string userId);
        Task<Vote> GetVoteAsync(int Id);
        Task<bool> InsertVoteAsync(Vote vote);
        Task<bool> InsertVotesAsync(IEnumerable<Vote> votes);

        Task<IEnumerable<Picture>> GetRank(int topN = -1);

        bool IsConnected();
        //Task<IEnumerable<Comment>> GetCommentsAsync();
        //Task<Comment> GetCommentAsync();
        //Task<bool> InsertCommentAsync(Comment comment);
        //Task<bool> InsertCommentsAsync(IEnumerable<Comment> comments);
    }
}
