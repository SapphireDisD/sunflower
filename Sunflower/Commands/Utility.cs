using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

public class CovidClasses
{
    public class Rootobject
    {
        public List[] List { get; set; }
    }

    public class List
    {
        public object Infected { get; set; }
        public object Tested { get; set; }
        public object Recovered { get; set; }
        public object Deceased { get; set; }
    }

    public class Lists
    {
        public static int TotalCases;
        public static int TotalRecovered;
        public static int TotalDead;
        public static int TotalTested;
    }
}

namespace Sunflower.Commands
{
    public class Utility : BaseCommandModule
    {
        [Command("covid")]
        public async Task Covid(CommandContext ctx)
        {
            var parsedJson = JsonConvert.DeserializeObject<CovidClasses.Rootobject>("{ \"list\": " + new StreamReader(WebRequest.Create("https://api.apify.com/v2/key-value-stores/tVaYRsPHLjNdNBu7S/records/LATEST?disableRedirect=true").GetResponse().GetResponseStream()).ReadToEnd() + " }");

            try
            {
                foreach (CovidClasses.List country in parsedJson.List)
                {
                    Int32.TryParse(country.Infected.ToString(), out int cases);
                    CovidClasses.Lists.TotalCases += cases;
                    Int32.TryParse(country.Recovered.ToString(), out int recovered);
                    CovidClasses.Lists.TotalRecovered += recovered;
                    Int32.TryParse(country.Deceased.ToString(), out int dead);
                    CovidClasses.Lists.TotalDead += dead;
                    Int32.TryParse(country.Tested.ToString(), out int tested);
                    CovidClasses.Lists.TotalTested += tested;
                }

                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    Description = "COVID-19 Statistics"
                };
                embed.AddField("Total Cases (Reported)", CovidClasses.Lists.TotalCases.ToString());
                embed.AddField("Total Recovered (Reported)", CovidClasses.Lists.TotalRecovered.ToString());
                embed.AddField("Total Dead (Reported)", CovidClasses.Lists.TotalDead.ToString());
                embed.AddField("Total Tested (Reported)", CovidClasses.Lists.TotalTested.ToString());
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
            catch
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#c85151"),
                    Description = "Something went terribly wrong, please try again later."
                };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
            }
        }

        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            long preDate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            var messageToEdit = await ctx.RespondAsync("Getting ping...");
            long postDate = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            long ping = postDate - preDate;
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Description = "Pong!\n\nIt took " + ping + " ms to edit the message and the websocket ping is " + ctx.Client.Ping + " ms.",
            };
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await messageToEdit.ModifyAsync("", embed.Build());
        }

        [Command("ping")]
        public async Task PollCommand(CommandContext ctx, params string[] args)
        {
            string[] pollArgs = Extentions.GetArguments(String.Join(" ", args));
            if (pollArgs.Length == 1)
            {
                DiscordMessage pollMessage = await ctx.RespondAsync("📊" + pollArgs[0]);
                await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":white_check_mark:"));
                await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":x:"));
            }
            else
            {
                if (pollArgs.Length < 12)
                {
                    string[] reactions = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
                    string[] options = { };
                    int i1 = 0;
                    pollArgs.Skip(1).ToList().ForEach(option => {
                        var pre = options.ToList();
                        pre.Add(DiscordEmoji.FromName(ctx.Client, ":" + reactions[i1] + ":") + ": " + option);
                        options = pre.ToArray();
                        i1++;
                    });
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#51c878"),
                        Description = String.Join("\n", options),
                    };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    DiscordMessage pollMessage = await ctx.RespondAsync(pollArgs[0], embed.Build());
                    int i2 = 0;
                    foreach (string _ in options)
                    {
                        await pollMessage.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":" + reactions[i2] + ":"));
                        i2++;
                    }
                }
                else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#c85151"),
                        Description = "Please don't input more than 10 options!",
                    };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync(pollArgs[0], embed.Build());
                }
            }
        }
    }
}