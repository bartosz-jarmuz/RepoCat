// -----------------------------------------------------------------------

using MongoDB.Bson.Serialization.Attributes;

namespace RepoCat.Persistence.Models
{
    /// <summary>
    /// Summary info about a collection
    /// </summary>
    [BsonIgnoreExtraElements]
    public class CollectionSummary
    {
        /// <summary>
        /// Name
        /// </summary>
        [BsonElement("ns")]
        public string CollectionNamespace { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("size")]
        public long SizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("avgObjSize")]
        public long AverageObjectSizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("count")]
        public long DocumentCount { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [BsonElement("nindexes")]
        public long IndexesCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("totalIndexSize")]
        public long TotalIndexSizeInBytes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [BsonElement("storageSize")]
        public long StorageSizeInBytes { get; set; }
    }

    
}