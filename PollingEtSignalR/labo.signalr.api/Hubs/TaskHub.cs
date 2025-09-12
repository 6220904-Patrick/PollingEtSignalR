using labo.signalr.api.Data;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;

namespace labo.signalr.api.Hubs
{
    public static class UserHandler
    {
        public static HashSet<string> ConnectedIds = new HashSet<string>();
    }

    public class TaskHub: Hub
    {
        ApplicationDbContext _context;

        public TaskHub(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task TaskList(int value)
        {
            
            await Clients.Caller.SendAsync("TaskList", value);
        }
    }
}
