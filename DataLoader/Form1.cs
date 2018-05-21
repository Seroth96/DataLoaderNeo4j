using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataLoader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {            
            //HelloWorldExample.RunExample();
            try
            {
                DoDemosAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }


        static async Task DoDemosAsync()
        {
            IAuthorizer auth = ChooseAuthenticationStrategy();

            await auth.AuthorizeAsync();

            // This is how you access credentials after authorization.
            // The oauthToken and oauthTokenSecret do not expire.
            // You can use the userID to associate the credentials with the user.
            // You can save credentials any way you want - database, isolated storage, etc. - it's up to you.
            // You can retrieve and load all 4 credentials on subsequent queries to avoid the need to re-authorize.
            // When you've loaded all 4 credentials, LINQ to Twitter will let you make queries without re-authorizing.
            //
            //var credentials = auth.CredentialStore;
            //string oauthToken = credentials.OAuthToken;
            //string oauthTokenSecret = credentials.OAuthTokenSecret;
            //string screenName = credentials.ScreenName;
            //ulong userID = credentials.UserID;
            //

            var twitterCtx = new TwitterContext(auth);

            await LinqToTwitter.SingleStatusQueryAsync(twitterCtx);

        }

        public static void Set(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Machine);
        }


        static IAuthorizer ChooseAuthenticationStrategy()
        {
            //    Console.WriteLine("Authentication Strategy:\n\n");

            //    Console.WriteLine("  1 - Pin (default)");
            //    Console.WriteLine("  2 - Application-Only");
            //    Console.WriteLine("  3 - Single User");
            //    Console.WriteLine("  4 - XAuth");

            //    Console.Write("\nPlease choose (1, 2, 3, or 4): ");
            //    ConsoleKeyInfo input = Console.ReadKey();
            //    Console.WriteLine("");

            IAuthorizer auth = null;

            //switch (input.Key)
            //{

            //    case ConsoleKey.D1:
            //        auth = DoPinOAuth();
            //        break;
            //    case ConsoleKey.D2:
            auth = DoApplicationOnlyAuth();
            //        break;
            //    case ConsoleKey.D3:
            //        auth = DoSingleUserAuth();
            //        break;
            //    case ConsoleKey.D4:
            //        auth = DoXAuth();
            //        break;
            //    default:
            //        auth = DoPinOAuth();
            //        break;
            //}

            return auth;
        }

        static IAuthorizer DoPinOAuth()
        {
            var auth = new PinAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
                GoToTwitterAuthorization = pageLink => Process.Start(pageLink),
                GetPin = () =>
                {
                    Console.WriteLine(
                        "\nAfter authorizing this application, Twitter " +
                        "will give you a 7-digit PIN Number.\n");
                    Console.Write("Enter the PIN number here: ");
                    return Console.ReadLine();
                }
            };

            return auth;
        }

        static IAuthorizer DoApplicationOnlyAuth()
        {
            var test = Environment.GetEnvironmentVariables();
            var auth = new ApplicationOnlyAuthorizer()
            {
                CredentialStore = new InMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret)
                },
            };

            return auth;
        }
        static IAuthorizer DoSingleUserAuth()
        {
            var auth = new SingleUserAuthorizer
            {
                CredentialStore = new SingleUserInMemoryCredentialStore
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    AccessToken = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessToken),
                    AccessTokenSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterAccessTokenSecret)
                }
            };

            return auth;
        }

        static IAuthorizer DoXAuth()
        {
            var auth = new XAuthAuthorizer
            {
                CredentialStore = new XAuthCredentials
                {
                    ConsumerKey = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerKey),
                    ConsumerSecret = Environment.GetEnvironmentVariable(OAuthKeys.TwitterConsumerSecret),
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            return auth;
        }
    }
}

