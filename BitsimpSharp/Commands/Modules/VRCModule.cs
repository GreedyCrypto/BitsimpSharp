using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using BitsimpBot.DataContext;
using Flurl.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VRChatN.Configuration;

namespace BitsimpBot.Commands.Modules
{
    public class VRCModule : InteractiveBase
    {


        // ~sample square 20 -> 400
        [Command("square")]
        [Summary("Squares a number.")]
        public async Task SquareAsync(
            [Summary("The number to square.")]
        int num)
        {
            // We can also access the channel from the Command Context.
            await Context.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
        }

        // ~sample userinfo --> foxbot#0282
        // ~sample userinfo @Khionu --> Khionu#8708
        // ~sample userinfo Khionu#8708 --> Khionu#8708
        // ~sample userinfo Khionu --> Khionu#8708
        // ~sample userinfo 96642168176807936 --> Khionu#8708
        // ~sample whois 96642168176807936 --> Khionu#8708
        [Command("userinfo")]
        [Summary
        ("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois")]
        public async Task UserInfoAsync(
            [Summary("The (optional) user to get info from")]
        SocketUser user = null)
        {
            var userInfo = user ?? Context.Client.CurrentUser;
            await ReplyAsync($"{userInfo.Username}#{userInfo.Discriminator}");
        }


        [Command("vrcuser")]
        [Summary
        ("Should return paginated DiscordEmbed")]
        public async Task PaginatedUserEmbed([Remainder] string args = null)
        {


            string[] data = Context.Message.ToString().Split(" ");
            string[] userdata = data[1..(data.Length)];
            string selectedUser = "";
            foreach (string s in userdata)
            {
                selectedUser += s;
            }

            string username = Settings.vrcusername;
            string password = Settings.vrcpassword;

                
            Console.WriteLine(Settings.APIBase + Settings.users_endpoint + "?search" + $"={selectedUser}&apiKey=" + Settings.APIKey);
            dynamic client = await (Settings.APIBase + Settings.users_endpoint + "?search" + $"={selectedUser}&apiKey=" + Settings.APIKey).WithBasicAuth(username, password).GetJsonAsync<List<Friend>>();

            int searchobjects = 0;
            foreach (Friend f in client)
            {
                searchobjects++;
            }





            Embed embed;

            List<PaginatedMessage> pages = new List<PaginatedMessage>();
            PaginatedMessage pmsg = new PaginatedMessage();
            pmsg.Pages = client;
            foreach (Friend f in client)
            {
                Console.WriteLine(f.currentAvatarThumbnailImageUrl);
                pmsg.ImageURL = f.currentAvatarThumbnailImageUrl;
                pmsg.Title = f.displayName;
                pmsg.AlternateDescription = f.bio;
            }

            await PagedReplyAsync(pmsg, false);


        }


    }





}
