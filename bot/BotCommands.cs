using System;

namespace bot
{
    public record BREW(int id) : BotCommand;
    public record WAIT() : BotCommand;
    public record REST() : BotCommand;
    public record CAST(int id) : BotCommand;
}