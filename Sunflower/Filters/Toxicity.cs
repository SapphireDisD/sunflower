using DSharpPlus;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class ToxicClasses
{

    public class Rootobject
    {
        public Attributescores attributeScores { get; set; }
    }

    public class Attributescores
    {
        public TOXICITY TOXICITY { get; set; }
    }

    public class TOXICITY
    {
        public Summaryscore summaryScore { get; set; }
    }

    public class Summaryscore
    {
        public float value { get; set; }
    }

}

namespace Sunflower.Filters
{
    class Toxicity
    {
        public static async Task Run(DiscordClient client, DiscordMessage message)
        {
            Dictionary<char, char> cylliricList = new Dictionary<char, char>()
            {
{ 'А', 'а'},
{ 'Б', 'б' },
{ 'В', 'в' },
{ 'Г', 'г' },
{ 'Ґ', 'ґ' },
{ 'Ѓ', 'ѓ' },
{ 'Д', 'д' },
{ 'Ђ', 'ђ' },
{ 'Є', 'є' },
{ 'Е', 'е' },
{ 'Ё', 'ё' },
{ 'Ж', 'ж' },
{ 'З', 'з' },
{ 'Ѕ', 'ѕ' },
{ 'И', 'и' },
{ 'І', 'і' },
{ 'Ї', 'ї' },
{ 'Й', 'й' },
{ 'Ј', 'ј' },
{ 'К', 'к' },
{ 'Л', 'л' },
{ 'Љ', 'љ' },
{ 'М', 'м' },
{ 'Н', 'н' },
{ 'Њ', 'њ' },
{ 'О', 'о' },
{ 'П', 'п' },
{ 'Р', 'р' },
{ 'С', 'с' },
{ 'Т', 'т' },
{ 'Ћ', 'ћ' },
{ 'Ќ', 'ќ' },
{ 'У', 'у' },
{ 'Ў', 'ў' },
{ 'Ф', 'ф' },
{ 'Х', 'х' },
{ 'Ц', 'ц' },
{ 'Ч', 'ч' },
{ 'Џ', 'џ' },
{ 'Ш', 'ш' },
{ 'Щ', 'щ' },
{ 'Ъ', 'ъ' },
{ 'Ы', 'ы' },
{ 'Ь', 'ь' },
{ 'Э', 'э' },
{ 'Ю', 'ю' },
{ 'Я', 'я' }
            };

            Dictionary<char, char> lowerList= new Dictionary<char, char>()
            {
                { 'а', 'a' },
{ 'б', 'b' },
{ 'в', 'b' },
{ 'г', 'r' },
{ 'ґ', 'r' },
{ 'ѓ', 'r' },
{ 'д', 'a' },
{ 'ђ', 'h' },
{ 'є', 'E' },
{ 'е', 'e' },
{ 'ё', 'e' },
{ 'ж', 'x' },
{ 'з', 'e' },
{ 'ѕ', 's' },
{ 'и', 'n' },
{ 'і', 'i' },
{ 'ї', 'i' },
{ 'й', 'n' },
{ 'ј', 'j' },
{ 'к', 'k' },
{ 'л', 'n' },
{ 'љ', 'b' },
{ 'м', 'm' },
{ 'н', 'h' },
{ 'њ', 'h' },
{ 'о', 'o' },
{ 'п', 'n' },
{ 'р', 'p' },
{ 'с', 'c' },
{ 'т', 't' },
{ 'ћ', 'h' },
{ 'ќ', 'k' },
{ 'у', 'y' },
{ 'ў', 'y' },
{ 'ф', 'o' },
{ 'х', 'x' },
{ 'ц', 'u' },
{ 'ч', 'y' },
{ 'џ', 'u' },
{ 'ш', 'w' },
{ 'щ', 'w' },
{ 'ъ', 'b' },
{ 'ы', 'b' },
{ 'ь', 'b' },
{ 'э', 'e' },
{ 'ю', 'o' },
{ 'я', 'r' }
            };
            string textA = lowerList.Aggregate(cylliricList.Aggregate(message.Content, (c, d) => c.Replace(d.Key, d.Value)), (c, d) => c.Replace(d.Key, d.Value));

            char[] preTextB = textA.ToCharArray();

            Array.Reverse(preTextB);

            string textB = String.Join("", preTextB);

            HttpClient httpClient = new HttpClient();

            StringContent content = new StringContent("{ \"comment\": { \"text\": \"" + textA + "\" }, \"requestedAttributes\": { \"TOXICITY\": {} } }", Encoding.UTF8, "application/json");

            HttpResponseMessage preResponse = await httpClient.PostAsync("https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key=" + Program.config.Perspective, content);

            string response = await preResponse.Content.ReadAsStringAsync();

            float toxicity = JsonConvert.DeserializeObject<ToxicClasses.Rootobject>(response).attributeScores?.TOXICITY?.summaryScore?.value ?? 0;

            if(toxicity > 0.8)
            {
               await SendToInspection(client, message);
            } else
            {
                StringContent contentB = new StringContent("{ \"comment\": { \"text\": \"" + textB + "\" }, \"requestedAttributes\": { \"TOXICITY\": {} } }", Encoding.UTF8, "application/json");

                HttpResponseMessage preResponseB = await httpClient.PostAsync("https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key=" + Program.config.Perspective, contentB);

                string responseB = await preResponseB.Content.ReadAsStringAsync();

                float toxicityB = JsonConvert.DeserializeObject<ToxicClasses.Rootobject>(responseB).attributeScores?.TOXICITY?.summaryScore?.value ?? 0;

                if(toxicityB > 0.8)
                {
                   await SendToInspection(client, message);
                }
            }
            return;
        }

        public static async Task SendToInspection(DiscordClient client, DiscordMessage message)
        {
            await message.DeleteAsync();
            await message.Channel.SendMessageAsync("No.");
            return;
        }
    }
}
