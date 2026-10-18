using System.Threading.Tasks;
using TagLib;

namespace Meio.app.Interfaces.Services;

public interface IDiscordService
{
    bool IsConnected { get; }

    Task SetPresence(string text = "", string state = "Idling...");

    Task SetPresencePlay(File audioFile);

    void ClearPresence();
}