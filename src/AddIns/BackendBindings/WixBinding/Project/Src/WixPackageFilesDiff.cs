// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.IO;

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
				fileNames.Add(fileElement.FileName);
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
	}
}
