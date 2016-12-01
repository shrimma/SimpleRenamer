using Sarjee.SimpleRenamer.Common.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sarjee.SimpleRenamer.Common
{
    public class Helper : IHelper
    {
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

        public bool AreListsEqual<T>(List<T> listOne, List<T> listTwo)
        {
            return (listOne.Count == listTwo.Count) && !listOne.Except(listTwo).Any();
        }
    }
}
