using System;

namespace RedGate.Publishing.InteractiveSql.SelfHosted
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new SelfHostedWebApp(59911);
            app.Start();
            Console.WriteLine("Hosting on " + app.HostAddress);
            Console.WriteLine("Hit enter to stop the server");
            Console.ReadLine();
            app.Stop();
        }
    }
}
