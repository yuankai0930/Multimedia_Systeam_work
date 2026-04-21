using Volo.Abp.Modularity;

namespace MyCompany.MyApp;

[DependsOn(
    typeof(MyAppDomainModule),
    typeof(MyAppTestBaseModule)
)]
public class MyAppDomainTestModule : AbpModule
{

}
