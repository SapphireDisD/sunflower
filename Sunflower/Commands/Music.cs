using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;

namespace Sunflower.Commands
{
    public class Music : BaseCommandModule
    {
        [Command("play")]
        public async Task PlayCommand(CommandContext ctx, [RemainingText] string search)
        {
            if(ctx.Member.VoiceState?.Channel != null)
            {
                LavalinkExtension lava = ctx.Client.GetLavalink();
                LavalinkNodeConnection node = lava.ConnectedNodes.Values.First();
                await node.ConnectAsync(ctx.Member.VoiceState.Channel);
                LavalinkGuildConnection conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
                if (conn != null)
                {
                    LavalinkLoadResult loadResult = await node.Rest.GetTracksAsync(search);

                    if (loadResult.LoadResultType != LavalinkLoadResultType.LoadFailed && loadResult.LoadResultType != LavalinkLoadResultType.NoMatches)
                    {
                        LavalinkTrack track = loadResult.Tracks.First();
                        await conn.PlayAsync(track);
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "Now playing **" + track.Title + "** by **" + track.Author + "**",
                        };
                        embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                        await ctx.RespondAsync("", embed.Build());
                    } else
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#c85151"),
                            Description = "Not found.",
                        };
                        embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                        await ctx.RespondAsync("", embed.Build());
                    }
                } else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#c85151"),
                        Description = "Something went wrong.",
                    };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                }

            } else
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
    }
}
