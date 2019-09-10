using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using RepoCat.Models.Manifests;
using RepoCat.Utilities;
using RepoCat.Web.Persistence;
using RepoCat.Web.Persistence.BooksApi.Models;

namespace RepoCat.Portal.Services
{
    public class ManifestsService
    {
        public readonly IMongoCollection<ProjectManifest> manifests;

        public ManifestsService(IRepoCatDbSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);
            this.manifests = database.GetCollection<ProjectManifest>(settings.ManifestsCollectionName);
            this.ConfigureIndexes();
        }

        private void ConfigureIndexes()
        {
            var keys = Builders<ProjectManifest>.IndexKeys
                .Text("Components.Tags");
                //.Text(m=>m.Repo);
            


            var indexModel = new CreateIndexModel<ProjectManifest>(keys);
            this.manifests.Indexes.CreateOne(indexModel);
        }

        public List<ProjectManifest> Get()
        {
            return this.manifests.Find(manifest => true).ToList();
        }

        public async Task<List<string>> GetRepositories()
        {
            IAsyncCursor<string> result = await this.manifests.DistinctAsync(x => x.Repo, FilterDefinition<ProjectManifest>.Empty);
            return await result.ToListAsync();
        }

        public async Task<ManifestQueryResult> GetAllCurrentProjects(string repositoryName)
        {
            var stopwatch = Stopwatch.StartNew();
            FilterDefinition<ProjectManifest> repoNameFilter = Builders<ProjectManifest>.Filter.Where(x => x.Repo.ToLower().Contains(repositoryName));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepoStamp, repoNameFilter)).ToListAsync();
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectManifest> filter = repoNameFilter & Builders<ProjectManifest>.Filter.Where(x => x.RepoStamp == newestStamp);
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
            FilterDefinition<ProjectManifest> repoNameFilter = Builders<ProjectManifest>.Filter.Where(x => x.Repo.ToLower().Contains(repositoryName));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepoStamp, repoNameFilter)).ToListAsync();
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectManifest> filter = repoNameFilter & Builders<ProjectManifest>.Filter.Where(x => x.RepoStamp == newestStamp);
            if (!string.IsNullOrEmpty(query))
            {
                filter = filter & BuildTextQuery(query, isRegex);
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

        private FilterDefinition<ProjectManifest> BuildTextQuery(string query, bool isRegex)
        {
            if (!isRegex)
            {
                return Builders<ProjectManifest>.Filter.Text(query);
            }
            else
            {
                var regex = new BsonRegularExpression(new System.Text.RegularExpressions.Regex(query, System.Text.RegularExpressions.RegexOptions.IgnoreCase));
                return Builders<ProjectManifest>.Filter.Regex(this.GetComponentFieldName(nameof(ComponentManifest.Tags)), regex) 
                    | Builders<ProjectManifest>.Filter.Regex(this.GetComponentFieldName(nameof(ComponentManifest.Name)), regex)
                    | Builders<ProjectManifest>.Filter.Regex(x=>x.AssemblyName, regex)
                    | Builders<ProjectManifest>.Filter.Regex(x=>x.ProjectName, regex)
                    | Builders<ProjectManifest>.Filter.Regex(x=>x.TargetExt, regex)
                    ;
            }
        }

        private string GetComponentFieldName(string field)
        {
            return nameof(ProjectManifest.Components) + "." + field;
        }

     

        public ProjectManifest Get(string id)
        {
            return this.manifests.Find<ProjectManifest>(manifest => manifest.Id == id).FirstOrDefault();
        }

        public ProjectManifest Create(ProjectManifest manifest)
        {
            this.manifests.InsertOne(manifest);
            return manifest;
        }

        public void Update(string id, ProjectManifest manifestIn)
        {
            this.manifests.ReplaceOne(manifest => manifest.Id == id, manifestIn);
        }

        public void Remove(ProjectManifest manifestIn)
        {
            this.manifests.DeleteOne(manifest => manifest.Id == manifestIn.Id);
        }

        public void Remove(string id)
        {
            this.manifests.DeleteOne(manifest => manifest.Id == id);
        }
    }
}
