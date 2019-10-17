using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkV3.Startup))]
namespace WorkV3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR();
        }
    }
}
