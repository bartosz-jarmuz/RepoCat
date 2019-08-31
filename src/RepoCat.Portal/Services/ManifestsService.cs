using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using RepoCat.Portal.Data.RepoCatDb.BooksApi.Models;
using RepoCat.Portal.Models;

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

        public async Task<Tuple<string,List<ProjectManifest>>> GetCurrentProjects(string repositoryName)
        {
            FilterDefinition<ProjectManifest> repoNameFilter = Builders<ProjectManifest>.Filter.Where(x => x.Repo.ToLower().Contains(repositoryName));
            List<string> stamps = await (await this.manifests.DistinctAsync(x => x.RepoStamp, repoNameFilter)).ToListAsync();
            string newestStamp = StampSorter.GetNewestStamp(stamps);

            FilterDefinition<ProjectManifest> filter = repoNameFilter & Builders<ProjectManifest>.Filter.Where(x => x.RepoStamp == newestStamp);
            var manifests = await (await this.manifests.FindAsync(filter)).ToListAsync();
            return new Tuple<string, List<ProjectManifest>>(newestStamp, manifests);
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
