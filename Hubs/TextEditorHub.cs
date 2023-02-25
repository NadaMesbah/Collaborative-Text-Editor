using RealTimeCollaborativeApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace RealTimeCollaborativeApp.Hubs
{
    public class TextEditorHub : Hub
    {

        //public async Task LoadDocument(string id)
        //{
        //    //Doc currentDoc = Doc.getDoc(id);
        //    DocumentDao documentDao = new DocumentDaoMemory();
        //    Document currentDoc = documentDao.GetById(id);

        //    await Groups.AddToGroupAsync(Context.ConnectionId, id);
        //    await Clients.Client(Context.ConnectionId).SendAsync("loadDocument", currentDoc.content);
        //}
        //public async Task SaveDoc(string content , string id)
        //{
        //    DocumentDao documentDao = new DocumentDaoMemory();
        //    Document currentDoc = documentDao.update(id);
        //    //Doc doc = Doc.getDoc(id);
        //    //doc.content = content;
        //    await Clients.Client(Context.ConnectionId).SendAsync("loadDocument", "save docs trigered");


        //}
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
