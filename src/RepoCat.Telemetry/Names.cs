// -----------------------------------------------------------------------
//  <copyright file="Names.cs" company="bartosz.jarmuz@gmail.com">
//   Copyright (c) Bartosz Jarmuż. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

namespace RepoCat.Telemetry
{
    public static class Names
    {
        public static string RecurringJobScheduled => "RecurringJobScheduled";
        public static string RecurringJobStarted => "RecurringJobStarted";
        public static string RecurringJobFinished => "RecurringJobFinished";
        public static string ViewRepository => "ViewRepository";
        public static string DeleteRepository => "DeleteRepository";
        public static string PostingProjectInfo => "PostingProjectInfo";
        public static string UpsertedRepository => "UpsertedRepository";
        public static string UpsertedProject => "UpsertedProject";
        public static string FileDownload => "FileDownload";
        public static string RepositorySearch => "RepositorySearch";
        public static string RepositoryBatchCleanupJobFinished => "RepositoryBatchCleanupJobFinished";
        public static string RepositoryCleanup => "RepositoryCleanup";
    }
}
