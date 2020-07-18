using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TravelExperts_Web_App.Startup))]
namespace TravelExperts_Web_App
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
