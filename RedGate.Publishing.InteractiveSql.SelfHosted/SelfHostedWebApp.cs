using System;
using System.Net;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using Nancy.Responses;
using RedGate.Publishing.InteractiveSql.Web;

namespace RedGate.Publishing.InteractiveSql.SelfHosted
{
    public class SelfHostedWebApp : IDisposable
    {
        private readonly int m_Port;
        private NancyHost m_NancyHost;

        public SelfHostedWebApp(int port)
        {
            m_Port = port;
        }

        public string HostAddress { get { return string.Format("http://localhost:{0}", m_Port); } }

        public void Start()
        {
            var nancyBootstrapper = InteractiveSqlBootstrapper.Create();
            try
            {
                RunServer(nancyBootstrapper, new HostConfiguration { RewriteLocalhost = true });
            }
            catch (AutomaticUrlReservationCreationFailureException)
            {
                RunServer(nancyBootstrapper, new HostConfiguration {RewriteLocalhost = false});
            }
        }

        private void RunServer(INancyBootstrapper nancyBootstrapper, HostConfiguration hostConfiguration)
        {
            var hostAddress = new Uri(HostAddress);
            m_NancyHost = new NancyHost(
                hostAddress,
                nancyBootstrapper,
                hostConfiguration
                );
            m_NancyHost.Start();
        }

        public void Stop()
        {
            if (m_NancyHost != null)
            {
                m_NancyHost.Stop();
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
