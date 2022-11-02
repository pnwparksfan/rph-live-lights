using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Reflection;

namespace LiveLights.Utils
{
    internal class LCPDFRVersionCheck
    {
        public int FileID { get; }
        public Version LatestVersion { get; }
        public Version CurrentVersion { get; }
        public bool UpdateAvailable { get; }

        public LCPDFRVersionCheck(int fileId, Version currentVersion)
        {
            this.FileID = fileId;
            this.CurrentVersion = currentVersion;

            LatestVersion = Task.Run(GetLatestVersion).Result;

            UpdateAvailable = (CurrentVersion != null && LatestVersion != null && LatestVersion > CurrentVersion);
        }

        public LCPDFRVersionCheck(int fileId) : this(fileId, Assembly.GetExecutingAssembly().GetName().Version)
        {
        }

        private async Task<Version> GetLatestVersion()
        {
            try
            {
                HttpClient client = new HttpClient();
                string versionResponse = await client.GetStringAsync($"https://www.lcpdfr.com/applications/downloadsng/interface/api.php?do=checkForUpdates&fileId={FileID}&textOnly=true");
                return new Version(versionResponse);
            } catch (Exception e)
            {
                Rage.Game.LogTrivial($"Unable to fetch latest version from lcpdfr.com: {e.Message}");
                return null;
            }
        }
    }
}
