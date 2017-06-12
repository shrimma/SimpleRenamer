using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sarjee.SimpleRenamer.Common
{
    /// <summary>
    /// Helper
    /// </summary>
    /// <seealso cref="Sarjee.SimpleRenamer.Common.Interface.IHelper" />
    public class Helper : IHelper
    {
        /// <summary>
        /// Checks whether the input is a valid file extension
        /// </summary>
        /// <param name="fExt">The file extension to process</param>
        /// <returns>
        /// True if a valid file extension
        /// </returns>
        public bool IsFileExtensionValid(string fExt)
        {
            bool answer = true;
            if (!String.IsNullOrWhiteSpace(fExt) && fExt.Length > 1 && fExt[0] == '.')
            {
                char[] invalidFileChars = Path.GetInvalidFileNameChars();
                foreach (char c in invalidFileChars)
                {
                    if (fExt.Contains(c.ToString()))
                    {
                        answer = false;
                        break;
                    }
                }
            }
            else
            {
                answer = false;
            }
            return answer;
        }

        /// <summary>
        /// Checks if the input lists are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOne">The list one.</param>
        /// <param name="listTwo">The list two.</param>
        /// <returns></returns>
        public bool AreListsEqual<T>(List<T> listOne, List<T> listTwo)
        {
            if (listOne.Count != listTwo.Count)
            {
                return false;
            }
            if (listOne.Except(listTwo).Any())
            {
                return false;
            }
            if (listTwo.Except(listOne).Any())
            {
                return false;
            }

            return true;
        }

        private static string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
        private Regex specialCharacterRegex = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
        /// <summary>
        /// Removes the special characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public string RemoveSpecialCharacters(string input)
        {
            return specialCharacterRegex.Replace(input, "");
        }
    }
}
