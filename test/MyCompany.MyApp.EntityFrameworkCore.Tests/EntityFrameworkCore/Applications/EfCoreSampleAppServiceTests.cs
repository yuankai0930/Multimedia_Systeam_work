using MyCompany.MyApp.Samples;
using Xunit;

namespace MyCompany.MyApp.EntityFrameworkCore.Applications;

[Collection(MyAppTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<MyAppEntityFrameworkCoreTestModule>
{

}
