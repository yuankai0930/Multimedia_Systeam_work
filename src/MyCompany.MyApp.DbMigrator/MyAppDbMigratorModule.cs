using MyCompany.MyApp.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace MyCompany.MyApp.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(MyAppEntityFrameworkCoreModule),
    typeof(MyAppApplicationContractsModule)
)]
public class MyAppDbMigratorModule : AbpModule
{
}
