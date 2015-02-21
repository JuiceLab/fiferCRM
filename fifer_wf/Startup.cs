using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(fifer_wf.Startup))]
namespace fifer_wf
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
