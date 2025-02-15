# Iron Mario Voting

[DGR_Bot](https://www.twitch.tv/DGR_Bot) was having some issues (sounds like maybe memory limits on its hosting?) while [Dave](https://www.twitch.tv/DGR_Dave) was trying to run the Iron Mario Epic Fails contest. Contest voting was put on hold until voting can be sorted out.

Learning how to work with [Streamer.bot](https://streamer.bot) had been on my list for a while, so I took this as an opportunity to learn how to write a custom handler.

I only tested this briefly with a single voter (myself), and because my stream wasn't live I'm not sure if the shoutout functionality works, but this may work as-is and is a good starting point if nothing else. I welcome [contributions](https://github.com/rnelson/imvote/pulls)!

## Setup

Adding this to your Streamer.bot setup is pretty simple. You just need to add a new action, give it a trigger of a Twitch chat message, and add a sub-action of executing C# code.

![Action](https://github.com/rnelson/imvote/blob/main/img/01-new_action.png)

When you add the C# code sub-action, it will open up a new window. Replace the entire text with the contents of [CPHInline.cs](https://github.com/rnelson/imvote/blob/main/CPHInline.cs). Then, click the *Find Refs* button. You should see something like this:

![References](https://github.com/rnelson/imvote/blob/main/img/02-refs.png)

Simply click *Save and Compile* and you're ready to go!

## Usage

| Command    | Users       | Arguments                                                    | Description     |
|------------|-------------|--------------------------------------------------------------|-----------------|
| `!imopen`  | Moderators  | `time` (seconds:integer), `streamer` (for a shoutout:string) | Opens voting    |
| `!imvote`  | Anyone      | `score` (vote from 1 to 10:integer)                          | Submits a vote  |
| `!imclose` | Moderators  |                                                              | Closes voting   |

![Sample runs](https://github.com/rnelson/imvote/blob/main/img/03-sample_runs.png)

## License

Released under the [MIT License](http://rnelson.mit-license.org).