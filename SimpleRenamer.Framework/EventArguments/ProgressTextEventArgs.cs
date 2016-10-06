using System;

namespace SimpleRenamer.Framework.EventArguments
{
    public class ProgressTextEventArgs : EventArgs
    {
        private string text;
        public ProgressTextEventArgs(string textToDisplay)
        {
            text = textToDisplay;
        }

        public string Text
        {
            get { return text; }
        }
    }
}
