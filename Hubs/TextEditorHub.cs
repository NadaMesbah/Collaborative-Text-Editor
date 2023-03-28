﻿using RealTimeCollaborativeApp.Models;
using Microsoft.AspNetCore.SignalR;
using RealTimeCollaborativeApp.Data;
using System.Security.Claims;

namespace RealTimeCollaborativeApp.Hubs
{
    public class TextEditorHub : Hub
    {
        //public static List<string> GroupsJoined { get; set; } = new List<string>();

        private readonly ApplicationDbContext _db;
        public TextEditorHub(ApplicationDbContext db)
        {
            //dependency injection : Constructor Injection
            _db = db;
        }

        public void JoinGroup(string groupName)
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!String.IsNullOrEmpty(UserId))
            {
                var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);
                Clients.OthersInGroup(groupName).SendAsync("newMemberJoined", UserId, userName, groupName);
                this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
            }
        }
        public void LeaveGroup(string groupName)
        {
            var UserId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (HubConnections.HasUserConnection(UserId, Context.ConnectionId))
            {
                var UserConnections = HubConnections.Users[UserId];
                UserConnections.Remove(Context.ConnectionId);

                HubConnections.Users.Remove(UserId);
                if (UserConnections.Any())
                    HubConnections.Users.Add(UserId, UserConnections);
            }

            if (!String.IsNullOrEmpty(UserId))
            {
                var userName = _db.Users.FirstOrDefault(u => u.Id == UserId).UserName;
                HubConnections.AddUserConnection(UserId, Context.ConnectionId);
                Clients.OthersInGroup(groupName).SendAsync("newMemberLeft", UserId, userName, groupName);
                this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
            }
        }

        public async Task SendMessage(string content)
        {
            Console.WriteLine(content);
            await Clients.OthersInGroup("global").SendAsync("updateDocument", content);
        }
        public async Task CursorSend(string range, string cursor)
        {
            Console.WriteLine(range);
            await Clients.OthersInGroup("global").SendAsync("cursorReceive", range, cursor);
        }
    }
}


