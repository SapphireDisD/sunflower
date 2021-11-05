using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Sunflower.Commands
{
    public class Info : BaseCommandModule
    {
        [Command("credits")]
        public async Task CreditsCommand(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Title = "Credits"
            };
            embed.AddField("Our library", "https://dsharpplus.github.io/");
            embed.AddField("Where we get bird photos", "https://www.reddit.com/r/birdpics/");
            embed.AddField("Where we get cat photos", "https://thecatapi.com/");
            embed.AddField(
              "Where we get dogs photos",
              "https://dog.ceo/api/breeds/image/random"
            );
            embed.AddField("Where we get fox photos", "https://randomfox.ca/floof/");
            embed.AddField(
              "Where we get info about the Covid-19",
              "https://apify.com/covid-19"
            );
            embed.AddField("Where we get memes", "https://www.reddit.com/r/Memes/");
            List<string> owners = new List<string>();
            Program.config.Owners.ForEach(async x => {
                DiscordUser user = await ctx.Client.GetUserAsync(x);
                owners.Add(user.Username + "#" + user.Discriminator);
            });
            embed.AddField(
              "Owners",
                String.Join(", ", owners)
            );
            embed.AddField("Our Logo & Rank Card Designer", "https://twitter.com/zeealeid");
            embed.AddField(
              "The Users",
              "You (Thanks for being our inspiration to keep creating)"
            );
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }

        [Command("info")]
        public async Task InfoCommand(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#51c878"),
                Title = "Bot info"
            };
            embed.AddField("Version", Program.version);
            embed.AddField("Library", "DSharpPlus 4.1");
            embed.AddField("Invite", Program.config.Domain + "/invite");
            embed.AddField("Contact us", Program.config.Contact);
            embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
            await ctx.RespondAsync("", embed.Build());
        }
    }
}
