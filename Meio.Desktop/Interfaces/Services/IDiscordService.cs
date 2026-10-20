using System.Threading.Tasks;
using TagLib;

namespace Meio.desktop.Interfaces.Services;

public interface IDiscordService
{
    bool IsConnected { get; }

    Task SetPresence(string text = "", string state = "Idling...");

    Task SetPresencePlay(File audioFile);

    void ClearPresence();
}