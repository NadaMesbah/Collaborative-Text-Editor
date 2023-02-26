namespace RealTimeCollaborativeApp.Hubs
{
    public class HubConnections
    {
        public static Dictionary<string, List<string>> Users = new();
        //will tell us whether the connection of that user exists in the dictionary or not
        public static bool HasUserConnection(string UsedId, string ConnectionId)
        {
            try
            {
                if (Users.ContainsKey(UsedId))
                {
                    return Users[UsedId].Any(p => p.Contains(ConnectionId));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public static bool HasUser(string UsedId)
        {
            try
            {
                if (Users.ContainsKey(UsedId))
                {
                    return Users[UsedId].Any();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        public static void AddUserConnection(string UserId, string ConnectionId)
        {
            if (!string.IsNullOrEmpty(UserId) && !HasUserConnection(UserId, ConnectionId))
            {
                if (Users.ContainsKey(UserId))
                    Users[UserId].Add(ConnectionId);
                else
                    Users.Add(UserId, new List<string> { ConnectionId });
            }

        }
        public static List<string> OnlineUsers()
        {
            //fetch the userId of the logged in users
            return Users.Keys.ToList();
        }
    }
}
