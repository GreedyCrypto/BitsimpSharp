using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BitsimpBot.Commands;
using BitsimpBot.Logging;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Discord.Addons.Interactive;
using Serilog;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using VRChatN.Configuration;
using BitsimpBot.Commands.Modules;
using Flurl.Http;

namespace BitsimpBot
{
    public class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;

        public Program()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "appsettings.json");

            // build the configuration and assign to _config          
            _config = _builder.Build();
        }


        public static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();
        


        public async Task MainAsync()
        {
            try
            {

        
                Log.Logger = new LoggerConfiguration()
                  .WriteTo.Console(Serilog.Events.LogEventLevel.Debug)
                  .MinimumLevel.Debug()
                  .Enrich.FromLogContext()
                  .CreateLogger();

                using (var services = ConfigureServices())
                {
                    // get the client and assign to client 
                    // you get the services via GetRequiredService<T>
                    var client = services.GetRequiredService<DiscordSocketClient>();
                    _client = client;

                    // setup logging and the ready event
                    client.Log += LogDiscord;
                    client.Ready += ReadyAsync;
                    services.GetRequiredService<CommandService>().Log += LogDiscord;

                    Settings.vrcusername = _config.GetValue<string>("VRChat:LoginData:Username");
                    Settings.vrcpassword = _config.GetValue<string>("VRChat:LoginData:Password");
                    Settings.discordtoken = _config.GetValue<string>("DiscordConfiguration:token");
                    Settings.friends_endpoint = _config.GetValue<string>("VRChat:Endpoints:Friends");
                    Settings.activeusers_endpoint = _config.GetValue<string>("VRChat:Endpoints:ActiveUsers");
                    Settings.userinfo_endpoint = _config.GetValue<string>("VRChat:Endpoints:UserInfo");
                    Settings.users_endpoint = _config.GetValue<string>("VRChat:Endpoints:Users");
                    Settings.recentworlds_endpoint = _config.GetValue<string>("VRChat:Endpoints:RecentWorlds");
                    Settings.favouriteworlds_endpoint = _config.GetValue<string>("VRChat:Endpoints:FavouriteWorlds");
                    Settings.APIBase = _config.GetValue<string>("VRChat:Endpoints:Base");
                    Settings.APIKey = _config.GetValue<string>("VRChat:Endpoints:key");
                    Settings.login_endpoint = _config.GetValue<string>("VRChat:Endpoints:Login");

                    // this is where we get the Token value from the configuration file, and start the bot
                    await client.LoginAsync(TokenType.Bot, Settings.discordtoken);
                    await client.StartAsync();

                    FlurlCookie AuthCookie = await VRCModule.LoginVRC();
                    if (AuthCookie == null)
                        Log.Error("Could not login to VRChat API");
                    else
                        Log.Information("Logged in. AuthCookie: " + AuthCookie.Value);

                    Settings.AuthCookie = AuthCookie;
                    // we get the CommandHandler class here and call the InitializeAsync method to start things up for the CommandHandler service
                    await services.GetRequiredService<CommandHandler>().InitializeAsync();

                    await Task.Delay(-1);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " " + ex.StackTrace);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        private Task LogDiscord(LogMessage msg)
        {
            Log.Information(msg.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<InteractiveService>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }
    }
}
