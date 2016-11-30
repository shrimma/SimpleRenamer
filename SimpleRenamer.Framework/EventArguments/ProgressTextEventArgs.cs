using System;

namespace SimpleRenamer.Common.EventArguments
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
