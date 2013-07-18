using System;
using System.Web;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Hosting.Aspnet;
using RedGate.Publishing.InteractiveSql.Web;
using RedGate.Publishing.Logging;

namespace RedGate.Publishing.InteractiveSql.AspNet
{
    public class InteractiveSqlHttpRequestHandler : IHttpHandler
    {
        private static INancyEngine engine;

        public bool IsReusable
        {
            get { return true; }
        }

        static InteractiveSqlHttpRequestHandler()
        {
            Try(() =>
            {
                var bootstrapper = GetBootstrapper();

                bootstrapper.Initialise();

                engine = bootstrapper.GetEngine();

            });
        }

        private static INancyBootstrapper GetBootstrapper()
        {
            return new InteractiveSqlBootstrapper();
        }

        public void ProcessRequest(HttpContext context)
        {
            Try(() =>
            {
                var wrappedContext = new HttpContextWrapper(context);
                var handler = new NancyHandler(engine);
                handler.ProcessRequest(wrappedContext);
            });
        }

        private static void Try(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                CreateLogger().LogException(Severity.Fatal, exception);
                throw;
            }
        }

        private static ILogger CreateLogger()
        {
            var logger = Logger.CreateFromConfiguration();
            var context = HttpContext.Current;
            if (context == null)
            {
                return logger;
            }
            else {
                return new LoggerWithHttpRequest(logger, context.Request);
            }
        }
    }
}