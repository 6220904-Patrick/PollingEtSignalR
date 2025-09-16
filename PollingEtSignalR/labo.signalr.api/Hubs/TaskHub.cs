﻿using labo.signalr.api.Data;
using labo.signalr.api.Models;
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

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            UserHandler.ConnectedIds.Add(Context.ConnectionId);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            await Clients.Caller.SendAsync("TaskList", _context.UselessTasks.ToList());
        }

        public async Task TaskList()
        {
            List<UselessTask> tasks = _context.UselessTasks.ToList();
            await Clients.Caller.SendAsync("TaskList", tasks);
        }

        public async Task AddTask(string taskName)
        {
            UselessTask task = new UselessTask();
            task.Text = taskName;
            _context.UselessTasks.Add(task);
            _context.SaveChanges();
            await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToList());
        }

        public async Task CompleteTask(int id)
        {
            UselessTask? task = await _context.FindAsync<UselessTask>(id);
            if (task != null)
            {
                task.Completed = true;
                await _context.SaveChangesAsync();
                await Clients.All.SendAsync("TaskList", _context.UselessTasks.ToList());
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            UserHandler.ConnectedIds.Remove(Context.ConnectionId);
            await Clients.All.SendAsync("UserCount", UserHandler.ConnectedIds.Count);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
