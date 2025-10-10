using System;
using System.Windows.Forms;

namespace RomArchiver.Utils.Extensions
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    internal static class TextBoxBaseExtensions
    {
        internal static void AppendTextWithNewLine(this TextBoxBase textBoxBase, string text)
        {
            textBoxBase.AppendText($"{text}{Environment.NewLine}");
        }
    }
}
