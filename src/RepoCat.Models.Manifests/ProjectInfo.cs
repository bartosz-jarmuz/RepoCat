using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
{
    public class ProjectInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectUri { get; set; }
        public string RepositoryName { get; set; }
        /// <summary>
        /// A datetime or version stamp of the repository as of when the project info was read and transmitted
        /// </summary>
        public string RepositoryStamp { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
        public string AssemblyName { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }
        public List<ComponentManifest> Components { get; set; } = new List<ComponentManifest>();
    }
}