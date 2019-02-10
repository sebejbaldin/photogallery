using Baldin.SebEJ.Gallery.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Data
{
    public interface IDataAccess
    {
        Picture GetPicture(int Id);
        Picture GetPictureByUrl(string url);
        IEnumerable<Picture> GetPicturesRangeById(int startId, int endId);
        IEnumerable<Picture> GetPictures();
        int InsertPicture(Picture picture);
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
        int InsertComment(Comment comment);
        bool UpdateComment(Comment comment);
        bool DeleteComment(int Id);

        Task<Picture> GetPictureAsync(int Id);
        Task<Picture> GetPictureByUrlAsync(string url);
        Task<IEnumerable<Picture>> GetPicturesRangeByIdAsync(int startId, int endId);
        Task<IEnumerable<Picture>> GetPicturesAsync();
        Task<int> InsertPictureAsync(Picture picture);
        Task<bool> UpdatePictureAsync(Picture picture);
        Task<bool> DeletePictureAsync(int Id);

        Task<IEnumerable<Vote>> GetVotesAsync();
        Task<IEnumerable<Vote>> GetVotesByUserIdAsync(string userId);
        Task<bool> InsertVoteAsync(Vote vote);
        Task<bool> UpdateVoteAsync(Vote vote);
        Task<bool> DeleteVoteAsync(int Id);

        Task<Comment> GetCommentAsync(int Id);
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<IEnumerable<Comment>> GetCommentsByPhotoIdAsync(int photoId);
        Task<int> InsertCommentAsync(Comment comment);
        Task<bool> UpdateCommentAsync(Comment comment);
        Task<bool> DeleteCommentAsync(int Id);
    }
}
