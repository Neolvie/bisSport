using System.Web.Mvc;

namespace bisSport.WebClient.Areas.Football
{
    public class FootballAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Football";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Football_default",
                "Football/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}