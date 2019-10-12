using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RepoCat.Persistence.Models;
using RepoCat.Utilities;

namespace RepoCat.Persistence.Service
{
    public class ManifestsService
    {
        public readonly IMongoCollection<ProjectInfo> manifests;

        public ManifestsService(IRepoCatDbSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            this.manifests = database.GetCollection<ProjectInfo>(settings.ManifestsCollectionName);
            this.ConfigureIndexes();
        }

        private void ConfigureIndexes()
        {
            var keys = Builders<ProjectInfo>.IndexKeys
                .Text("Components.Tags");
                //.Text(m=>m.Repo);
            


            var indexModel = new CreateIndexModel<ProjectInfo>(keys);
            this.manifests.Indexes.CreateOne(indexModel);
        }

        public List<ProjectInfo> Get()
        {
            return this.manifests.Find(manifest => true).ToList();
        }

        public async Task<List<string>> GetRepositories()
        {
            IAsyncCursor<string> result = await this.manifests.DistinctAsync(x => x.RepositoryName, FilterDefinition<ProjectInfo>.Empty);
            return await result.ToListAsync();
        }

        public async Task<ManifestQueryResult> GetAllCurrentProjects(string repositoryName)
        {
            var stopwatch = Stopwatch.StartNew();
            FilterDefinition<ProjectInfo> repoNameFilter = Builders<ProjectInfo>.Filter.Where(x => x.RepositoryName.ToLower().Contains(repositoryName));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepositoryStamp, repoNameFilter)).ToListAsync();
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectInfo> filter = repoNameFilter & Builders<ProjectInfo>.Filter.Where(x => x.RepositoryStamp == newestStamp);
            var list = await (await this.manifests.FindAsync(filter)).ToListAsync();
            stopwatch.Stop();

            return new ManifestQueryResult()
            {
                RepoStamp = newestStamp,
                Elapsed = stopwatch.Elapsed,
                Manifests = list
            };
        }

        public async Task<ManifestQueryResult> FindCurrentProjects(string repositoryName, string query, bool isRegex)
        {
            var stopwatch = Stopwatch.StartNew();
            FilterDefinition<ProjectInfo> repoNameFilter = Builders<ProjectInfo>.Filter.Where(x => x.RepositoryName.ToLower().Contains(repositoryName));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepositoryStamp, repoNameFilter)).ToListAsync();
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectInfo> filter = repoNameFilter & Builders<ProjectInfo>.Filter.Where(x => x.RepositoryStamp == newestStamp);
            if (!string.IsNullOrEmpty(query))
            {
                filter = filter & this.BuildTextQuery(query, isRegex);
            }

            var list = await (await this.manifests.FindAsync(filter)).ToListAsync();

            stopwatch.Stop();
            return new ManifestQueryResult()
            {
                Repo = repositoryName,
                RepoStamp = newestStamp,
                Elapsed = stopwatch.Elapsed,
                Manifests = list,
                IsRegex = isRegex,
                QueryString = query
            };
                
        }

        private FilterDefinition<ProjectInfo> BuildTextQuery(string query, bool isRegex)
        {
            if (!isRegex)
            {
                return Builders<ProjectInfo>.Filter.Text(query);
            }
            else
            {
                var regex = new BsonRegularExpression(new System.Text.RegularExpressions.Regex(query, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                return Builders<ProjectInfo>.Filter.Regex(this.GetComponentFieldName(nameof(ComponentManifest.Tags)), regex) 
                    | Builders<ProjectInfo>.Filter.Regex(this.GetComponentFieldName(nameof(ComponentManifest.Name)), regex)
                    | Builders<ProjectInfo>.Filter.Regex(x=>x.AssemblyName, regex)
                    | Builders<ProjectInfo>.Filter.Regex(x=>x.ProjectName, regex)
                    | Builders<ProjectInfo>.Filter.Regex(x=>x.TargetExt, regex)
                    ;
            }
        }

        private string GetComponentFieldName(string field)
        {
            return nameof(ProjectInfo.Components) + "." + field;
        }

     

        public ProjectInfo Get(string id)
        {
            return this.manifests.Find<ProjectInfo>(manifest => manifest.Id == id).FirstOrDefault();
        }

        public ProjectInfo Create(ProjectInfo info)
        {
            this.manifests.InsertOne(info);
            return info;
        }

        public void Update(string id, ProjectInfo info)
        {
            this.manifests.ReplaceOne(manifest => manifest.Id == id, info);
        }

        public void Remove(ProjectInfo info)
        {
            this.manifests.DeleteOne(manifest => manifest.Id == info.Id);
        }

        public void Remove(string id)
        {
            this.manifests.DeleteOne(manifest => manifest.Id == id);
        }
    }
}
