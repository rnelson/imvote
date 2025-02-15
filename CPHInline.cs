using System;
using System.Collections.Concurrent;
using System.Linq;

public class CPHInline
{
    private const int DEFAULT_VOTE_TIME = 30; // seconds
    private readonly ConcurrentDictionary<string, int> _votes = new();

    private bool IsVoteActive()
    {
        var active = false;
        var activeString = CPH.GetGlobalVar<string>("imVoteActive", true);
        
        if (!string.IsNullOrEmpty(activeString))
        {
            active = bool.Parse(activeString);
        }

        return active;
    }

    private void SetVoteActive(bool value)
    {
        CPH.SetGlobalVar("imVoteActive", value, true);
    }

    private bool OpenVote()
    {
        var user = args["user"].ToString();
        var message = args["message"].ToString();
        var mod = bool.Parse(args["isModerator"].ToString());
        var timer = DEFAULT_VOTE_TIME;
        
        // Only allow mods to control voting
        if (!mod) return true;
        
        // Bail out if the vote is already active
        if (IsVoteActive())
        {
            return true;
        }

        // Parse the arguments
        if (args.ContainsKey("input1"))
        {
            // Allow an override of the voting time
            if (int.TryParse(args["input1"].ToString(), out var requestedTime))
                timer = requestedTime;
        }
        if (args.ContainsKey("input2"))
        {
            // If a username was provided, send a shoutout
            var shoutoutUser = args["input2"].ToString();
            CPH.TwitchSendShoutoutByLogin(shoutoutUser);
        }

        // Make sure we have no votes
        _votes.Clear();

        // Activate the vote
        SetVoteActive(true);
        CPH.SendMessage($"Voting has been opened for {timer} seconds!");

        // Wait for the time to end and close it
        CPH.Wait(1000 * timer);
        return CloseVote();
    }

    private bool LogVote()
    {
        // Grab the user and their entire message
        var user = args["user"].ToString();
        var message = args["message"].ToString();

        if (!IsVoteActive())
        {
            return true;
        }

        // Grab the value the sender selected and make sure it's valid
        if (!args.ContainsKey("input1")
            || !int.TryParse(args["input1"].ToString(), out var value)
            || value < 0
            || value > 10)
        {
            CPH.SendMessage($"@{user}: Invalid vote, must be 1 through 10");
            return true;
        }

        // Log the vote
        _votes.AddOrUpdate(user, value, (u, v) => value);
        CPH.SendMessage($"@{user}: Your vote of {value} has been set.");
        
        return true;
    }

    private bool CloseVote()
    {
        var user = args["user"].ToString();
        var message = args["message"].ToString();
        var mod = bool.Parse(args["isModerator"].ToString());
        var streamer = args["broadcastUser"].ToString();
        
        // Only allow mods to control voting
        if (!mod) return true;
        
        // Bail out if the vote is already active
        if (!IsVoteActive())
        {
            return true;
        }
        
        // Deactivate the vote
        SetVoteActive(false);
        CPH.SendMessage("Voting has been closed.");

        // Calculate and display the average
        var average = _votes.ToArray().Average(kvp => kvp.Value).ToString("#.##");
        CPH.SendMessage($"@{streamer}: Chat has spoken! The score is {average}.");

        return true;
    }

    public bool Execute()
    {
        var message = args["messageStripped"].ToString();

        if (message.StartsWith("!imopen"))
            return OpenVote();

        if (message.StartsWith("!imclose"))
            return CloseVote();

        if (message.StartsWith("!imvote"))
            return LogVote();
        
        return true;
    }
}