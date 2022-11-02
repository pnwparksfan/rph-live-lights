using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace LiveLights.Utils
{
    using Octokit;

    internal class GithubVersionCheck
    {
        public GitHubClient Client { get; } 
        public string Owner { get; }
        public string Project { get; }
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

        public Version CurrentVersion { get; }
        public Version LatestVersion { get; }
        public bool UpdateAvailable { get; }

        public GithubVersionCheck(string owner, string project) : this(owner, project, Assembly.GetExecutingAssembly().GetName().Version)
        {

        }

        public GithubVersionCheck(string owner, string project, Version currentVersion)
        {
            this.Client = new GitHubClient(new ProductHeaderValue("simple-version-check"));
            this.Owner = owner;
            this.Project = project;
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

            CurrentVersion = currentVersion;
            LatestVersion = GetGithubVersion(LatestRelease);

            UpdateAvailable = (CurrentVersion != null && LatestVersion != null && LatestVersion > CurrentVersion);
        }

        private Version GetGithubVersion(Release release)
        {
            if (release != null)
            {
                var match = new Regex(@"\d+\.\d+(?:\.\d+)*").Match(release.TagName);
                if (match.Success)
                {
                    return new Version(match.Value);
                }
                else
                {
                    Rage.Game.LogTrivial($"Unable to parse GitHub version from tag {LatestRelease.TagName}");
                }
            } else
            {
                Rage.Game.LogTrivial("Unable to retrieve version from GitHub");
            }

            return null;
        }
    }
}
