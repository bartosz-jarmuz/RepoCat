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
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

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
