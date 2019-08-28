using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Portal.Models
{
    public class ProjectManifest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Repo { get; set; }
        public string RepoStamp { get; set; }
        public DateTime AddedDateTime { get; set; } = DateTime.UtcNow;
        public string AssemblyName { get; set; }
        public string ProjectPath { get; set; }
        public string TargetExt { get; set; }
        public string OutputType { get; set; }

        public List<ComponentManifest> Components { get; set; } = new List<ComponentManifest>();
    }
}