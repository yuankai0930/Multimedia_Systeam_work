using Microsoft.Extensions.Localization;
using MyCompany.MyApp.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace MyCompany.MyApp;

[Dependency(ReplaceServices = true)]
public class MyAppBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<MyAppResource> _localizer;

    public MyAppBrandingProvider(IStringLocalizer<MyAppResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
