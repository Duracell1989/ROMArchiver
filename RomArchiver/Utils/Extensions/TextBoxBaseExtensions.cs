using System;
using System.Windows.Forms;

namespace RomLister.Utils.Extensions
{
    internal static class TextBoxBaseExtensions
    {
        internal static void AppendTextWithNewLine(this TextBoxBase textBoxBase, string text)
        {
            textBoxBase.AppendText($"{text}{Environment.NewLine}");
        }
    }
}
