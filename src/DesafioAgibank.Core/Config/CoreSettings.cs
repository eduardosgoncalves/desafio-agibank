namespace DesafioAgibank.Core.Config
{
    public class CoreSettings
    {
        public string EnvironmentPath { get; set; }
        public DataInSettings DataIn { get; set; }
        public DataOutSettings DataOut { get; set; }

        public CoreSettings()
        {
            DataIn = new DataInSettings();
            DataOut = new DataOutSettings();
        }
    }
}