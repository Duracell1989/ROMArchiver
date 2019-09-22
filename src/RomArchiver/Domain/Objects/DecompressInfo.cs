using System.Collections.Generic;
using System.Text;

namespace RomLister
{
    internal class DecompressInfo : IProcessInfo
    {
        private string _archiveName;
        private string _workingDir;
        private List<string> _arguments = new List<string>();

        public string Arguments
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("e");

                foreach (string argument in _arguments)
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
            _workingDir = workingDir;
            _arguments.Add("-bso0"); // Set output to disable
            _arguments.Add("-bsp1"); // Set progress to stdout
            _arguments.Add("-bse2"); // Set error to stderr
            _arguments.Add("-y"); // Yes to all
            _arguments.Add($"-o\"{_workingDir}\""); // Set output dir
        }

        public string UserfriendlyProgress(int progress)
        {
            return $"Decompressing: {progress}%";
        }
    }
}