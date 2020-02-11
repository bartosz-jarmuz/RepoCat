// -----------------------------------------------------------------------
//  <copyright file="TestHelpers.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.CompilerServices;
using RepoCat.Persistence.Service;

namespace Repocat.Persistence.Tests
{
    public static class TestHelpers
    {
        public static IRepoCatDbSettings GetSettings()
        {
            return new RepoCatDbSettings()
            {
                ProjectsCollectionName = "Projects",
                SearchStatisticsCollectionName = "SearchStatistics",
                RepositoriesCollectionName = "Repositories",
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "RepoCatDbTESTS"
            };
        }
        public static string GetMethodName([CallerMemberName] string caller = "")
        {
            StackTrace stackTrace = new StackTrace();
            var frames = stackTrace.GetFrames();
            for (int index = 1; index < frames.Length; index++)
            {
                StackFrame stackFrame = frames[index];
                var methodBase = stackFrame.GetMethod();
                if (methodBase.Name == "MoveNext" 
                    || methodBase.Name == "Start"
                    )

                {
                    continue;
                }
                if (methodBase.DeclaringType != null && !
                        (methodBase.DeclaringType.Assembly.FullName.StartsWith("mscorlib")
                         || methodBase.DeclaringType.Assembly.FullName.StartsWith("System.Net.Http")
                         || methodBase.DeclaringType.Assembly.FullName.StartsWith("System, ")
                        ))
                {
                    return methodBase.Name;
                }
            }

            return caller;
        }
    }
}