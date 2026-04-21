using Volo.Abp.Modularity;

namespace MyCompany.MyApp;

[DependsOn(
    typeof(MyAppApplicationModule),
    typeof(MyAppDomainTestModule)
)]
public class MyAppApplicationTestModule : AbpModule
{

}
