using DiscordRPC;
using DiscordRPC.Logging;
using System;

class DiscordPresence
{
    private static DiscordRpcClient client;

    public static void Init(string clientId)
    {
        client = new DiscordRpcClient(clientId);
        client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };
        client.Initialize();
    }

    public static void SetPresence(string image, string texte, string sousTexte)
    {
        if (client == null)
        {
            Console.WriteLine("Erreur : Discord RPC non initialisé. Appelle Init(clientId) avant.");
            return;
        }

        client.SetPresence(new RichPresence()
        {
            Details = texte,
            State = sousTexte,
            Assets = new Assets()
            {
                LargeImageKey = image,
                LargeImageText = texte
            }
        });
    }

    public static void Close()
    {
        client.Dispose();
    }
}