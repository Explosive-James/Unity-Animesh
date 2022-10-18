using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace AnimeshEditor.Exporting
{
    internal static class DatabaseHelper
    {
        #region Public Functions
        /// <summary>
        /// Creates a collection of Unity assets into the Assets folder and creates any needed subdirectories.
        /// </summary>
        /// <param name="directory">Asset relative path to creates assets in.</param>
        /// <param name="assets">Collection of assets to create / save.</param>
        public static void CreateUnityAssets(string directory, params Object[] assets)
        {
            // Creating the directory in the assets folder if it doesn't already exist.
            string fullDirectory = Application.dataPath + directory;

            if (!Directory.Exists(fullDirectory)) {
                Directory.CreateDirectory(fullDirectory);
            }

            // Saving assets to the database.
            foreach (Object asset in assets) {

                string assetDirectory = $"Assets{directory}/{asset.name}.asset";
                AssetDatabase.CreateAsset(asset, assetDirectory);
            }

            // If the database isn't refreshed the user might not be able to see the files.
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Returns a directory that doesn't contain any invalid characters.
        /// </summary>
        /// <param name="directory">Directory to get a valid version of.</param>
        public static string GetValidDirectory(string directory)
        {
            // Spltting the directory to rebuild it folder by folder.
            string[] subdirectory = directory.Split('/', '\\');
            string result = string.Empty;

            for (int i = 0; i < subdirectory.Length; i++)
                if (subdirectory[i].Length > 0) {

                    // Removing illegal characters from the directory so the folder can be created.
                    string folderName = RemoveInvalidCharacters(subdirectory[i], Path.GetInvalidPathChars());

                    // If the folder name was all illegal characters then the folder should be ignored.
                    if (folderName.Length > 0) result += $"/{folderName}";
                }

            return result;
        }
        /// <summary>
        /// Returns a file name that doesn't contain any invalid characters.
        /// </summary>
        /// <param name="filename">Filename to get a valid version of.</param>
        public static string GetValidFilename(string filename)
        {
            return RemoveInvalidCharacters(filename, Path.GetInvalidFileNameChars());
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Removes and invalid characters from the target string.
        /// </summary>
        /// <param name="target">String to remove the invalid characters from.</param>
        /// <param name="invalidCharacters">Invalid characters to remove.</param>
        private static string RemoveInvalidCharacters(string target, char[] invalidCharacters)
        {
            // Invalid characters that shouldn't appear in the result.
            HashSet<char> invalidHashSet = new HashSet<char>(invalidCharacters);
            string result = string.Empty;

            // Removing the invalid characters.
            for (int i = 0; i < target.Length; i++)
                if (!invalidHashSet.Contains(target[i])) {

                    result += target[i];
                }

            return result;
        }
        #endregion
    }
}
