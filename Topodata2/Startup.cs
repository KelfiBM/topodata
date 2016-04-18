using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Topodata2.Startup))]
namespace Topodata2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
