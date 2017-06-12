using System.Collections.Generic;

namespace Sarjee.SimpleRenamer.Common.Interface
{
    /// <summary>
    /// Helper interface
    /// </summary>
    public interface IHelper
    {
        /// <summary>
        /// Checks whether the input is a valid file extension
        /// </summary>
        /// <param name="fExt">The file extension to process</param>
        /// <returns>True if a valid file extension</returns>
        bool IsFileExtensionValid(string fExt);

        /// <summary>
        /// Checks if the input lists are equal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listOne">The list one.</param>
        /// <param name="listTwo">The list two.</param>
        /// <returns></returns>
        bool AreListsEqual<T>(List<T> listOne, List<T> listTwo);

        /// <summary>
        /// Removes any special characters.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        string RemoveSpecialCharacters(string input);
    }
}
