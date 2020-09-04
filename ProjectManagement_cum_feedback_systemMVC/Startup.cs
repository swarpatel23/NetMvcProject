using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ProjectManagement_cum_feedback_systemMVC.Startup))]
namespace ProjectManagement_cum_feedback_systemMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // ------------------------------
            app.MapSignalR();
            // ------------------------------
        }
    }
}
