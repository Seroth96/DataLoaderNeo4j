﻿using LinqToTwitter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataLoader
{
    class LinqToTwitter
    {

        private static TwitterContext _twitterCtx { get; set; }

        public static async Task SingleStatusQueryAsync(TwitterContext twitterCtx)
        {
            _twitterCtx = twitterCtx;
            // ulong tweetID = 806571633754284032;

            //Random gen = new Random();
            //DateTime randomDate = new DateTime(2018, 1, 1);
            //int range = (DateTime.Today - randomDate).Days;
            //randomDate = randomDate.AddDays(gen.Next(range));

            var searchResponse =
                await
                (from search in twitterCtx.Search
                 where search.Type == SearchType.Search &&
                       search.IncludeEntities == true &&
                       search.ResultType == ResultType.Mixed &&
                       search.TweetMode == TweetMode.Extended &&
                       search.SearchLanguage == "en" &&
                       search.Count == 10 &&
                       search.Query == "\"a\""// &&
             //          search.Until == randomDate
                 select search)
                .SingleOrDefaultAsync();

            foreach (var t in searchResponse.Statuses)
            {                
                if (t != null && t.User != null)

                    MessageBox.Show(String.Format(
                                "User: {0}, \nTweet: {1}",
                                t.User.ScreenNameResponse,
                                t.FullText.StripHTMLandURLs()), "Tweet status"
                                , MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                await GetTweetHistoryUpwardsAsync(t);

            }   
        }

        public static async Task GetTweetHistoryUpwardsAsync(Status status)
        {
            if (status.InReplyToStatusID != 0)
            {
                var searchResponse =
                await
                (from search in _twitterCtx.Status
                 where search.Type == StatusType.Show &&
                       search.ID == status.InReplyToStatusID &&
                       search.IncludeEntities == true &&
                       search.IncludeAltText == true &&
                       search.TweetMode == TweetMode.Extended 
                 select search)
                .SingleOrDefaultAsync();

                MessageBox.Show(String.Format(
                            "User: {0}, \n\nTweet:\n\t {1}",
                            searchResponse.User.ScreenNameResponse,
                            searchResponse.FullText.StripHTMLandURLs()), "Tweet status", 
                            MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                await GetTweetHistoryUpwardsAsync(searchResponse);
            }
            else
            {
                return;
            }
        }
    }
}