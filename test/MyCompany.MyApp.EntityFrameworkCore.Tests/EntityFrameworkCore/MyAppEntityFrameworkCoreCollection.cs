using Xunit;

namespace MyCompany.MyApp.EntityFrameworkCore;

[CollectionDefinition(MyAppTestConsts.CollectionDefinitionName)]
public class MyAppEntityFrameworkCoreCollection : ICollectionFixture<MyAppEntityFrameworkCoreFixture>
{

}
