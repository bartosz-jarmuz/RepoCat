// -----------------------------------------------------------------------
//  <copyright file="InputUriProviderBase.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission
{
    public abstract class InputUriProviderBase : IInputUriProvider
    {
        private readonly ILogger logger;

        protected InputUriProviderBase(ILogger logger)
        {
            this.logger = logger;
        }

        [SuppressMessage("Design", "CA1056:Uri properties should not be strings", Justification = "It's a suffix")]
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
            try
            {
                var control = FileSystemAclExtensions.GetAccessControl(new DirectoryInfo(path));
                var securityIdentifierRules = control.GetAccessRules(true, true, typeof(SecurityIdentifier));
                var nTAccountRules = control.GetAccessRules(true, true, typeof(NTAccount));
                
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                if (identity.Groups == null)
                {
                    this.logger.Warn("Identity groups are null. Cannot check folder access. Access is undetermined, it might or might not work. ");
                    return true;
                }
                foreach (FileSystemAccessRule rule in nTAccountRules)
                {
                    if (rule.IdentityReference.Value == identity.Name)
                    {
                        if (this.RuleAllowsAccess(path, rights, rule)) return true;
                    }
                }
                foreach (FileSystemAccessRule rule in securityIdentifierRules)
                {
                    if (identity.Groups.Contains(rule.IdentityReference))
                    {
                        if (this.RuleAllowsAccess(path, rights, rule)) return true;
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

        private bool RuleAllowsAccess(string path, FileSystemRights rights, FileSystemAccessRule rule)
        {
            if ((rights & rule.FileSystemRights) == rights)
            {
                if (rule.AccessControlType == AccessControlType.Allow)
                {
                    this.logger.Debug($"Confirmed access rights to [{path}]");
                    return true;
                }
            }

            return false;
        }
    }
}