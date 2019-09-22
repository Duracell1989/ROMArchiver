using System;
using System.Diagnostics;

namespace RomLister
{
    internal class SevenZipProcessor
    {
        internal delegate void ProgressChangedHandler(int progress, string userFriendlyProgress);
        internal delegate void CompletedHandler();
        internal delegate void ExceptionOccuredHandler(string errorMessage);
        internal event ExceptionOccuredHandler OnExceptionOccured;
        internal event ProgressChangedHandler OnProgressChanged;
        internal event CompletedHandler OnCompleted;

        internal void Cancel()
        {
            if (_process != null)
            {
                _process.Kill();
            }
        }

        private Process _process;
        private IProcessInfo _processInfo;

        internal SevenZipProcessor()
        {
            Init();
        }

        private void Init()
        {
            _process = new Process();
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.CreateNoWindow = true;
            _process.EnableRaisingEvents = true;
            _process.StartInfo.FileName = ConfigReader.Instance.SevenZipExecutablePath;
            _process.StartInfo.RedirectStandardError = true;
            _process.StartInfo.RedirectStandardOutput = true;

            _process.OutputDataReceived += Process_OutputDataReceived;
            _process.Exited += Process_Exited;
            _process.ErrorDataReceived += Process_ErrorDataReceived;
        }

        internal void Run(IProcessInfo processInfo)
        {
            _processInfo = processInfo;

            _process.StartInfo.Arguments = processInfo.Arguments;
            _process.Start();
            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();
            _process.WaitForExit();
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(e.Data))
            {
                ExceptionOccured(e.Data);
                _process.Close();
                _process.Dispose();
            }
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            _process.Close();
            _process.Dispose();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && e.Data.Contains("%"))
            {
                string shouldBeProgress = e.Data.Substring(0, e.Data.IndexOf("%"));

                if (int.TryParse(shouldBeProgress, out int progress))
                {
                    UpdateProgress(progress, _processInfo.UserfriendlyProgress(progress)); // TODO: better userfriendly message
                }
            }
        }

        private void ExceptionOccured(string errorMessage)
        {
            // Make sure someone is listening to event
            OnExceptionOccured?.Invoke(errorMessage);
        }

        private void UpdateProgress(int progress, string userFriendlyProgress)
        {
            // Make sure someone is listening to event
            OnProgressChanged?.Invoke(progress, userFriendlyProgress);
        }

        private void Complete()
        {
            // Make sure someone is listening to event
            OnCompleted?.Invoke();
        }
    }
}
