using System.Collections.Generic;
using System.Text;
using RomLister;

namespace RomArchiver.Domain.Objects
{
    internal class DecompressInfo : IProcessInfo
    {
        private readonly string _archiveName;
        private readonly List<string> _arguments = new();

        public string Arguments
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append('e');

                foreach (var argument in _arguments)
                {
                    sb.Append($" {argument}");
                }

                sb.Append($" \"{_archiveName}\"");
                return sb.ToString();
            }
        }

        internal DecompressInfo(string archiveName, string workingDir)
        {
            _archiveName = archiveName;
            _arguments.Add("-bso0"); // Set output to disable
            _arguments.Add("-bsp1"); // Set progress to stdout
            _arguments.Add("-bse2"); // Set error to stderr
            _arguments.Add("-y"); // Yes to all
            _arguments.Add($"-o\"{workingDir}\""); // Set output dir
        }

        public string UserfriendlyProgress(int progress)
        {
            return $"Decompressing: {progress}%";
        }
    }
}