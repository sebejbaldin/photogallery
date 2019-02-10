using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baldin.SebEJ.Gallery.Web.Hubs
{
    public class GalleryHub : Hub
    {
        public async Task SendComment(string username, string text, DateTime insertDate, string groupName)
        {
            await Clients.Group(groupName).SendAsync("ReceiveComment", username, text, insertDate.ToString("dd/MM/yyyy HH:mm"));
        }

        public async Task DeleteComment(int commentId, string groupName)
        {
            await Clients.Group(groupName).SendAsync("EraseComment", commentId);
        }

        public async Task UpdateComment(int commentId, string body, string groupName)
        {
            await Clients.Group(groupName).SendAsync("ModifyComment", commentId, body);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}
