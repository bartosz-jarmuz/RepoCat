// -----------------------------------------------------------------------
//  <copyright file="InputUriProviderBase.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace RepoCat.Transmission
{
    public abstract class InputUriProviderBase : IInputUriProvider
    {
        private readonly ILogger logger;

        protected InputUriProviderBase(ILogger logger)
        {
            this.logger = logger;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "It's a suffix")]
        protected abstract string InputUriSuffix { get; }

        public virtual IEnumerable<string> GetUris(string rootUri,  Regex ignoredPathsRegex = null)
        {
            this.CheckIfCanAccesDirectory(rootUri, FileSystemRights.Read);

            var root = new DirectoryInfo(rootUri);
            if (root.Exists)
            {
                var paths = root.EnumerateFiles("*", SearchOption.AllDirectories)
                    .Where(x => x.FullName.EndsWith(this.InputUriSuffix, StringComparison.OrdinalIgnoreCase))
                    .Select(x => x.FullName);
                if (ignoredPathsRegex != null)
                {
                    this.logger.Debug($"Adding regex to exclude paths [{ignoredPathsRegex}]");
                    return paths.Where(path => !ignoredPathsRegex.IsMatch(path));
                }

                return paths;
            }
            return Array.Empty<string>();
        }

        public virtual bool CheckIfCanAccesDirectory(string path, FileSystemRights rights)
        {
            if (string.IsNullOrEmpty(path)) return false;

            try
            {
                AuthorizationRuleCollection rules = System.IO.FileSystemAclExtensions.GetAccessControl(new DirectoryInfo(path))
                    .GetAccessRules(true, true, typeof(System.Security.Principal.SecurityIdentifier));
                WindowsIdentity identity = WindowsIdentity.GetCurrent();

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (identity.Groups.Contains(rule.IdentityReference))
                    {
                        if ((rights & rule.FileSystemRights) == rights)
                        {
                            if (rule.AccessControlType == AccessControlType.Allow)
                            {
                                this.logger.Debug($"Confirmed access rights to [{path}]");
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.Error($"Cannot access directory [{path}]", ex);
                throw;
            }
            this.logger.Error($"No permissions to directory [{path}]");
            throw new UnauthorizedAccessException($"No permissions to directory [{path}]");
        }
    }
}