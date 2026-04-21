using Volo.Abp.Modularity;

namespace MyCompany.MyApp;

public abstract class MyAppApplicationTestBase<TStartupModule> : MyAppTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
