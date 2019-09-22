using System;
using System.Collections.Generic;
using System.Text;

namespace RomLister
{
    internal class CompressInfo : IProcessInfo
    {
        private string _fileName;
        private string _archiveName;
        private string _workingDir;
        private List<string> _arguments = new List<string>();

        public string Arguments
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("a");
                sb.Append($" \"{_archiveName}\"");
                sb.Append($" \"{_fileName}\"");

                foreach (string argument in _arguments)
                {
                    sb.Append($" {argument}");
                }
                return sb.ToString();
            }
        }

        internal CompressInfo(string fileName, string archiveName, string workingDir)
        {
            _fileName = fileName;
            _archiveName = archiveName;
            _workingDir = workingDir;
            _arguments.Add("-bso0"); // Set output to disable
            _arguments.Add("-bsp1"); // Set progress to stdout
            _arguments.Add("-bse2"); // Set error to stderr
            _arguments.Add($"-mmt={Environment.ProcessorCount}"); // Set number of CPU threads
            _arguments.Add("-mx=9"); // Sets level of compression (9=ultra)
            _arguments.Add("-ms=on"); // Enables solid mode
            _arguments.Add("-sdel"); // Delete files after compression
            _arguments.Add("-mhc=on"); //Enables archive header compressing.
            _arguments.Add("-ma=1"); // Sets compression mode: 0 = fast, 1 = normal. Default value is 1.
            _arguments.Add("-mmf=bt4"); // Sets Match Finder for LZMA. Default method is bt4 [bt2;bt3;bt4;hc4]
            _arguments.Add("-mmc=1000000000"); //Sets number of cycles (passes) for match finder. [0 ... 1000000000]

            /*
            Sets Dictionary size for LZMA [2^(1...30); 1536m]
            If you do not specify any symbol from the set [b|k|m], the dictionary size will be calculated as DictionarySize = 2^Size bytes
            */
            _arguments.Add("-md=1536m"); //Sets Dictionary size for LZMA to highest available
            _arguments.Add("-mfb=273"); // Sets number of fast bytes for LZMA. (5 ... 273)
            _arguments.Add("-y");
            _arguments.Add($" -w\"{_workingDir}\"");
        }

        public string UserfriendlyProgress(int progress)
        {
            return $"Compressing: {progress}%";
        }
    }
}