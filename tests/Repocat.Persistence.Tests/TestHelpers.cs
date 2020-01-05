// -----------------------------------------------------------------------
//  <copyright file="TestHelpers.cs" company="SDL plc">
//   Copyright (c) SDL plc. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Repocat.Persistence.Tests
{
    public static class TestHelpers
    {

        public static string GetMethodName([CallerMemberName] string caller = "")
        {
            StackTrace stackTrace = new System.Diagnostics.StackTrace();
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