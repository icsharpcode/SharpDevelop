// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Microsoft.Win32;

namespace ICSharpCode.Reports.Core.Globals
{
	/// <summary>
	/// A utility class related to file utilities.
	/// </summary>
	public sealed class FileUtility
	{
		readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
		const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";
		// This is an arbitrary limitation built into the .NET Framework.
		// Windows supports paths up to 32k length.
		public const int MaxPathLength = 260;
		
		private FileUtility()
		{
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
		
		public static bool IsUrl(string path)
		{
			return path.IndexOf(':') >= 2;
		}
		
		/// <summary>
		/// Converts a given absolute path and a given base path to a path that leads
		/// from the base path to the absoulte path. (as a relative path)
		/// </summary>
		public static string GetRelativePath(string baseDirectoryPath, string absPath)
		{
			if (IsUrl(absPath) || IsUrl(baseDirectoryPath)){
				return absPath;
			}
			
			baseDirectoryPath = NormalizePath(baseDirectoryPath);
			absPath           = NormalizePath(absPath);
			
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
			return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
		}
		
		/// <summary>
		/// Gets the normalized version of fileName.
		/// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
		/// </summary>
		public static string NormalizePath(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return fileName;
			
			int i;
			
			bool isWeb = false;
			for (i = 0; i < fileName.Length; i++) {
				if (fileName[i] == '/' || fileName[i] == '\\')
					break;
				if (fileName[i] == ':') {
					if (i > 1)
						isWeb = true;
					break;
				}
			}
			
			char outputSeparator = isWeb ? '/' : '\\';
			
			StringBuilder result = new StringBuilder();
			if (isWeb == false && fileName.StartsWith(@"\\") || fileName.StartsWith("//")) {
				i = 2;
				result.Append(outputSeparator);
			} else {
				i = 0;
			}
			int segmentStartPos = i;
			for (; i <= fileName.Length; i++) {
				if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\') {
					int segmentLength = i - segmentStartPos;
					switch (segmentLength) {
						case 0:
							// ignore empty segment (if not in web mode)
							if (isWeb) {
								result.Append(outputSeparator);
							}
							break;
						case 1:
							// ignore /./ segment, but append other one-letter segments
							if (fileName[segmentStartPos] != '.') {
								if (result.Length > 0) result.Append(outputSeparator);
								result.Append(fileName[segmentStartPos]);
							}
							break;
						case 2:
							if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.') {
								// remove previous segment
								int j;
								for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--);
								if (j > 0) {
									result.Length = j;
								}
								break;
							} else {
								// append normal segment
								goto default;
							}
						default:
							if (result.Length > 0) result.Append(outputSeparator);
							result.Append(fileName, segmentStartPos, segmentLength);
							break;
					}
					segmentStartPos = i + 1; // remember start position for next segment
				}
			}
			if (isWeb == false) {
				if (result.Length > 0 && result[result.Length - 1] == outputSeparator) {
					result.Length -= 1;
				}
				if (result.Length == 2 && result[1] == ':') {
					result.Append(outputSeparator);
				}
			}
			return result.ToString();
		}
		
		public static bool IsEqualFileName(string fileName1, string fileName2)
		{
			return string.Equals(NormalizePath(fileName1),
			                     NormalizePath(fileName2),
			                     StringComparison.OrdinalIgnoreCase);
		}
		
		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			try {
				baseDirectory = NormalizePath(baseDirectory).ToUpperInvariant();
				testDirectory = NormalizePath(testDirectory).ToUpperInvariant();
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
			fileName     = NormalizePath(fileName);
			oldDirectory = NormalizePath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			newDirectory = NormalizePath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			if (IsBaseDirectory(oldDirectory, fileName)) {
				if (fileName.Length == oldDirectory.Length) {
					return newDirectory;
				}
				return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
			}
			return fileName;
		}
		
		
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
			if (String.IsNullOrEmpty(name)) {
				throw new ArgumentNullException("name");
			}
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
		
		public static bool IsDirectory(string filename)
		{
			if (String.IsNullOrEmpty(filename)) {
				throw new ArgumentNullException("filename");
			}
			
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
			filename = filename.ToUpper(System.Globalization.CultureInfo.InstalledUICulture);
			pattern = pattern.ToUpper();
			string[] patterns = pattern.Split(';');
			foreach (string p in patterns) {
				if (Match(filename, p)) {
					return true;
				}
			}
			return false;
		}
	}
}
