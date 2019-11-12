using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace ProjectManagement_cum_feedback_systemMVC
{
    public class ChatHub : Hub
    {
        public void Send(string name,string projname, string message)
        {
            // Call the addNewMessageToPage method to update clients.

            Clients.All.addNewMessageToPage(name,projname, message);
        }
    }
}