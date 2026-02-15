using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using WandererAttendance.Services.Logging;

namespace PutIsland;

public static class Utils {
    public static ILoggerFactory Logger() {
        return LoggerFactory.Create(c => {
            c.AddConsoleFormatter<ClassIslandConsoleFormatter,
                ConsoleFormatterOptions>();
            c.AddConsole(console => { console.FormatterName = "classisland"; });
            c.SetMinimumLevel(LogLevel.Trace);
        });
    }
}