using System;

namespace SimpleRenamer.EventArguments
{
    public class ExtensionEventArgs : EventArgs
    {
        public ExtensionEventArgs(string s)
        {
            extension = s;
        }
        private string extension;
        public string Extension
        {
            get { return extension; }
        }
    }
}
