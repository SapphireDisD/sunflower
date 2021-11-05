using System;
using System.Linq;

namespace Sunflower
{

    // By Ahmed
    public interface ICommand
    {
        void Run(DSharpPlus.DiscordClient client, DSharpPlus.Entities.DiscordMessage message);
    }

    public static class Extentions
    {
        public static string[] GetArguments(string body)
        {
            string[] args = { };
            string str = body.Trim();
            while (str.Length > 0)
            {
                string arg;
                if (str.StartsWith('"') && str.IndexOf('"', 1) > 0)
                {
                    arg = str.Substring(1, str.IndexOf('"', 1));
                    str = str.Substring(str.IndexOf('"', 1) + 1);
                    var pre = args.ToList();
                    pre.Add(arg.Remove(arg.Length - 1).Trim());
                    args = pre.ToArray();
                    str = str.Trim();
                }
                else
                {
                    return new string[] { };
                }
            }
            return args;
        }
    }
}
