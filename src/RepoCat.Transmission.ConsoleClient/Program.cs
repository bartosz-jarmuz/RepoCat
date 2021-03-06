﻿// -----------------------------------------------------------------------
//  <copyright file="Program.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using log4net;
using RepoCat.Transmission.Contracts;

namespace RepoCat.Transmission.ConsoleClient
{
#pragma warning disable S1118 // Utility classes should not have public constructors
    class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        static void Main(string[] args)
        {
            var log = LogManager.GetLogger(typeof(Program));

            try
            {
                log.Info($"Console transmitter [{GetAssemblyFileVersion()}] starting...");
                ILogger logAdapter = new Log4NetAdapter(log);
                using (var sender = new HttpProjectInfoSender(logAdapter))
                {
                    Transmitter client = new Transmitter(logAdapter,sender );
                    client.Work(args).GetAwaiter().GetResult();
                }
                log.Info($"{typeof(Program).Assembly.GetName().Name} - Finished");
            }
            catch (Exception ex)
            {
                log.Fatal("Something went wrong", ex);
                throw;
            }
        }
        
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Doesn't matter what happens, just don't throw.")]
        private static string GetAssemblyFileVersion()
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(typeof(Program).Assembly.Location).FileVersion;
            }
            catch
            {
                return "0.0.0.1";
            }
        }
    }
}
