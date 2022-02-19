using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using WindowsHelper.ConsoleOptions.Generators.Tsp;
using WindowsHelper.Shared;

namespace WindowsHelper.Tasks.Generators.Tsp
{
    public static class GenerateRepository
    {
        public static async Task ExecuteAsync(GenerateRepositoryOptions options, TspProjectSettings settings)
        {
            var projectRoot = new DirectoryInfo(settings.ProjectRoot);
            if (!projectRoot.Exists) throw new ArgumentException(nameof(TspProjectSettings.ProjectRoot));
            if (options.Names == null || !options.Names.Any())
                throw new ArgumentNullException(nameof(GenerateRepositoryOptions.Names));

            foreach (var name in options.Names)
            {
                await GenerateInterfaceAsync(settings, name);
                await GenerateImplAsync(settings, name);
            }

            await UpdateIRepositoryWrapperAsync(settings, options.Names);
            await UpdateRepositoryWrapperAsync(settings, options.Names);
        }

        private static async Task UpdateRepositoryWrapperAsync(TspProjectSettings settings, IEnumerable<string> names)
        {
            var path = Path.Join(settings.ProjectRoot, "Repository", "Implementations", "RepositoryWrapper.cs");
            if (!File.Exists(path))
            {
                throw new ApplicationException($"Unable to locate RepositoryWrapper.cs at {path}");
            }

            Log.Information("Updating {Path}", path);
            
            try
            {
                var content = await File.ReadAllLinesAsync(path);
                var newContent = new List<string>();
            
                foreach (var line in content)
                {
                    if(line.Contains("public IRcOfficerSkillsetRepository RcOfficerSkillset"))
                    {
                        newContent.AddRange(names.Select(name => $"        private I{name}Repository {GetAsReadOnlyFieldName(name)};") .ToList());
                    } else if (line.Contains(" BeginTransaction"))
                    {
                        newContent.AddRange(names.Select(name => @$"        public I{name}Repository {name} =>
            {GetAsReadOnlyFieldName(name)} ??=
                new {name}Repository(_repoContext, _userResolverService);{Environment.NewLine}"));
                    }
                    newContent.Add(line);
                }
                await File.WriteAllLinesAsync(path, newContent);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while updating IRepositoryWrapper.cs");
            }
        }

        private static string GetAsReadOnlyFieldName(string name)
        {
            return $"_{char.ToLower(name[0])}{name[1..]}";
        }

        private static async Task UpdateIRepositoryWrapperAsync(TspProjectSettings settings, IEnumerable<string> names)
        {
            var path = Path.Join(settings.ProjectRoot, "Repository", "Contracts", "IRepositoryWrapper.cs");
            if (!File.Exists(path))
            {
                throw new ApplicationException($"Unable to locate IRepositoryWrapper.cs at {path}");
            }

            Log.Information("Updating {Path}", path);
            
            try
            {
                var content = await File.ReadAllLinesAsync(path);
                var newContent = new List<string>();
            
                foreach (var line in content)
                {
                    if(line.Contains("SaveWithAuditAsync()"))
                    {
                        newContent.AddRange(names.Select(name => $"        I{name}Repository {name} {{ get; }}") .ToList());
                    }
                    newContent.Add(line);
                }
                await File.WriteAllLinesAsync(path, newContent);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while updating IRepositoryWrapper.cs");
            }
        }
        
         private static async Task GenerateImplAsync(TspProjectSettings settings, string name)
                {
                    var repositoryTemplate = @$"using CertisRcms.Data;
using CertisRcms.Models;
using CertisRcms.Repository.Contracts;
using CertisRcms.Utils;

namespace CertisRcms.Repository.Implementations
{{
    public class {name}Repository : RepositoryBase<{name}>, I{name}Repository
    {{
        public {name}Repository(CertisRcmsDbContext context, UserResolverService userResolverService)
            : base(context, userResolverService)
        {{
        }}
    }}
}}";
                    var repositoryPath = Path.Join(settings.ProjectRoot, "Repository", "Implementations", $"{name}Repository.cs");
        
                    try
                    {
                        Log.Information("Creating {Path}", repositoryPath);
                        await File.WriteAllTextAsync(repositoryPath, repositoryTemplate);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "An error occured while creating {Path}", repositoryPath);
                    }
                }

        private static async Task GenerateInterfaceAsync(TspProjectSettings settings, string name)
        {
            var iRepositoryTemplate = @$"using CertisRcms.Models;

namespace CertisRcms.Repository.Contracts
{{
    public interface I{name}Repository : IRepositoryBase<{name}>
    {{
    }}
}}";
            var iRepositoryPath = Path.Join(settings.ProjectRoot, "Repository", "Contracts", $"I{name}Repository.cs");

            try
            {
                Log.Information("Creating {Path}", iRepositoryPath);
                await File.WriteAllTextAsync(iRepositoryPath, iRepositoryTemplate);
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured while creating {Path}", iRepositoryPath);
            }
        }
    }
}