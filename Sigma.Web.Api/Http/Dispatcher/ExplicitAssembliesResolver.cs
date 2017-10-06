namespace Sigma.Web.Http.Dispatcher
{
    using System.Web.Http.Dispatcher;
    using System.Collections.Generic;
    using System.Reflection;

    public class ExplicitAssembliesResolver : DefaultAssembliesResolver
    {
        private readonly Assembly[] assemblies;

        public ExplicitAssembliesResolver(params Assembly[] assemblies)
        {
            this.assemblies = assemblies;
        }

        public override ICollection<Assembly> GetAssemblies()
        {
            return this.assemblies;
        }
    }
}