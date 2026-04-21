using MyCompany.MyApp.Samples;
using Xunit;

namespace MyCompany.MyApp.EntityFrameworkCore.Domains;

[Collection(MyAppTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MyAppEntityFrameworkCoreTestModule>
{

}
