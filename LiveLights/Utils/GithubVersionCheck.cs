using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveLights.Utils
{
    using Octokit;

    internal class GithubVersionCheck
    {
        public GitHubClient Client { get; } 
        public string Owner { get; }
        public string Project { get; }
        public int BuildReleaseId { get; }
        public Release LatestRelease { get; }
        
        private Release[] allReleases = null;
        public Release[] AllReleases 
        { 
            get
            {
                // Only call the all releases API if needed
                if(allReleases == null)
                {
                    try
                    {
                        allReleases = Task.Run(async () => await Client.Repository.Release.GetAll(Owner, Project)).Result.ToArray();
                    } catch (Exception e)
                    {
                        Rage.Game.LogTrivial($"Unable to check release history on GitHub: {e.Message}");
                        allReleases = new Release[] { };
                    }
                }
                return allReleases;
            }
        }

        public Release BuildRelease
        {
            get
            {
                return AllReleases.FirstOrDefault(r => r.Id == BuildReleaseId);
            }
        }

        public GithubVersionCheck(string owner, string project, int buildReleaseId)
        {
            this.Client = new GitHubClient(new ProductHeaderValue("simple-version-check"));
            this.Owner = owner;
            this.Project = project;
            this.BuildReleaseId = buildReleaseId;
            try
            {
                LatestRelease = Task.Run(async () => await Client.Repository.Release.GetLatest(Owner, Project)).Result;
            } catch (Exception e)
            {
                LatestRelease = AllReleases.FirstOrDefault();
                Rage.Game.LogTrivialDebug($"Unable to check latest release on GitHub: {e.Message}");
                if (LatestRelease == null)
                {
                    Rage.Game.LogTrivial("Full release list unsuccessful");
                } else
                {
                    Rage.Game.LogTrivial("Retrieved full release list");
                }
            }
        }

        public bool IsUpdateAvailable()
        {
            if (LatestRelease?.Id == BuildReleaseId || BuildRelease == null)
            {
                return false;
            }

            return true;
        }
    }
}
