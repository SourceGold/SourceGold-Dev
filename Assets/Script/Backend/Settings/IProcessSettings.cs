using System.Threading.Tasks;

namespace Assets.Script.Backend
{
    public interface IProcessSettings
    {
        Task<GlobalSettings> LoadGlobalSettingsAsync();

        /// <summary>
        /// Save Global Settings
        /// </summary>
        /// <param name="globalSettings"></param>
        /// <returns>If save successed</returns>
        Task<bool> SaveGlobalSettingsAsync(GlobalSettings globalSettings);
    }
}
