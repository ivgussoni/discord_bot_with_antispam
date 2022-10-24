using Discord.Commands;

namespace DIscordBotOptions
{
    public class BotCommands : ModuleBase<SocketCommandContext>
    {
        [Command("commands")]
        public async Task Comands()
        {
            await ReplyAsync(GameCommands.GetCommands());
        }

        [Command("botime")]
        public async Task Time()
        {
            await ReplyAsync(string.Format("Time is: {0}", DateTime.Now));
        }
    }
}
