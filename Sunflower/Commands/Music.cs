using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Newtonsoft.Json;

public class YouTube
{
    public class ResourceId
    {
        public string VideoId { get; set; }
    }

    public class Snippet
    {
        public ResourceId ResourceId { get; set; }
    }

    public class Item
    {
        public Snippet Snippet { get; set; }
    }

    public class Rootobject
    {
        public List<Item> Items { get; set; }
    }
}


namespace Sunflower.Commands
{
    public class Music : BaseCommandModule
    {
        [Command("play")]
        public async Task PlayCommand(CommandContext ctx, [RemainingText] string search)
        {
            if (ctx.Member.VoiceState?.Channel != null)
            {
                await Program.node.ConnectAsync(ctx.Member.VoiceState.Channel);

                Program.conn[ctx.Guild.Id] = Program.node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                if (Program.conn[ctx.Guild.Id] != null)
                {
                    LavalinkLoadResult loadResult = await Program.node.Rest.GetTracksAsync(search);

                    if (loadResult.LoadResultType != LavalinkLoadResultType.LoadFailed && loadResult.LoadResultType != LavalinkLoadResultType.NoMatches)
                    {
                        LavalinkTrack track = loadResult.Tracks.First();
                        if (!Program.playing)
                        {
                            Program.playing = true;
                            await Program.conn[ctx.Guild.Id].PlayAsync(track);
                            var embed = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor("#51c878"),
                                Description = "Now playing **[" + track.Title + "](" + track.Uri + ")** by **" + track.Author + "**",
                            };
                            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                            await ctx.RespondAsync("", embed.Build());
                        }
                        else
                        {
                            if (!Program.queue.ContainsKey(ctx.Guild.Id)) Program.queue[ctx.Guild.Id] = new List<LavalinkTrack>();
                            Program.queue[ctx.Guild.Id].Add(track);
                            var embed = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor("#51c878"),
                                Description = "Added **[" + track.Title + "](" + track.Uri + ")** by **" + track.Author + "**",
                            };
                            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                            await ctx.RespondAsync("", embed.Build());
                        }
                    }
                    else
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#c85151"),
                            Description = "Not found.",
                        };
                        embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                        await ctx.RespondAsync("", embed.Build());
                    }
                }
                else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#c85151"),
                        Description = "Something went wrong.",
                    };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                }

            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#c85151"),
                    Description = "Please join a Voice Channel!",
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
        }

        [Command("playlist")]
        public async Task PlaylistCommand(CommandContext ctx, [RemainingText] string playlistId)
        {
            if (ctx.Member.VoiceState?.Channel != null)
            {
                await Program.node.ConnectAsync(ctx.Member.VoiceState.Channel);

                Program.conn[ctx.Guild.Id] = Program.node.GetGuildConnection(ctx.Member.VoiceState.Guild);

                if (Program.conn[ctx.Guild.Id] != null)
                {

                    WebRequest request = WebRequest.Create("https://www.googleapis.com/youtube/v3/playlistItems?part=snippet&maxResults=50&playlistId=" + playlistId + "&key=" + Program.config.Youtube);
                    WebResponse response = request.GetResponse();
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();

                        var responseJsoned = JsonConvert.DeserializeObject<YouTube.Rootobject>(responseFromServer);

                        if (!Program.queue.ContainsKey(ctx.Guild.Id)) Program.queue[ctx.Guild.Id] = new List<LavalinkTrack>();

                        int failed = 0;

                        foreach(YouTube.Item item in responseJsoned.Items) {
                                LavalinkLoadResult loadResult = await Program.node.Rest.GetTracksAsync(new Uri("https://www.youtube.com/watch?v=" + item.Snippet.ResourceId.VideoId));

                                if (loadResult.LoadResultType != LavalinkLoadResultType.LoadFailed && loadResult.LoadResultType != LavalinkLoadResultType.NoMatches)
                                {

                                    LavalinkTrack track = loadResult.Tracks.First();
                                    Program.queue[ctx.Guild.Id].Add(track);
                                } else
                            {
                                failed++;
                            }
                        };

                        if (!Program.playing)
                        {
                            Program.playing = true;
                            await Program.conn[ctx.Guild.Id].PlayAsync(Program.queue[ctx.Guild.Id].First());
                            Program.queue[ctx.Guild.Id].RemoveAt(0);
                        }
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "Playlist added! " + failed + " songs failed to load.",
                        };
                        embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                        await ctx.RespondAsync("", embed.Build());

                    }
                    response.Close();
                }
                else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#c85151"),
                        Description = "Something went wrong.",
                    };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                }

            }
            else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#c85151"),
                    Description = "Please join a Voice Channel!",
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
        }

        [Command("pause")]
        public async Task PauseCommand(CommandContext ctx, [RemainingText] string search)
        {
            await Program.conn[ctx.Guild.Id].PauseAsync();
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "Song paused!",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("resume")]
        public async Task ResumeCommand(CommandContext ctx, [RemainingText] string search)
        {
            await Program.conn[ctx.Guild.Id].ResumeAsync();
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "Song resumed!",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("skip")]
        public async Task SkipCommand(CommandContext ctx, [RemainingText] string search)
        {
            await Program.conn[ctx.Guild.Id].StopAsync();
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "Song skipped!",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("stop")]
        public async Task StopCommand(CommandContext ctx, [RemainingText] string search)
        {
            Program.queue[ctx.Guild.Id].Clear();
            await Program.conn[ctx.Guild.Id].StopAsync();
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "Song stopped and queue cleared!",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("remove")]
        public async Task RemoveCommand(CommandContext ctx, [RemainingText] int place)
        {
            if (Program.queue.ContainsKey(ctx.Guild.Id) && Program.queue[ctx.Guild.Id][place - 1] != null)
            {
                Program.queue[ctx.Guild.Id].RemoveAt(place - 1);
            }
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "Song removed!",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("queue")]
        public async Task QueueCommand(CommandContext ctx, [RemainingText] string search)
        {
            if (Program.playing)
            {
                List<string> queue = new List<string>();

                int i = 1;

                queue.Add("#0 **[" + Program.conn[ctx.Guild.Id].CurrentState.CurrentTrack.Title + "](" + Program.conn[ctx.Guild.Id].CurrentState.CurrentTrack.Uri + ")** by **" + Program.conn[ctx.Guild.Id].CurrentState.CurrentTrack.Author + "** - " + Program.conn[ctx.Guild.Id].CurrentState.PlaybackPosition.ToString().Substring(0, Program.conn[ctx.Guild.Id].CurrentState.PlaybackPosition.ToString().IndexOf(".")) + "/" + Program.conn[ctx.Guild.Id].CurrentState.CurrentTrack.Length);

                if (Program.queue.ContainsKey(ctx.Guild.Id))
                {
                    Program.queue[ctx.Guild.Id].ForEach(track =>
                {
                    queue.Add("#" + i.ToString() + " **[" + track.Title + "](" + track.Uri + ")** by **" + track.Author + "** - " + track.Length);
                    i++;
                });
                }   

                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    Description = String.Join("\n", queue),
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            } else
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#c85151"),
                    Description = "Queue empty!",
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
        }
    }
}