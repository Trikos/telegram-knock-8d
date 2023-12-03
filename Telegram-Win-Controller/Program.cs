using System.Media;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


var botClient = new TelegramBotClient("bot_token_here");
var assets = "\\Telegram-Win-Controller\\assets";

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    PlayKnock(assets);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}

void PlayKnock(string path)
{
#if WINDOWS
    // Check if the operating system is Windows
    if (Environment.OSVersion.Platform == PlatformID.Win32NT)
    {
        // If it's Windows, create a new SoundPlayer and play the sound
        var player = new SoundPlayer($"{path}\\knock.wav");
        player.Play();
    }
    else
    {
        // If it's not Windows, display a message indicating that sound cannot be played
        Console.WriteLine("The operating system is not Windows. Unable to play the sound.");
    }
#else
    Console.WriteLine("The WINDOWS conditional compilation directive is not defined. Unable to play the sound.");
#endif
}
