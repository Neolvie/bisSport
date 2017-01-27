using System.Configuration;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(bisSport.WebClient.Startup))]
namespace bisSport.WebClient
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      ConfigureAuth(app);

      Server.Repository.UnitOfWork.Initialize(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
    }
  }
}
