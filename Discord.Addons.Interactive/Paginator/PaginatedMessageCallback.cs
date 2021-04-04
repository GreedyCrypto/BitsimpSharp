using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord.Addons.Interactive
{
    public class PaginatedMessageCallback : IReactionCallback
    {
        public SocketCommandContext Context { get; }
        public InteractiveService Interactive { get; private set; }
        public IUserMessage Message { get; private set; }

        public RunMode RunMode => RunMode.Sync;
        public ICriterion<SocketReaction> Criterion => _criterion;
        public TimeSpan? Timeout => options.Timeout;

        private readonly ICriterion<SocketReaction> _criterion;
        private readonly PaginatedMessage _pager;

        private PaginatedAppearanceOptions options => _pager.Options;
        private readonly int pages;
        private int page = 1;
        public int currentPage = 0;
        List<dynamic> pageList = new List<dynamic>();

        public PaginatedMessageCallback(InteractiveService interactive, 
            SocketCommandContext sourceContext,
            PaginatedMessage pager,
            ICriterion<SocketReaction> criterion = null)
        {
            Interactive = interactive;
            Context = sourceContext;
            _criterion = criterion ?? new EmptyCriterion<SocketReaction>();
            _pager = pager;
            pages = _pager.Pages.Count();
            if (_pager.Pages is IEnumerable<EmbedFieldBuilder>)
                pages = ((_pager.Pages.Count() - 1) / options.FieldsPerPage) + 1;
        }

        public async Task DisplayAsync()
        {
            var embed = BuildEmbed();
            var message = await Context.Channel.SendMessageAsync(_pager.Content, embed: embed).ConfigureAwait(true);
            Message = message;
            Interactive.AddReactionCallback(message, this);
            // Reactions take a while to add, don't wait for them
            _ = Task.Run(async () =>
            {
                //await message.AddReactionAsync(options.First);
                await message.AddReactionAsync(options.Back);
                await message.AddReactionAsync(options.Next);
                //await message.AddReactionAsync(options.Last);

                var manageMessages = (Context.Channel is IGuildChannel guildChannel)
                    ? (Context.User as IGuildUser).GetPermissions(guildChannel).ManageMessages
                    : false;

                /*if (options.JumpDisplayOptions == JumpDisplayOptions.Always
                    || (options.JumpDisplayOptions == JumpDisplayOptions.WithManageMessages && manageMessages))
                    await message.AddReactionAsync(options.Jump);
                */
                //await message.AddReactionAsync(options.Stop);

                //if (options.DisplayInformationIcon)
                    //await message.AddReactionAsync(options.Info);
            });
            // TODO: (Next major version) timeouts need to be handled at the service-level!
            if (Timeout.HasValue && Timeout.Value != null)
            {
                _ = Task.Delay(Timeout.Value).ContinueWith(_ =>
                {
                    Interactive.RemoveReactionCallback(message);
                    _ = Message.DeleteAsync();
                });
            }
        }

        public async Task<bool> HandleCallbackAsync(SocketReaction reaction)
        {
            var emote = reaction.Emote;

            if (emote.Equals(options.First))
                page = 1;
            else if (emote.Equals(options.Next))
            {
                if (page >= pages)
                    return false;
                ++page;
            }
            else if (emote.Equals(options.Back))
            {
                if (page <= 1)
                    return false;
                --page;
            }
            else if (emote.Equals(options.Last))
                page = pages;
            else if (emote.Equals(options.Stop))
            {
                await Message.DeleteAsync().ConfigureAwait(false);
                return true;
            }
            else if (emote.Equals(options.Jump))
            {
                _ = Task.Run(async () =>
                {
                    var criteria = new Criteria<SocketMessage>()
                        .AddCriterion(new EnsureSourceChannelCriterion())
                        .AddCriterion(new EnsureFromUserCriterion(reaction.UserId))
                        .AddCriterion(new EnsureIsIntegerCriterion());
                    var response = await Interactive.NextMessageAsync(Context, criteria, TimeSpan.FromSeconds(15));
                    var request = int.Parse(response.Content);
                    if (request < 1 || request > pages)
                    {
                        _ = response.DeleteAsync().ConfigureAwait(false);
                        await Interactive.ReplyAndDeleteAsync(Context, options.Stop.Name);
                        return;
                    }
                    page = request;
                    _ = response.DeleteAsync().ConfigureAwait(false);
                    await RenderAsync().ConfigureAwait(false);
                });
            }
            else if (emote.Equals(options.Info))
            {
                await Interactive.ReplyAndDeleteAsync(Context, options.InformationText, timeout: options.InfoTimeout);
                return false;
            }
            _ = Message.RemoveReactionAsync(reaction.Emote, reaction.User.Value);
            await RenderAsync().ConfigureAwait(false);
            return false;
        }

        protected virtual Embed BuildEmbed()
        {


            if(InteractiveService.paginatedEmbedType == 2)
            {
                    // Type is World embed!!!
                if (pageList == null || pageList.Count == 0)
                {
                    foreach (dynamic world in _pager.Pages)
                    {
                        pageList.Add(world);
                    }
                    currentPage = 0;
                }
                else
                {
                    currentPage = currentPage + 1;
                }

                string WorldThumb = pageList[page - 1].WorldImage;
                string WorldName = pageList[page - 1].WorldName;
                int MaxPlayers = pageList[page - 1].MaxPlayers;
                DateTime date = pageList[page - 1].DateCreated;
                string author = pageList[page - 1].AuthorName;
                
                var builder = new EmbedBuilder()
                .WithAuthor(WorldName)
                .WithThumbnailUrl(WorldThumb)
                .WithDescription("created by " + author)
                .WithColor(_pager.Color)
                .WithImageUrl(WorldThumb)
                .WithFooter(f => f.Text = string.Format(options.FooterFormat, page, pages))
                .WithTitle(WorldName)
                .AddField("Time created", date.ToString(), true);

                return builder.Build();

            }else if(InteractiveService.paginatedEmbedType == 1)
            {
                
        

            if (pageList == null || pageList.Count == 0)
            {
                foreach (dynamic friend in _pager.Pages)
                {
                    pageList.Add(friend);
                }
                currentPage = 0;
            }
            else
            {
                currentPage = currentPage + 1;
            }

            string currentAvatarThumb = pageList[page - 1].currentAvatarThumbnailImageUrl;
            string currentName = pageList[page - 1].displayName;
            string currentBio = pageList[page - 1].bio;
            bool supporter = (pageList[page - 1].tags.Contains("system_supporter") ? true: false);
            string status = pageList[page - 1].statusDescription;
            
            
            if (status is null || status == "")
                status = "none";

            

            string taglist = "";
            
            foreach(string tag in pageList[page - 1].tags)
            {
                taglist += tag + "\n";
            }

            if (taglist is null || taglist == "")
                taglist = "none";

            dynamic trustlist = pageList[page - 1].tags;


            string currentTrustRank = "";

            
           
            currentTrustRank = GetCurrentTrustRank(trustlist).Result;


            
            var builder = new EmbedBuilder()
                .WithAuthor(currentName)
                .WithThumbnailUrl(currentAvatarThumb)
                .WithDescription(currentBio)
                .WithColor(_pager.Color)
                .WithImageUrl(currentAvatarThumb)
                .WithFooter(f => f.Text = string.Format(options.FooterFormat, page, pages))
                .WithTitle(currentName)
                .AddField("Status", status, true)
                .AddField("TurstRank", currentTrustRank)
                .AddField("Supporter", supporter, true)
                .AddField("Tags", taglist, false);


            if (_pager.Pages is IEnumerable<EmbedFieldBuilder> efb)
            {
                builder.Fields = efb.Skip((page - 1) * options.FieldsPerPage).Take(options.FieldsPerPage).ToList();
                builder.Description = _pager.AlternateDescription;
            } 
            else
            {
                ////builder.ImageUrl = _pager.Pages.ElementAt(page - 1).ToString();
                
                builder.Description = currentBio;
            }
            
            return builder.Build();
            }
            return null;
        }
        private async Task RenderAsync()
        {
            var embed = BuildEmbed();
            await Message.ModifyAsync(m => m.Embed = embed).ConfigureAwait(true);
        }

        public async Task<string> GetCurrentTrustRank(dynamic tags)
        {
            
                if (tags.IndexOf("troll") != -1)
                {
                    return "Confirmed troll.";
                }
                else if (tags.IndexOf("admin_moderator") != -1)
                {
                    return "Administrator";
                }
                else if (tags.IndexOf("system_legend") != -1)
                {
                    return "Legendary User and appears as Trusted User";
                }
                else if (tags.IndexOf("system_trust_legend") != -1)
                {
                    return "Veteran User and appears as Trusted User";
                }
                else if (tags.IndexOf("system_trust_veteran") != -1)
                {
                    return "Trusted User";
                }
                else if (tags.IndexOf("system_trust_trusted") != -1)
                {
                    return "Known User";
                }
                else if (tags.IndexOf("system_trust_known") != -1)
                {
                    return "User";
                }
                else if (tags.IndexOf("system_trust_basic") != -1)
                {
                    return "New User";
                }
                else
                {
                    return "Visitor";
                }
            
        }
    }
}
