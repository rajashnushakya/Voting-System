using DbUp;
using DbUp.Engine;
using DbUp.ScriptProviders;

namespace VotingAPI.Service
{
    public class DbMaintainer
    {
        private static string _scriptPath;
        static DbMaintainer()
        {
            _scriptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migration");
        }

        public static DatabaseUpgradeResult? Maintain(string connectionString)
        {
            try
            {
                var upgrader = DeployChanges.To
            .SqlDatabase(connectionString)
            .WithScriptsFromFileSystem(_scriptPath)
            .LogToConsole()
            .Build();

                if (upgrader.IsUpgradeRequired())
                {
                    var result = upgrader.PerformUpgrade();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
