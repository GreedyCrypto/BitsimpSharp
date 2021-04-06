using Flurl.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace VRChatN.Configuration
{
    class Settings
    {

        // PATH Setup


        //API
        public static string APIKey;
        public static string APIBase;

        //Endpoints
        public static string login_endpoint;
        public static string userinfo_endpoint;
        public static string friends_endpoint;
        public static string users_endpoint;
        public static string activeusers_endpoint;
        public static string recentworlds_endpoint;
        public static string favouriteworlds_endpoint;
        public static string usersearch_endpoint;

        public static FlurlCookie AuthCookie; 

        public static string vrcusername { get; internal set; }
        public static string vrcpassword { get; internal set; }
        public static string discordtoken { get; internal set; }
    }
}
