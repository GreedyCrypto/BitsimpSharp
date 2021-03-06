using BitsimpBot.DataContext;
using Discord;
using Discord.Commands;
using Flurl.Http;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Threading.Tasks;
using VRChatN.Configuration;


namespace BitsimpBot.Commands.Modules
{
    public class DomainModule : ModuleBase<SocketCommandContext>
    {

        //using ip2whois.com api


        [Command("checkd")]
        [Summary("Checks domain data")]
        public async Task CheckDomain([Remainder] string args = null)
        {
            try
            {
                string[] data = Context.Message.ToString().Split(" ");
                string[] domains = data[1..(data.Length)];

                for (var i = 0; i <= domains.Length - 1; i++)
                {
                    Domain domaindata = await ("https://api.ip2whois.com/v1?key=" + Settings.DomainAPIKey + "&domain=" + domains[i]).GetJsonAsync<Domain>();
                    var discordembed = new EmbedBuilder
                    {
                        Title = (domaindata.domain is null || domaindata.domain == "") ? "undefined" : domaindata.domain,
                        Description = (domaindata.domain is null || domaindata.domain == "") ? "undefined" : domaindata.domain,
                        Timestamp = DateTime.UtcNow 
                    };
                    discordembed.AddField("Registrant Name", (domaindata.registrant.name is null || domaindata.registrant.name == "") ? "undefined" : domaindata.registrant.name);
                    discordembed.AddField("Registrant Organisation", (domaindata.registrant.organization is null || domaindata.registrant.organization == "") ? "undefined" : domaindata.registrant.organization);
                    discordembed.AddField("Domain Created", (domaindata.createdDate is null || domaindata.createdDate.ToString() == "") ? "undefined" : domaindata.createdDate, true);
                    discordembed.AddField("Domain Updated", (domaindata.updateDate is null || domaindata.updateDate.ToString() == "") ? "undefined" : domaindata.updateDate, true);
                    discordembed.AddField("Domain Status", (domaindata.status is null || domaindata.status == "") ? "undefined" : domaindata.status, false);
                    await ReplyAsync(embed: discordembed.Build());
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " " + ex.StackTrace);
            }
            return;
        }
    }
}
