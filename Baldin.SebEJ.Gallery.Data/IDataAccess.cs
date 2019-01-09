using Baldin.SebEJ.Gallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baldin.SebEJ.Gallery.Data
{
    public interface IDataAccess
    {
        Picture GetPicture(int Id);
        IEnumerable<Picture> GetPictures();
        bool InsertPicture(Picture picture);
        bool UpdatePicture(Picture picture);
        bool DeletePicture(int Id);

        IEnumerable<Vote> GetVotes();
        IEnumerable<Vote> GetVotesByUserId(string userId);
        bool InsertVote(Vote vote);
        bool UpdateVote(Vote vote);
        bool DeleteVote(int Id);

        Comment GetComment(int Id);
        IEnumerable<Comment> GetComments();
        IEnumerable<Comment> GetCommentsByPhotoId(int photoId);
        bool InsertComment(Comment comment);
        bool UpdateComment(Comment comment);
        bool DeleteComment(int Id);
    }
}
