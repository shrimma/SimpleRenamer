﻿
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace SimpleRenamer.Framework
{
    public static class FileWatcher
    {
        /// <summary>
        /// Chcecks the program settings for the list of watch folders 
        /// and goes through of all these looking for video files.
        /// </summary>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        public static List<string> SearchTheseFolders(Settings settings)
        {
            List<string> foundFiles = new List<string>();

            //FOR EACH WATCH FOLDER
            foreach (string folder in settings.WatchFolders)
            {
                //if the directory exists and contains at least 1 file (search sub directories if settings allow) -- limitation of searchPattern means we can't filter video extensions here
                if (Directory.Exists(folder) && Directory.GetFiles(folder, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Length > 0)
                {
                    //search the folder for files with video extensions
                    List<string> temp = SearchThisFolder(folder, settings);
                    //if we find any files here add to the global list
                    if (temp.Count > 0)
                    {
                        foundFiles.AddRange(temp);
                    }
                }
            }

            return foundFiles;
        }

        /// <summary>
        /// Searches a given folder for all video files
        /// </summary>
        /// <param name="dir">The folder to search</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private static List<string> SearchThisFolder(string dir, Settings settings)
        {
            List<string> foundFiles = new List<string>();
            foreach (string file in Directory.GetFiles(dir, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (IsValidExtension(Path.GetExtension(file), settings))
                {
                    foundFiles.Add(file);
                }
            }

            return foundFiles;
        }

        public static async Task<List<string>> SearchTheseFoldersAsync(Settings settings)
        {
            List<string> foundFiles = new List<string>();

            //FOR EACH WATCH FOLDER
            foreach (string folder in settings.WatchFolders)
            {
                //if the directory exists and contains at least 1 file (search sub directories if settings allow) -- limitation of searchPattern means we can't filter video extensions here
                if (Directory.Exists(folder) && Directory.GetFiles(folder, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Length > 0)
                {
                    //search the folder for files with video extensions
                    List<string> temp = await SearchThisFolderAsync(folder, settings);
                    //if we find any files here add to the global list
                    if (temp.Count > 0)
                    {
                        foundFiles.AddRange(temp);
                    }
                }
            }

            return foundFiles;
        }

        /// <summary>
        /// Searches a given folder for all video files
        /// </summary>
        /// <param name="dir">The folder to search</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private static async Task<List<string>> SearchThisFolderAsync(string dir, Settings settings)
        {
            List<string> foundFiles = new List<string>();
            foreach (string file in Directory.GetFiles(dir, "*", settings.SubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                if (IsValidExtension(Path.GetExtension(file), settings))
                {
                    foundFiles.Add(file);
                }
            }

            return foundFiles;
        }

        /// <summary>
        /// Returns true if the input extension is one of our valid extensions
        /// </summary>
        /// <param name="input">The input extension</param>
        /// <param name="settings">Our current settings</param>
        /// <returns></returns>
        private static bool IsValidExtension(string input, Settings settings)
        {
            foreach (string extension in settings.ValidExtensions)
            {
                if (input.ToLowerInvariant().Equals(extension.ToLowerInvariant()))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
