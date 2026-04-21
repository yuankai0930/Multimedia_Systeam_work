using Volo.Abp.Modularity;

namespace MyCompany.MyApp;

/* Inherit from this class for your domain layer tests. */
public abstract class MyAppDomainTestBase<TStartupModule> : MyAppTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
