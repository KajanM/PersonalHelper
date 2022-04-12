using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions;

namespace WindowsHelper.Tasks
{
    public static class AlterDb
    {
        public static async Task ExecuteAsync(AlterDbOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.FileName)) throw new ArgumentNullException(nameof(options.FileName));

            var sqlPath = Path.Combine(options.Path, $"{options.FileName}.sql");
            if (!File.Exists(sqlPath)) throw new ArgumentException($"file does not exist at {sqlPath}");
            var overridenSqlPath = Path.Combine(options.Path, $"{options.FileName}-o.sql");
            await using var writer = new StreamWriter(overridenSqlPath);
            var newDbName = DateTime.Now.ToString("yyMMddmm");
            const string prodDbName = "certisrcmsdb";
            var linesToIgnore = new List<string>
            {
                "INSERT INTO `RcOfficerSkillsetSapExportJobStatusChangeRecord`",
                "INSERT INTO `RcOfficerSkillsetSapExportRecordIncludedSkillset`",
                "INSERT INTO `RcOfficerSkillsetSapImportRowProcessingResults`",
                "INSERT INTO `AuditLogs`",
            }.AsReadOnly();
            
            Log.Information("Starting to process {SourcePath}", sqlPath);
            
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            foreach (var line in File.ReadLines(sqlPath).Where(l => linesToIgnore.All(l.StartsWith)))
            {
                await writer.WriteLineAsync(line.Contains(prodDbName)
                    ? line.Replace(prodDbName, newDbName)
                    : line);
            }

            stopWatch.Stop();
            Log.Information("Saved to {DestinationPat}. Took {Minutes} minutes", overridenSqlPath,
                stopWatch.Elapsed.TotalMinutes);
            Console.WriteLine("enter a key to exist");
            Console.ReadLine();
        }
    }
}