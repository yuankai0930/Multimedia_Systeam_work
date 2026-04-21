using MyCompany.MyApp.Localization;
using Volo.Abp.Application.Services;

namespace MyCompany.MyApp;

/* Inherit your application services from this class.
 */
public abstract class MyAppAppService : ApplicationService
{
    protected MyAppAppService()
    {
        LocalizationResource = typeof(MyAppResource);
    }
}
