using System;
using CliWrap;
using CliWrap.Buffered;
using Serilog;

namespace WindowsHelper.Services.Helpers.Http
{
    public class CliWrapperHelper
    {
        public static BufferedCommandResult Execute(CliWrapperOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.ExecutablePath))
                throw new ArgumentNullException(nameof(options.ExecutablePath));

            var commandResult = Cli.Wrap(options.ExecutablePath)
                .WithWorkingDirectory(options.WorkingDirectory)
                .WithArguments(options.Arguments)
                .ExecuteBufferedAsync().Task.Result;
           
            Log.Information("Command completed in {Duration}", commandResult.RunTime);
            Log.Information(commandResult.StandardOutput);
            Log.Information(commandResult.StandardError);

            return commandResult;
        }
    }
}