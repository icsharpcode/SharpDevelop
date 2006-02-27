// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
	public static class FileUtility
	{
		// TODO: GetFullPath is a **very** expensive method (performance-wise)!
		// Call it only when necessary. (see IsEqualFile)
		
		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
		static string applicationRootPath = Environment.CurrentDirectory;
		const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";
		
		public static string ApplicationRootPath {
			get {
				return applicationRootPath;
			}
			set {
				applicationRootPath = value;
			}
		}
		
		public static string NETFrameworkInstallRoot {
			get {
				using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework")) {
					object o = installRootKey.GetValue("InstallRoot");
					return o == null ? String.Empty : o.ToString();
				}
			}
		}
		
		public static string NetSdkInstallRoot {
			get {
				using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\.NETFramework")) {
					object o = installRootKey.GetValue("sdkInstallRootv2.0");
					return o == null ? String.Empty : o.ToString();
				}
			}
		}
		
		public static string[] GetAvailableRuntimeVersions()
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
			
			string result = paths[0];
			for (int i = 1; i < paths.Length; i++) {
				result = Path.Combine(result, paths[i]);
			}
			return result;
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
				if(!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
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
		
		public static bool IsEqualFileName(string fileName1, string fileName2)
		{
			// Optimized for performance:
			//return Path.GetFullPath(fileName1.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLower() == Path.GetFullPath(fileName2.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).ToLower();
			
			if (fileName1.Length == 0 || fileName2.Length == 0) return false;
			
			char lastChar;
			lastChar = fileName1[fileName1.Length - 1];
			if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
				fileName1 = fileName1.Substring(0, fileName1.Length - 1);
			lastChar = fileName2[fileName2.Length - 1];
			if (lastChar == Path.DirectorySeparatorChar || lastChar == Path.AltDirectorySeparatorChar)
				fileName2 = fileName2.Substring(0, fileName2.Length - 1);
			
			try {
				if (fileName1.Length < 2 || fileName1[1] != ':' || fileName1.IndexOf("/.") >= 0 || fileName1.IndexOf("\\.") >= 0)
					fileName1 = Path.GetFullPath(fileName1);
				if (fileName2.Length < 2 || fileName2[1] != ':' || fileName2.IndexOf("/.") >= 0 || fileName2.IndexOf("\\.") >= 0)
					fileName2 = Path.GetFullPath(fileName2);
			} catch (Exception) { }
			return string.Equals(fileName1, fileName2, StringComparison.OrdinalIgnoreCase);
		}
		
		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			try {
				baseDirectory = Path.GetFullPath(baseDirectory).ToUpperInvariant();
				testDirectory = Path.GetFullPath(testDirectory).ToUpperInvariant();
				baseDirectory = baseDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				testDirectory = testDirectory.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				
				if (baseDirectory[baseDirectory.Length - 1] != Path.DirectorySeparatorChar)
					baseDirectory += Path.DirectorySeparatorChar;
				if (testDirectory[testDirectory.Length - 1] != Path.DirectorySeparatorChar)
					testDirectory += Path.DirectorySeparatorChar;
				
				return testDirectory.StartsWith(baseDirectory);
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
		/// The found files are added to the List&lt;string&gt;
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
		
		public static int MaxPathLength = 260;
		
		/// <summary>
		/// This method checks the file fileName if it is valid.
		/// </summary>
		public static bool IsValidFileName(string fileName)
		{
			// Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
			//        I can't find a .NET property or method for determining this variable.
			
			if (fileName == null || fileName.Length == 0 || fileName.Length >= MaxPathLength) {
				return false;
			}
			
			// platform independend : check for invalid path chars
			
			if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0) {
				return false;
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
				nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
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
		
		/// <summary>
		/// Checks that a single directory name (not the full path) is valid.
		/// </summary>
		public static bool IsValidDirectoryName(string name)
		{
			if (!IsValidFileName(name)) {
				return false;
			}
			if (name.IndexOfAny(new char[]{Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar}) >= 0) {
				return false;
			}
			if (name.Trim(' ').Length == 0) {
				return false;
			}
			return true;
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

		//TODO This code is Windows specific
		static bool MatchN (string src, int srcidx, string pattern, int patidx)
		{
			int patlen = pattern.Length;
			int srclen = src.Length;
			char next_char;

			for (;;) {
				if (patidx == patlen)
					return (srcidx == srclen);
				next_char = pattern[patidx++];
				if (next_char == '?') {
					if (srcidx == src.Length)
						return false;
					srcidx++;
				}
				else if (next_char != '*') {
					if ((srcidx == src.Length) || (src[srcidx] != next_char))
						return false;
					srcidx++;
				}
				else {
					if (patidx == pattern.Length)
						return true;
					while (srcidx < srclen) {
						if (MatchN(src, srcidx, pattern, patidx))
							return true;
						srcidx++;
					}
					return false;
				}
			}
		}

		static bool Match(string src, string pattern)
		{
			if (pattern[0] == '*') {
				// common case optimization
				int i = pattern.Length;
				int j = src.Length;
				while (--i > 0) {
					if (pattern[i] == '*')
						return MatchN(src, 0, pattern, 0);
					if (j-- == 0)
						return false;
					if ((pattern[i] != src[j]) && (pattern[i] != '?'))
						return false;
				}
				return true;
			}
			return MatchN(src, 0, pattern, 0);
		}

		public static bool MatchesPattern(string filename, string pattern)
		{
			filename = filename.ToUpper();
			pattern = pattern.ToUpper();
			string[] patterns = pattern.Split(';');
			foreach (string p in patterns) {
				if (Match(filename, p)) {
					return true;
				}
			}
			return false;
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
