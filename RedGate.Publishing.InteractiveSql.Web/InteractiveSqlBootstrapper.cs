using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Responses;
using Nancy.ViewEngines;
using RedGate.Publishing.InteractiveSql.Courses;
using RedGate.Publishing.Logging;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class InteractiveSqlBootstrapper : ExplicitModuleBootstrapper
    {
        private static bool s_ViewSetupCompleted = false;

        public static INancyBootstrapper Create()
        {
            return new InteractiveSqlBootstrapper();
        }

        private static IList<Type> ModuleTypes()
        {
            return new[]
                {
                    typeof(LessonModule),
                    typeof(QueryModule),
                    typeof(AboutModule)
                };
        }

        private static string FindAssemblyDirectory()
        {
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        private readonly string m_RootDirectory;

        public InteractiveSqlBootstrapper() : this(FindAssemblyDirectory())
        {
        }

        public InteractiveSqlBootstrapper(string rootDirectory) : base(ModuleTypes())
        {
            m_RootDirectory = rootDirectory;
        }

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, IPipelines pipelines)
        {
            container.Resolve<InteractiveSqlApplication>().Start();
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("static", Path.Combine(m_RootDirectory, "static"))
            );
        }

        protected override void ConfigureApplicationContainer(Nancy.TinyIoc.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register(SqlServerConfiguration.ReadAppSettings());
            var sqlExecutor = container.Resolve<SqlExecutor>();
            container.Register(SqlSelectCourse.Load(sqlExecutor));
            container.Register<ILogger>(Logger.CreateFromConfiguration());
            

            var assembly = GetType().Assembly;
            if (!s_ViewSetupCompleted)
            {
                ResourceViewLocationProvider
                    .RootNamespaces
                    .Add(assembly, "RedGate.Publishing.InteractiveSql.Web.Views");
                s_ViewSetupCompleted = true;
            }
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                return NancyInternalConfiguration.WithOverrides(
                    config => config.ViewLocationProvider = typeof (ResourceViewLocationProvider)
                );
            }
        }

        protected override byte[] FavIcon
        {
            get { return null; }
        }

        protected override Nancy.IRootPathProvider RootPathProvider
        {
            get { return new ConstantRootPathProvider(m_RootDirectory); }
        }

    }

    public class ConstantRootPathProvider : IRootPathProvider
    {
        private readonly string m_RootDirectory;

        public ConstantRootPathProvider(string rootDirectory)
        {
            m_RootDirectory = rootDirectory;
        }

        public string GetRootPath()
        {
            return m_RootDirectory;
        }
    }
}
