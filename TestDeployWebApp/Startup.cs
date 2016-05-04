using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestDeployWebApp.Startup))]
namespace TestDeployWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
