using System;
using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.Bootstrapper;

namespace RedGate.Publishing.InteractiveSql.Web
{
    public class ExplicitModuleBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IList<Type> m_ModuleTypes;

        public ExplicitModuleBootstrapper(IList<Type> moduleTypes)
        {
            m_ModuleTypes = moduleTypes;
        }

        protected override IEnumerable<ModuleRegistration> Modules
        {
            get
            {
                return m_ModuleTypes
                    .Select(ModuleRegistration)
                    .ToList();
            }
        }

        private ModuleRegistration ModuleRegistration(Type moduleType)
        {
            return new ModuleRegistration(moduleType);
        }
    }
}
