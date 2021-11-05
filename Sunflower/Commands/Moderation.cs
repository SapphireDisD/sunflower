using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Sunflower.Commands
{
    public class Moderation : BaseCommandModule
    {
        [Command("kick")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task KickCommand(CommandContext ctx, DiscordMember member, string reason)
        {
            try
            {
                try
                {
                    var embedM = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#c85151"),
                        Description = "You were kicked from **" + ctx.Guild.Name + "** by **" + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + "** for **" + reason + "**",
                    };
                    embedM.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await (await member.CreateDmChannelAsync()).SendMessageAsync("", embedM.Build());
                }
                catch { }
                await member.RemoveAsync(reason);
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    Description = member.Username + "#" + member.Discriminator + " kicked successfully."
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
            catch
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#c85151"),
                    Description = "Failed kicking this member!",
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
        }

        [Command("warn")]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task WarnCommand(CommandContext ctx, DiscordMember member, string reason)
        {
            try
            {

                var embedM = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#c85151"),
                    Description = "You were warned from **" + ctx.Guild.Name + "** by **" + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + "** for **" + reason + "**",
                };
                embedM.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await (await member.CreateDmChannelAsync()).SendMessageAsync("", embedM.Build());
            }
            catch { }
            string[] warnsThingy = { };
            DbTables.Warns warn = new DbTables.Warns();
            warn.Id = Program.generator.CreateId().ToString();
            warn.UserId = member.Id.ToString();
            warn.GuildId = ctx.Guild.Id.ToString();
            warn.Reason = reason;
            warn.Time = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            warn.Moderator = ctx.Message.Author.Id.ToString();
            Program.db.Insert(warn);
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = member.Username + "#" + member.Discriminator + " warned successfully."
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());

        }

        [Command("warns")]
        public async Task WarnsCommand(CommandContext ctx, DiscordMember member = null)
        {

            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "WARNS:",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);

            if (member == null) member = ctx.Member;
                var warns = Program.db.Table<DbTables.Warns>().ToList().Find(x => x.UserId == member.Id.ToString() && x.GuildId == ctx.Guild.Id.ToString());
                embed.AddField("Warn ID: " + warns?.Id, "Moderator: <@!" + warns?.Moderator + "> | Reason: **" + warns?.Reason + "** | Date: **<t:" + warns?.Time + ">**");
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("removewarn")]
        [RequireUserPermissions(Permissions.KickMembers)]
        public async Task RemoveWarnCommand(CommandContext ctx, string warnId)
        {
                    var userId = Program.db.Table<DbTables.Warns>().ToList().Find(x => x.Id == warnId)?.UserId;
                    ulong id = ulong.Parse(userId ?? "0");
                    if (id > 0)
                    {
                        DiscordMember member = await ctx.Guild.GetMemberAsync(id);
                            Program.db.Delete<DbTables.Warns>(warnId);
                            var embed = new DiscordEmbedBuilder
                            {
                                Color = new DiscordColor("#51c878"),
                                Description = "Warn removed successfully."
                            };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());

                        }
                    else
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#c85151"),
                            Description = "Invalind warn ID.",
                        };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
                    }
                }
        }

    }