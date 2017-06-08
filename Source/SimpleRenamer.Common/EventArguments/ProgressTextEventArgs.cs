using System;

namespace Sarjee.SimpleRenamer.Common.EventArguments
{
    /// <summary>
    /// ProgressTextEventArgs
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class ProgressTextEventArgs : EventArgs
    {
        private string _text;
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressTextEventArgs" /> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <exception cref="System.ArgumentNullException">text</exception>
        public ProgressTextEventArgs(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }
            _text = text;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get { return _text; }
        }
    }
}
