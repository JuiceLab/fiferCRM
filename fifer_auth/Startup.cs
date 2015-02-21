using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(fifer_auth.Startup))]
namespace fifer_auth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}
