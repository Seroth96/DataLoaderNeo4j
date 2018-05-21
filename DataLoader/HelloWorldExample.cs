using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataLoader
{
    public class HelloWorldExample : IDisposable
    {
        private readonly IDriver _driver;

        public HelloWorldExample(string uri, string user, string password)
        {
            _driver = GraphDatabase.Driver(uri, AuthTokens.Basic(user, password));
        }

        public void Dispose()
        {
            _driver?.Dispose();
        }
              
        public static void AddTweetToGraph(Message msg, Source src)
        {
            using (var context = new HelloWorldExample("bolt://localhost:7687", "neo4j", "localneo4j"))
            {
                context.AddTweet(msg, src);
            }
        }

        private void AddTweet(Message msg, Source src)
        {
            using (var session = _driver.Session())
            {
               /* var tweet =*/ session.WriteTransaction(tx =>
                {
                    var result = tx.Run("MERGE (s:SOURCE {name: $src.Name}) " +
                                        "CREATE (m:MESSAGE:RAW {id:$msg.ID, text:$msg.Text, favoriteCount:$msg.FavoriteCount, retweetCount:$msg.RetweetCount, loaded:$timestamp}) " +
                                        "CREATE (m)-[r:FROM]->(s) " +
                                        @"RETURN 'ID: ' + m.id + '\n ' + " +
                                        @"'Text: ' + m.text + '\n ' + " +
                                        @"'Fovorite: ' + m.favoriteCount + '\n ' + " +
                                        @"'Retweets: ' + m.retweetCount + '\n ' + " +
                                        @"'Loaded: ' +  m.loaded + ' \n ' + 'from node ' + id(m)",
                        new { msg, src, timestamp = DateTime.Now.Ticks });
                    //return result.Single()[0].As<string>();
                });
               // MessageBox.Show(tweet);
            }
        }

        public static void AddTweetRelation(Message msg1, Message msg2, Source src)
        {
            using (var context = new HelloWorldExample("bolt://localhost:7687", "neo4j", "localneo4j"))
            {
                using (var session = context._driver.Session())
                {//TODO: MERGE (s:SOURCE {name: $src.Name}) 
                    session.WriteTransaction(tx =>
                    {
                        tx.Run("MATCH (m1:MESSAGE {id: $msg1.ID}) " +
                            "MERGE(s: SOURCE { name: $src.Name})" +
                            "CREATE (m2:MESSAGE:RAW {id:$msg2.ID, text:$msg2.Text, favoriteCount:$msg2.FavoriteCount, retweetCount:$msg2.RetweetCount, loaded:$timestamp}) " +
                            "CREATE (m2)-[from:FROM]->(s) " +
                            "CREATE (m1)-[r:PRECEDES]->(m2) " +
                            "RETURN m1,m2,r" ,
                            new { msg1, msg2, timestamp = DateTime.Now.Ticks, src });
                    });
                }
            }
        }

    }
}
