using System.Threading.Tasks;

namespace RepoCat.Portal.RecurringJobs
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScanRepositoryJob 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        Task Run(RepositoryToScanSettings settings);
    }
}