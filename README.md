tokinonagare
============


userstream Twitter bot (using linqtotwitter) based on c# ( .net 4.0 or higher)


how to use it
===============

in program.cs..

<code>
<pre>
        static async Task ReplyAsync(TwitterContext twitterCtx, ulong tweetid,string status)
        {
            var tweet = await twitterCtx.ReplyAsync(tweetid, "@changeityours "+status);  // &lt;- change it
                if (tweet != null)
                    Console.WriteLine(
                        "Status returned: " +
                        "(" + tweet.StatusID + ")" +
                        tweet.User.Name + ", " + tweetid + ", " +
                        tweet.Text + "\n");
        }
        
        ...
        
        static bool chkengine(string msg)
        {
            string mentchk = mentidparser(msg);
            if (mentchk.ToLower() == "mentioncheckstring") // &lt;- change it
                
                string idchk = idparser(msg);
                if (idchk.ToLower() == "idcheckstring") // &lt;- change it
                {
                    return true;
                }
            }
            return false;
        }
        
</pre>
</code>

in App.config..
<code>
<pre>

    &lt;add key="consumerSecret" value="insertyourAPIkeySecret" /&gt;
    &lt;add key="ClientSettingsProvider.ServiceUri" value="" /&gt;
  
  </pre>
 </code>
 
 
change two key values.<br>
and build it<br>
END.
    

