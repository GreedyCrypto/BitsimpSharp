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
using System.Net;

namespace BitsimpBot.Commands.Modules
{
    public class VRCModule : InteractiveBase
    {


        
        public static async Task<FlurlCookie> LoginVRC()
        {
            string username = Settings.vrcusername;
            string password = Settings.vrcpassword;
            dynamic client = await (Settings.APIBase + Settings.login_endpoint + "?apiKey=" + Settings.APIKey).WithBasicAuth(username, password).WithCookies(out var jar).WithHeader("User-Agent", "BitsimpSharp").GetJsonAsync();
            Console.WriteLine(jar.Count.ToString());
            foreach(FlurlCookie cookie in jar)
            {
                if (cookie.Name == "auth")
                {
                    FlurlCookie AuthCookie;
                    AuthCookie = cookie;
                    return AuthCookie;
                }
            }
            return null;
        }




            [Command("vrcuser")]
        [Summary
        ("Should return paginated DiscordEmbed")]
        public async Task GetVRChatUser([Remainder] string args = null)
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
            //dynamic client = await (Settings.APIBase + Settings.login_endpoint + "?apiKey=" + Settings.APIKey).WithBasicAuth(username, password).WithHeader("User-Agent", "BitsimpSharp").GetJsonAsync<List<Friend>>();

            dynamic client = await (Settings.APIBase + Settings.users_endpoint + "?search" + $"={selectedUser}&apiKey=" + Settings.APIKey).WithCookie("auth", Settings.AuthCookie.Value).WithHeader("User-Agent", "BitsimpSharp").GetJsonAsync<List<Friend>>();

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
            InteractiveService.paginatedEmbedType = 1;

            await PagedReplyAsync(pmsg, false);


        }



[Command("vrcworlds")]
        [Summary
        ("Should return recent vrc worlds")]
        public async Task PaginatedUserEmbed([Remainder] string args = null)
        {

            /*
            string[] data = Context.Message.ToString().Split(" ");
            string[] userdata = data[1..(data.Length)];
            string selectedUser = "";
            foreach (string s in userdata)
            {
                selectedUser += s;
            }
            */
            string username = Settings.vrcusername;
            string password = Settings.vrcpassword;

                
            //Console.WriteLine(Settings.APIBase + Settings.recentworlds_endpoint + "&&apiKey=" + Settings.APIKey);
            dynamic client = await (Settings.APIBase + Settings.recentworlds_endpoint + "?sort=_created_at&apiKey=" + Settings.APIKey).WithCookie("auth", Settings.AuthCookie.Value).WithHeader("User-Agent", "BitsimpSharp").GetJsonAsync<List<World>>();
            int searchobjects = 0;
            foreach (World w in client)
            {
                searchobjects++;
            }





            Embed embed;

            List<PaginatedMessage> pages = new List<PaginatedMessage>();
            PaginatedMessage pmsg = new PaginatedMessage();
            pmsg.Pages = client;
            foreach (World w in client)
            {
                Console.WriteLine(w.WorldImage);
                pmsg.ImageURL = w.WorldImage;
                pmsg.Title = w.WorldName;
                pmsg.AlternateDescription = "Created by " + w.AuthorName;
            }

            InteractiveService.paginatedEmbedType = 2;

            await PagedReplyAsync(pmsg, false);


        }








    }





}
