// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

namespace ICSharpCode.Core
{
	public enum FileErrorPolicy {
		Inform,
		ProvideAlternative
	}
	
	public enum FileOperationResult {
		OK,
		Failed,
		SavedAlternatively
	}
	
	public delegate void FileOperationDelegate();
	
	public delegate void NamedFileOperationDelegate(string fileName);
	
	/// <summary>
	/// A utility class related to file utilities.
	/// </summary>
	public sealed class FileUtility
	{
		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
		static string sharpDevelopRootPath;
		const string fileNameRegEx = @"^(([a-zA-Z]:)|.)[^:]*$";
		
		public static string SharpDevelopRootPath {
			get {
				return sharpDevelopRootPath;
			}
		}
		
		static FileUtility()
		{
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			// entryAssembly == null might happen in unit test mode
			if (entryAssembly != null) {
				sharpDevelopRootPath = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), "..");
			} else {
				sharpDevelopRootPath = Environment.CurrentDirectory;
			}
		}
		
		
		public static string NETFrameworkInstallRoot {
			get {
				RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework");
				object o = installRootKey.GetValue("InstallRoot");
				return o == null ? String.Empty : o.ToString();
			}
		}
		
		public static string[] GetAvaiableRuntimeVersions()
		{
			string   installRoot = NETFrameworkInstallRoot;
			string[] files       = Directory.GetDirectories(installRoot);
			
			ArrayList runtimes = new ArrayList();
			foreach (string file in files) {
				string runtime = Path.GetFileName(file);
				if (runtime.StartsWith("v")) {
					runtimes.Add(runtime);
				}
			}
			return (string[])runtimes.ToArray(typeof(string));
		}
		
		public static string Combine(params string[] paths)
		{
			if (paths == null || paths.Length == 0) {
				return String.Empty;
			}
			
			if (paths.Length == 1) {
				return paths[0];
			}
			
			string[] newPaths = new string[paths.Length - 1];
			
			newPaths[0] = Path.Combine(paths[0], paths[1]);
			
			Array.Copy(paths, 2, 
			           newPaths, 1, 
			           paths.Length - 2);
			
			return Combine(newPaths);
		}
		
		
		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			baseDirectoryPath = Path.GetFullPath(baseDirectoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			absPath           = Path.GetFullPath(absPath);
			
			string[] bPath = baseDirectoryPath.Split(separators);
			string[] aPath = absPath.Split(separators);
			int indx = 0;
			for(; indx < Math.Min(bPath.Length, aPath.Length); ++indx){
				if(!bPath[indx].ToUpper().Equals(aPath[indx].ToUpper()))
					break;
			}
			
			if (indx == 0) {
				return absPath;
			}
			
			StringBuilder erg = new StringBuilder();
			
			if(indx == bPath.Length) {
//				erg.Append('.');
//				erg.Append(Path.DirectorySeparatorChar);
			} else {
				for (int i = indx; i < bPath.Length; ++i) {
					erg.Append("..");
					erg.Append(Path.DirectorySeparatorChar);
				}
			}
			erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length-indx));
			return erg.ToString();
		}
		
		/// <summary>
		/// Converts a given relative path and a given base path to a path that leads
		/// to the relative path absoulte.
		/// </summary>
		public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
		{
			return Path.GetFullPath(Path.Combine(baseDirectoryPath, relPath));
		}
		
		public static bool IsEqualFile(string fileName1, string fileName2)
		{
			try {
				return Path.GetFullPath(fileName1.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLower() == Path.GetFullPath(fileName2.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLower();
			} catch (Exception) {
				return false;
			}
		}
		
		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			try {
				baseDirectory = Path.GetFullPath(baseDirectory.ToUpper());
				testDirectory = Path.GetFullPath(testDirectory.ToUpper());
				
				return baseDirectory.Length <= testDirectory.Length &&
						testDirectory.StartsWith(baseDirectory) &&
						(testDirectory.Length == baseDirectory.Length ||
					 	 testDirectory[baseDirectory.Length] == Path.DirectorySeparatorChar ||
					 	 testDirectory[baseDirectory.Length] == Path.AltDirectorySeparatorChar);
			} catch (Exception) {
				return false;
			}
		}
		
		public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
		{
			fileName     = Path.GetFullPath(fileName);
			oldDirectory = Path.GetFullPath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			newDirectory = Path.GetFullPath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			if (IsBaseDirectory(oldDirectory, fileName)) {
				if (fileName.Length == oldDirectory.Length) {
					return newDirectory;
				}
				return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
			}
			return fileName;
		}
		
		public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
		{
			if (!Directory.Exists(destinationDirectory)) {
				Directory.CreateDirectory(destinationDirectory);
			}
			foreach (string fileName in Directory.GetFiles(sourceDirectory)) {
				File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
			}
			foreach (string directoryName in Directory.GetDirectories(sourceDirectory)) {
				DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
			}
		}
		
		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
		{
			List<string> collection = new List<string>();
			SearchDirectory(directory, filemask, collection, searchSubdirectories, ignoreHidden);
			return collection;
		}
		
		public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
		{
			return SearchDirectory(directory, filemask, searchSubdirectories, false);
		}
		
		public static List<string> SearchDirectory(string directory, string filemask)
		{
			return SearchDirectory(directory, filemask, true, false);
		}
		
		/// <summary>
		/// Finds all files which are valid to the mask <paramref name="filemask"/> in the path
		/// <paramref name="directory"/> and all subdirectories
		/// (if <paramref name="searchSubdirectories"/> is true).
		/// The found files are added to the List<string>
		/// <paramref name="collection"/>.
		/// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
		/// </summary>
		static void SearchDirectory(string directory, string filemask, List<string> collection, bool searchSubdirectories, bool ignoreHidden)
		{
			string[] file = Directory.GetFiles(directory, filemask);
			foreach (string f in file) {
				if (ignoreHidden && (File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden) {
					continue;
				}
				collection.Add(f);
			}
			
			if (searchSubdirectories) {
				string[] dir = Directory.GetDirectories(directory);
				foreach (string d in dir) {
					if (ignoreHidden && (File.GetAttributes(d) & FileAttributes.Hidden) == FileAttributes.Hidden) {
						continue;
					}
					SearchDirectory(d, filemask, collection, searchSubdirectories, ignoreHidden);
				}
			}
		}
		
		/// <summary>
		/// This method checks the file fileName if it is valid.
		/// </summary>
		public static bool IsValidFileName(string fileName)
		{
			// Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
			//        I can't find a .NET property or method for determining this variable.
			
			if (fileName == null || fileName.Length == 0 || fileName.Length >= 260) {
				return false;
			}
			
			// platform independend : check for invalid path chars
			foreach (char invalidChar in Path.GetInvalidPathChars()) {
				if (fileName.IndexOf(invalidChar) >= 0) {
					return false;
				}
			}
			if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0) {
				return false;
			}
			
			if (!Regex.IsMatch(fileName, fileNameRegEx)) {
				return false;
			}
			
			// platform dependend : Check for invalid file names (DOS)
			// this routine checks for follwing bad file names :
			// CON, PRN, AUX, NUL, COM1-9 and LPT1-9
			
			string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			if (nameWithoutExtension != null) {
				nameWithoutExtension = nameWithoutExtension.ToUpper();
			}
			
			if (nameWithoutExtension == "CON" ||
			    nameWithoutExtension == "PRN" ||
			    nameWithoutExtension == "AUX" ||
			    nameWithoutExtension == "NUL") {
				return false;
			}
			
			char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';
			
			return !((nameWithoutExtension.StartsWith("COM") ||
			          nameWithoutExtension.StartsWith("LPT")) &&
			         Char.IsDigit(ch));
		}
		
		
		public static bool TestFileExists(string filename)
		{
			if (!File.Exists(filename)) {
				MessageService.ShowWarning(StringParser.Parse("${res:Fileutility.CantFindFileError}", new string[,] { {"FILE",  filename} }));
				return false;
			}
			return true;
		}
		
		public static bool IsDirectory(string filename)
		{
			if (!Directory.Exists(filename)) {
				return false;
			}
			FileAttributes attr = File.GetAttributes(filename);
			return (attr & FileAttributes.Directory) != 0;
		}
		
		// Observe SAVE functions
		public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
		{
			System.Diagnostics.Debug.Assert(IsValidFileName(fileName));
			try {
				saveFile();
				OnFileSaved(new FileNameEventArgs(fileName));
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, false)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK: // choose location (never happens here)
									break;
								case DialogResult.Retry:
									return ObservedSave(saveFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy)
		{
			return ObservedSave(saveFile,
			                    fileName,
			                    ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName)
		{
			return ObservedSave(saveFile, fileName, FileErrorPolicy.Inform);
		}
		
		public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			System.Diagnostics.Debug.Assert(IsValidFileName(fileName));
			try {
				string directory = Path.GetDirectoryName(fileName);
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
				saveFileAs(fileName);
				OnFileSaved(new FileNameEventArgs(fileName));
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
					restartlabel:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, true)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK:
									using (SaveFileDialog fdiag = new SaveFileDialog()) {
										fdiag.OverwritePrompt = true;
										fdiag.AddExtension    = true;
										fdiag.CheckFileExists = false;
										fdiag.CheckPathExists = true;
										fdiag.Title           = "Choose alternate file name";
										fdiag.FileName        = fileName;
										if (fdiag.ShowDialog() == DialogResult.OK) {
											return ObservedSave(saveFileAs, fdiag.FileName, message, policy);
										} else {
											goto restartlabel;
										}
									}
								case DialogResult.Retry:
									return ObservedSave(saveFileAs, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			return ObservedSave(saveFileAs,
			                    fileName,
			                    ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedSave(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		
		// Observe LOAD functions
		public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, string message, FileErrorPolicy policy)
		{
			System.Diagnostics.Debug.Assert(IsValidFileName(fileName));
			try {
				loadFile();
				OnFileLoaded(new FileNameEventArgs(fileName));
				return FileOperationResult.OK;
			} catch (Exception e) {
				switch (policy) {
					case FileErrorPolicy.Inform:
						using (SaveErrorInformDialog informDialog = new SaveErrorInformDialog(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e)) {
							informDialog.ShowDialog();
						}
						break;
					case FileErrorPolicy.ProvideAlternative:
						using (SaveErrorChooseDialog chooseDialog = new SaveErrorChooseDialog(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e, false)) {
							switch (chooseDialog.ShowDialog()) {
								case DialogResult.OK: // choose location (never happens here)
									break;
								case DialogResult.Retry:
									return ObservedLoad(loadFile, fileName, message, policy);
								case DialogResult.Ignore:
									return FileOperationResult.Failed;
							}
						}
						break;
				}
			}
			return FileOperationResult.Failed;
		}
		
		public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, FileErrorPolicy policy)
		{
			return ObservedLoad(loadFile,
			                    fileName,
			                    ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName)
		{
			return ObservedLoad(loadFile, fileName, FileErrorPolicy.Inform);
		}
		
		class LoadWrapper
		{
			NamedFileOperationDelegate loadFile;
			string fileName;
			
			public LoadWrapper(NamedFileOperationDelegate loadFile, string fileName)
			{
				this.loadFile = loadFile;
				this.fileName   = fileName;
			}
			
			public void Invoke()
			{
				loadFile(fileName);
			}
		}
		
		public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
		{
			return ObservedLoad(new FileOperationDelegate(new LoadWrapper(saveFileAs, fileName).Invoke), fileName, message, policy);
		}
		
		public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy)
		{
			return ObservedLoad(saveFileAs,
			                    fileName,
			                    ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
			                    policy);
		}
		
		public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName)
		{
			return ObservedLoad(saveFileAs, fileName, FileErrorPolicy.Inform);
		}
		
		static  void OnFileLoaded(FileNameEventArgs e) 
		{
			if (FileLoaded != null) {
				FileLoaded(null, e);
			}
		}
		
		static void OnFileSaved(FileNameEventArgs e) {
			if (FileSaved != null) {
				FileSaved(null, e);
			}
		}
		
		public static event FileNameEventHandler FileLoaded;
		public static event FileNameEventHandler FileSaved;
	}
}
