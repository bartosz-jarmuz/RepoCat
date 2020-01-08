using System.Threading.Tasks;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScanRepositoryJob 
    {
        Task Run(RepositoryToScanSettings settings);
    }
}