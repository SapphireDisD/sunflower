using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IdGen;
using SQLite;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using DSharpPlus.Net;
using DSharpPlus.Lavalink;
using System.Collections;

public class Config
{
    public string Token { get; set; }
    public string Cat { get; set; }
    public string Perspective { get; set; }
    public string Youtube { get; set; }
    public string Domain { get; set; }
    public string Contact { get; set; }
    public List<ulong> Owners { get; set; }
    public string[] Prefixes { get; set; }
}

public class DbTables
{

    public class Prefixes
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Prefix { get; set; }
    }

    public class Warns
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GuildId { get; set; }
        public string Reason { get; set; }
        public string Time { get; set; }
        public string Moderator { get; set; }
    }
}

public class WarnerDictionary : IEnumerable
{
    private Dictionary<ulong, LavalinkGuildConnection> Dictionary { get; set; }

    public WarnerDictionary()
    {
        Dictionary = new Dictionary<ulong, LavalinkGuildConnection>();
    }

    public LavalinkGuildConnection this[ulong key]
    {
        get
        {
            return Dictionary.ContainsKey(key) ? Dictionary[key] : null;
        }
        set
        {
                Dictionary[key] = value;
                value.PlaybackFinished += async (s, e) =>
                {
                    if (Sunflower.Program.queue.Count > 0)
                    {
                        await value.PlayAsync(Sunflower.Program.queue[key].First());

                        Sunflower.Program.queue[key].RemoveAt(0);
                    }
                    else
                    {
                        Sunflower.Program.playing = false;
                        await value.DisconnectAsync();
                    }
                };
        }
    }

    public IEnumerator GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }
}

namespace Sunflower
{
    class Program
    {
        public static Config config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(@"Config.json"));
        public static string version = "WIP";
        public static SQLiteConnection db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "CISP.db"));
        public static IdGenerator generator = new IdGenerator(0);
        public static LavalinkExtension lava;
        public static LavalinkNodeConnection node;
        public static WarnerDictionary conn = new WarnerDictionary();
        public static bool playing = false;
        public static Dictionary<ulong, List<LavalinkTrack>> queue = new Dictionary<ulong, List<LavalinkTrack>>();

        static void Main(string[] args)
        {
            db.CreateTable<DbTables.Prefixes>();
            db.CreateTable<DbTables.Warns>();
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {

           var discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = config.Token,
                TokenType = TokenType.Bot
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = config.Prefixes
            });

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            commands.RegisterCommands<Commands.Fun>();
            commands.RegisterCommands<Commands.Info>();
            commands.RegisterCommands<Commands.Moderation>();
            commands.RegisterCommands<Commands.Music>();
            commands.RegisterCommands<Commands.Utility>();

            var lavalink = discord.UseLavalink();

            discord.MessageCreated += (s, e) =>
            {
                foreach (Type typeA in typeof(ICommand).Assembly.GetTypes().Where(type => type.Namespace == "Sunflower.Filters" && !type.FullName.Contains("+")).ToArray())
                {
                    var methodInfo = typeA.GetMethod("Run");
                    object classInstance = Activator.CreateInstance(typeA, null);
                    methodInfo.Invoke(classInstance, new object[] { s, e.Message });
                }
                return Task.CompletedTask;
            };

            discord.Ready += async (s, e) =>
            {
                await s.UpdateStatusAsync(new DiscordActivity("@" + s.CurrentUser.Username + " help - " + Program.version));
                Console.WriteLine("Ready!");
            };

            await discord.ConnectAsync();

            await lavalink.ConnectAsync(lavalinkConfig);

            lava = discord.GetLavalink();
            node = lava.ConnectedNodes.Values.First();

        await Task.Delay(-1);
            }
    }
}