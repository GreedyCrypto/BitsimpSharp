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
