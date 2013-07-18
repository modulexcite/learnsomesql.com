using Nancy;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class AboutModule : NancyModule
    {
        public AboutModule()
        {
            Get["/about"] = parameters => View["about.html"];
        }
    }
}
