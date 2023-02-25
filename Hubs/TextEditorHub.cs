using RealTimeCollaborativeApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace RealTimeCollaborativeApp.Hubs
{
    public class TextEditorHub : Hub
    {
        public void JoinGroup(string groupName)
        {
            this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        }
        public void LeaveGroup(string groupName)
        {
            this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
        }
        public async Task SendMessage(string content)
        {
            Console.WriteLine(content);
            await Clients.OthersInGroup("global").SendAsync("updateDocument", content);
        }
        public async Task CursorSend(string range,string cursor)
        {
            Console.WriteLine(range);
            await Clients.OthersInGroup("global").SendAsync("cursorReceive", range,cursor);
        }
    }
}
