using System.Collections;

namespace DIscordBotOptions
{
    public class AntiSpam
    {
        Hashtable ChatTimeStamp = new();
        Hashtable NewUserChat = new();

        public AntiSpam()
        {

        }

        public void AddChatTimeStamp(ulong DiscordID)
        {
            if (!ChatTimeStamp.Contains(DiscordID))
            {
                ChatTimeStamp.Add(DiscordID, Environment.TickCount);
            }
            else
            {
                ChatTimeStamp[DiscordID] = Environment.TickCount;
            }
        }

        private void SetNewUser(ulong DiscordID)
        {
            if (!NewUserChat.Contains(DiscordID))
            {
                NewUserChat.Add(DiscordID, true);
            }
            else
            {
                if ((bool)NewUserChat[DiscordID])
                {
                    NewUserChat[DiscordID] = false;
                }
            }
        }

        private bool IsNewUserChat(ulong DiscordID)
        {
            return (bool)NewUserChat[DiscordID];
        }

        public bool IsSpammer(ulong DiscordID)
        {
            SetNewUser(DiscordID);

            if (!IsNewUserChat(DiscordID))
            {
                int TimeStamp = Convert.ToInt32(ChatTimeStamp[DiscordID]);
                int LastMessageTime = Math.Abs(TimeStamp - Environment.TickCount);

                if (LastMessageTime < EnvVariables.MessageDelay)
                {
                    return true;
                }
            }

            AddChatTimeStamp(DiscordID);

            return false;
        }
    }
}
