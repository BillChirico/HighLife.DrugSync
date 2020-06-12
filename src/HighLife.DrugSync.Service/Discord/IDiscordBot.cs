using System.Threading.Tasks;

namespace HighLife.DrugSync.Service.Discord
{
    public interface IDiscordBot
    {
        Task Connect(string token);
    }
}