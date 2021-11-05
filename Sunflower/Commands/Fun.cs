using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;

public class ImgurClasses
{
    public class Rootobject
    {
        public Datum[] Data { get; set; }
    }

    public class Datum
    {
        public string Hash { get; set; }
        public string Ext { get; set; }
    }

    public class Images
    {
        public static string[] Names = { };
        public static string[] Extensions = { };
    }
}

public class CatClasses
{
    public class Rootobject
    {
        public Thing[] Thing { get; set; }
    }

    public class Thing
    {
        public string Url { get; set; }
    }
}

public class DogClasses
{
    public class Rootobject
    {
        public string Message { get; set; }
    }
}

public class FoxClasses
{


    public class Rootobject
    {
        public string Image { get; set; }
    }


}

namespace Sunflower.Commands
{
    public class Fun : BaseCommandModule
    {
        [Command("bird")]
        public async Task BirdCommand(CommandContext ctx)
        {
            try
            {
                foreach (ImgurClasses.Datum data in JsonConvert.DeserializeObject<ImgurClasses.Rootobject>(new StreamReader(WebRequest.Create("https://imgur.com/r/birdpics/hot.json").GetResponse().GetResponseStream()).ReadToEnd()).Data)
                {
                    List<string> pre = new List<string>(ImgurClasses.Images.Names);
                    pre.Add(data.Hash);
                    ImgurClasses.Images.Names = pre.ToArray();

                    List<string> pre2 = new List<string>(ImgurClasses.Images.Extensions);
                    pre2.Add(data.Ext);
                    ImgurClasses.Images.Extensions = pre2.ToArray();
                }

                int selected = new Random().Next(0, ImgurClasses.Images.Names.Length - 1);

                string url = "https://i.imgur.com/" + ImgurClasses.Images.Names[selected] + ImgurClasses.Images.Extensions[selected];

                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    ImageUrl = url
                };
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

        [Command("cat")]
        public async Task CatCommand(CommandContext ctx)
        {
            try
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    ImageUrl = JsonConvert.DeserializeObject<CatClasses.Rootobject>("{ \"thing\": " + new StreamReader(WebRequest.Create("https://api.thecatapi.com/v1/images/search?api_key=" + Program.config.Cat).GetResponse().GetResponseStream()).ReadToEnd() + " }").Thing[0].Url
                };
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

        [Command("dog")]
        public async Task DogCommand(CommandContext ctx)
        {
            try
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    ImageUrl = JsonConvert.DeserializeObject<DogClasses.Rootobject>(new StreamReader(WebRequest.Create("https://dog.ceo/api/breeds/image/random").GetResponse().GetResponseStream()).ReadToEnd()).Message
                };
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

        [Command("fox")]
        public async Task FoxCommand(CommandContext ctx)
        {
            try
            {
                var embed = new DiscordEmbedBuilder
                {
                    Color = new DiscordColor("#51c878"),
                    ImageUrl = JsonConvert.DeserializeObject<FoxClasses.Rootobject>(new StreamReader(WebRequest.Create("https://randomfox.ca/floof/").GetResponse().GetResponseStream()).ReadToEnd()).Image
                };
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

        [Command("meme")]
        [RequireNsfw]
        public async Task MemeCommand(CommandContext ctx)
        {
                try
                {
                    foreach (ImgurClasses.Datum data in JsonConvert.DeserializeObject<ImgurClasses.Rootobject>(new StreamReader(WebRequest.Create("https://imgur.com/r/memes/hot.json").GetResponse().GetResponseStream()).ReadToEnd()).Data)
                    {
                        List<string> pre = new List<string>(ImgurClasses.Images.Names);
                        pre.Add(data.Hash);
                        ImgurClasses.Images.Names = pre.ToArray();

                        List<string> pre2 = new List<string>(ImgurClasses.Images.Extensions);
                        pre2.Add(data.Ext);
                        ImgurClasses.Images.Extensions = pre2.ToArray();
                    }

                    int selected = new Random().Next(0, ImgurClasses.Images.Names.Length - 1);

                    string url = "https://i.imgur.com/" + ImgurClasses.Images.Names[selected] + ImgurClasses.Images.Extensions[selected];

                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#51c878"),
                        ImageUrl = url
                    };
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

        [Command("rps")]
        public static async Task Run(CommandContext ctx, string option)
        {
                if (
                  (option.ToLower() == "rock") ||
                  option.ToLower() == "paper" ||
                  option.ToLower() == "scissors"
                )
                {
                    int answerInNumbers = new Random().Next(1, 3);
                    if (option.ToLower() == "rock" && answerInNumbers == 1)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected rock and I selected rock. Draw.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "rock" && answerInNumbers == 2)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected rock and I selected paper. I won.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "rock" && answerInNumbers == 3)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected rock and I selected scissors. You won.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "paper" && answerInNumbers == 1)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected paper and I selected rock. You won.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "paper" && answerInNumbers == 2)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected paper and I selected paper. Draw.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "paper" && answerInNumbers == 3)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected paper and I selected scissors. I won",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "scissors" && answerInNumbers == 1)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected scissors and I selected rock. I won.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "scissors" && answerInNumbers == 2)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected scissors and I selected paper. You won.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                    if (option.ToLower() == "scissors" && answerInNumbers == 3)
                    {
                        var embed = new DiscordEmbedBuilder
                        {
                            Color = new DiscordColor("#51c878"),
                            Description = "You selected scissors and I selected scissors. Draw.",
                        };
                    embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                    await ctx.RespondAsync("", embed.Build());
                    }
                }
                else
                {
                    var embed = new DiscordEmbedBuilder
                    {
                        Color = new DiscordColor("#51c878"),
                        Description = "Please select rock, paper or scissors.",
                    };
                embed.WithFooter("Requested by " + ctx.Message.Author.Username + "#" + ctx.Message.Author.Discriminator + " (" + ctx.Message.Author.Id + ")", ctx.Message.Author.AvatarUrl);
                await ctx.RespondAsync("", embed.Build());
                }
            }

    }
}
