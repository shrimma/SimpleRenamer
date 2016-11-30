using System.Collections.Generic;

namespace SimpleRenamer.Common.Interface
{
    public interface IHelper
    {
        /// <summary>
        /// Checks whether the input is a valid file extension
        /// </summary>
        /// <param name="fExt">The file extension to process</param>
        /// <returns>True if a valid file extension</returns>
        bool IsFileExtensionValid(string fExt);

        bool AreListsEqual<T>(List<T> listOne, List<T> listTwo);
    }
}
