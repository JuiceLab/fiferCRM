using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(fifer_crm.Startup))]
namespace fifer_crm
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
