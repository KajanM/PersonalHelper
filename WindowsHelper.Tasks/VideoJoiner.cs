using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsHelper.ConsoleOptions;
using CliWrap;

namespace WindowsHelper.Tasks
{
    public class VideoJoiner
    {
        private readonly JoinMultipleVideosOptions _options;

        public VideoJoiner(JoinMultipleVideosOptions options)
        {
            _options = options;
            _options.Path ??= Environment.CurrentDirectory;
            
            if(!Directory.Exists(options.Path)) throw new ArgumentException($"Directory does not exist: {options.Path}");
        }
        
        public async Task<bool> JoinVideosAsync()
        {
            var directory = new DirectoryInfo(_options.Path);
            var concatenatedFileNames = string.Join(" ", directory.GetFiles().OrderBy(f => f.Name).Select(f => $"\"{f.FullName}\"").ToList());
            var stdOutBuffer = new StringBuilder();
            var stdErrBuffer = new StringBuilder();

            var command = "\"" + _options.VlcPath + "\"" + $" {concatenatedFileNames} -sout \"#gather:std{{access=file,dst=output.mp4}}\" -sout-keep";

            if (_options.IsDryRun)
            {
                Console.WriteLine(command); 
            }
            else
            {
                Cli.Wrap($"\"{_options.VlcPath}\"")
                    .WithArguments($"{concatenatedFileNames} --sout \"#gather:std{{access=file,dst=output.mp4}}\" --no-sout-all --sout-keep")
                    .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
                    .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                    .ExecuteAsync();
                // command = "echo hai";
                // System.Diagnostics.Process.Start("CMD.exe", $"/C {command}");
                Console.WriteLine($"output{Environment.NewLine}");
                Console.WriteLine(stdOutBuffer.ToString());

                Console.WriteLine($"error{Environment.NewLine}");
                Console.WriteLine(stdErrBuffer.ToString());
            }
            
            return true;
        }
    }
}