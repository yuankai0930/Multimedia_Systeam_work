using System.Threading.Tasks;

namespace MyCompany.MyApp.Data;

public interface IMyAppDbSchemaMigrator
{
    Task MigrateAsync();
}
