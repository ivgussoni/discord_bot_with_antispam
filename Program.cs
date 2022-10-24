using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DIscordBotOptions;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordBot
{
    class Program
    {
        static void Main(string[] args) => new Program().RunBotAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        public CommandService _commands;
        private IServiceProvider _services;

        private ulong LogChannelID;

        AntiSpam? AntiSpamer;

        public async Task RunBotAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            _client.Log += _client_Log;

            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, EnvVariables.DiscordBotToken);

            await _client.StartAsync();

            AntiSpamer = new();

            await Task.Delay(-1);
        }

        private Task _client_Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message.Author.IsBot)
            {
                return;
            }

            ulong UserDiscordID = message.Author.Id;

            if (!AntiSpamer.IsSpammer(UserDiscordID))
            {
                int argPos = 0;
                var context = new SocketCommandContext(_client, message);
                var channel = _client.GetChannel(LogChannelID) as SocketTextChannel;

                if (message.HasStringPrefix("/", ref argPos))
                {
                    var result = await _commands.ExecuteAsync(context, argPos, _services);
                    if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
                    if (result.Error.Equals(CommandError.UnmetPrecondition)) await message.Channel.SendMessageAsync(result.ErrorReason);
                }
                else
                {
                    var text = message.ToString().ToLower();

                    if (message.HasStringPrefix("!", ref argPos))
                    {
                        switch (text)
                        {
                            case "!ping":
                                await message.Channel.SendMessageAsync("Pong " + message.Author.Username + "!");
                                break;
                            default:
#if DEBUG
                                await message.Channel.SendMessageAsync("(DEBUG) Message removed from the chat.");
#endif
                                await message.DeleteAsync();
                                break;
                        }
                    }
                    else
                    {
                        await message.DeleteAsync();
                    }
                }
            }
            else
            {
                await message.DeleteAsync();
                await message.Channel.SendMessageAsync(string.Format("Hey {0}, you're going too fast, wait {1} secs to send a new message!", message.Author.Username, EnvVariables.MessageDelay / 1000));
            }
        }
    }
}
