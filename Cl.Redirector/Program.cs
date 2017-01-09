using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Cl.Redirector
{
    class Program
    {
        const string OriginalCl = "original_cl.exe";
        static void Main(string[] args)
        {
            var sanatizedArgs = args.Where(a => !a.Equals("/Zi"));
            var fileCommands = sanatizedArgs.FirstOrDefault(a => a.StartsWith("@"));
            if (!string.IsNullOrEmpty(fileCommands))
            {
                fileCommands = fileCommands.TrimStart('@');
                var fileContent = File.ReadAllText(fileCommands);
                if (fileContent.IndexOf("/Zi") != -1)
                {
                    fileContent = fileContent.Replace("/Zi", "");
                    File.WriteAllText(fileCommands, fileContent);
                }
            }
            sanatizedArgs = sanatizedArgs.Select(a => a.IndexOf(" ") != -1 ? $"\"{a}\"" : a);
            var processInfo = new ProcessStartInfo
            {
                FileName = OriginalCl,
                Arguments = string.Join(" ", sanatizedArgs),
                UseShellExecute = false
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
        }
    }
}
