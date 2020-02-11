// -----------------------------------------------------------------------
//  <copyright file="ExcelBasedUriProvider.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission.Builders.Excel
{


    /// <summary>
    /// Class LocalProjectUriProvider.
    /// </summary>
    /// <seealso cref="IInputUriProvider" />
    public class ExcelBasedUriProvider : IInputUriProvider
    {
        private readonly ILogger logger;

        public ExcelBasedUriProvider(ILogger logger)
        {
            this.logger = logger;
        }

        public IEnumerable<string> GetUris(string rootUri, Regex ignoredPathsRegex = null)
        {
            this.logger.Info($"Getting URIs from {rootUri}");
            if (IsExcelChecker.IsExcelFile(rootUri))
            {
                yield return rootUri;
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(rootUri);
                IEnumerable<FileInfo> query = directory.EnumerateFiles("*.*", SearchOption.AllDirectories);
                if (ignoredPathsRegex != null)
                {
                    query = query.Where(x => !ignoredPathsRegex.IsMatch(x.FullName));
                }
                foreach (var file in query.Where(x => IsExcelChecker.IsExcelFile(x.FullName)))
                {
                    yield return file.FullName;
                }

            }
        }
    }
}