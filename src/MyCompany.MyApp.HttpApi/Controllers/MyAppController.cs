using MyCompany.MyApp.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace MyCompany.MyApp.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class MyAppController : AbpControllerBase
{
    protected MyAppController()
    {
        LocalizationResource = typeof(MyAppResource);
    }
}
