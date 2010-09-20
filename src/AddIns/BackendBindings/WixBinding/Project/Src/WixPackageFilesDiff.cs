// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Compares the files specified in the WixDocument against those
	/// on the file system and returns any differences.
	/// </summary>
	public class WixPackageFilesDiff
	{
		IDirectoryReader directoryReader;
		List<string> searchedDirectories;
		ExcludedNames excludedFileNames = new ExcludedNames();
		
		public WixPackageFilesDiff() : this(new DirectoryReader())
		{
		}
		
		public WixPackageFilesDiff(IDirectoryReader directoryReader)
		{
			this.directoryReader = directoryReader;
		}
		
		/// <summary>
		/// Gets the list of filenames to exclude.
		/// </summary>
		/// <remarks>
		/// Each filename in this list does not include its full path. 
		/// </remarks>
		public ExcludedNames ExcludedFileNames {
			get {
				return excludedFileNames;
			}
		}
		
		/// <summary>
		/// Compares the files defined in the WixDirectoryElement against those
		/// on the file system and returns any differences.
		/// </summary>
		public WixPackageFilesDiffResult[] Compare(WixDirectoryElementBase directoryElement)
		{
			List<string> wixPackageFiles = GetAllFiles(directoryElement);	
			List<string> files = new List<string>();
			
			// Find all files on the file system based on the directories
			// used in the Wix document.
			searchedDirectories = new List<string>();
			foreach (string fileName in wixPackageFiles) {
				string directory = Path.GetDirectoryName(fileName);
				if (!HasDirectoryBeenSearched(directory)) {
					if (directoryReader.DirectoryExists(directory)) {
						foreach (string directoryFileName in directoryReader.GetFiles(directory)) {
							if (!excludedFileNames.IsExcluded(directoryFileName)) {
								files.Add(Path.Combine(directory, directoryFileName));
							}
						}
					}
				}
			}
			
			// Look for new files.
			List<string> missingFiles = new List<string>();
			List<string> removedFiles = new List<string>();
			foreach (string fileName in wixPackageFiles) {
				int index = GetFileNameIndex(files, fileName);
				if (index >= 0) {
					removedFiles.Add(files[index]);
					files.RemoveAt(index);
				} else {
					// Check that this file has not already been removed.
					index = GetFileNameIndex(removedFiles, fileName);
					if (index == -1) {
						missingFiles.Add(fileName);
					}
				}
			}
						
			// Add new files.
			List<WixPackageFilesDiffResult> results = new List<WixPackageFilesDiffResult>();
			foreach (string fileName in files) {
				results.Add(new WixPackageFilesDiffResult(fileName, WixPackageFilesDiffResultType.NewFile));
			}
			
			// Add missing files.
			foreach (string fileName in missingFiles) {
				results.Add(new WixPackageFilesDiffResult(fileName, WixPackageFilesDiffResultType.MissingFile));
			}

			// Add new directories.
			results.AddRange(GetNewDirectories());
			
			return results.ToArray();
		}
		
		List<string> GetAllFiles(WixDirectoryElementBase directoryElement)
		{
			List<string> files = new List<string>();
			
			// Get all the child directory elements.
			foreach (WixDirectoryElement childDirectoryElement in directoryElement.GetDirectories()) {
				files.AddRange(GetAllFiles(childDirectoryElement));
			}
			
			// Get all the files from any child components.
			foreach (WixComponentElement componentElement in directoryElement.GetComponents()) {
				files.AddRange(GetFileNames(componentElement.GetFiles()));
			}
			
			return files;
		}
		
		List<string> GetFileNames(WixFileElement[] fileElements)
		{
			List<string> fileNames = new List<string>();
			foreach (WixFileElement fileElement in fileElements) {
				fileNames.Add(fileElement.GetSourceFullPath());
			}
			return fileNames;
		}
		
		/// <summary>
		/// Finds the filename in the files list.
		/// </summary>
		/// <returns>The index at which the filename exists in the list.</returns>
		int GetFileNameIndex(List<string> files, string fileName)
		{
			for (int i = 0; i < files.Count; ++i) {
				string currentFileName = files[i];
				if (FileUtility.IsEqualFileName(currentFileName, fileName)) {
					return i;
				}
			}
			return -1;
		}
		
		/// <summary>
		/// Makes sure that directories are only searched once.
		/// </summary>
		bool HasDirectoryBeenSearched(string directory)
		{
			directory = directory.ToLowerInvariant();
			if (searchedDirectories.Contains(directory)) {
				return true;
			}
	
			// Directory not found.
			searchedDirectories.Add(directory);
			return false;
		}
		
		/// <summary>
		/// Looks for new subdirectories that are not included in those searched. 
		/// </summary>
		/// <remarks>
		/// TODO: Should not be using ExcludedFileNames to check whether the directory should be added.
		/// </remarks>
		List<WixPackageFilesDiffResult> GetNewDirectories()
		{
			List<WixPackageFilesDiffResult> results = new List<WixPackageFilesDiffResult>();
			string[] directories = new string[searchedDirectories.Count];
			searchedDirectories.CopyTo(directories);
			foreach (string directory in directories) {
				foreach (string subDirectory in directoryReader.GetDirectories(directory)) {
					DirectoryInfo dirInfo = new DirectoryInfo(subDirectory);
					if (!excludedFileNames.IsExcluded(dirInfo.Name)) {
						if (!HasDirectoryBeenSearched(subDirectory)) {
							results.Add(new WixPackageFilesDiffResult(subDirectory, WixPackageFilesDiffResultType.NewDirectory));
						}
					}
				}
			}
			return results;
		}
	}
}
