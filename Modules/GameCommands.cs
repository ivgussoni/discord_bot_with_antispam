namespace DIscordBotOptions
{
    public class GameCommands
    {
        private static readonly Dictionary<string, string> UsersCommands = new()
        {
            { "!ping", "You will get a PONG" },
            { "!1234", "Lorem ipsum" },
            { "!5678", "Lorem ipsum" },
            { "!91011", "Lorem ipsum" },
            { "!12131", "Lorem ipsum" }
        };

        public static string GetCommands()
        {
            string CommandsList = string.Empty;

            foreach (KeyValuePair<string, string> GameCmds in UsersCommands)
            {
                CommandsList += string.Format("{0} - {1}\n", GameCmds.Key, GameCmds.Value);
            }

            return CommandsList;
        }

        public bool IsValidCommand(string command)
        {
            if (UsersCommands.ContainsKey(command))
            {
                return true;
            }

            return false;
        }
    }
}
