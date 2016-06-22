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
using System.IO;
using System.Text;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Utility class that converts from long filenames to DOS 8.3 filenames based on the
	/// rough algorithm outlined here: http://support.microsoft.com/kb/142982/EN-US/
	/// apart from using an underscore instead of a '~'.
	/// </summary>
	public sealed class ShortFileName
	{
		const int MaximumFileNameWithoutExtensionLength = 8;
		
		/// <summary>
		/// The filename length to use when appending "_1" to it.
		/// </summary>
		const int FileNameWithoutExtensionTruncatedLength = 6;
		
		/// <summary>
		/// Maximum short filename extension length including dot.
		/// </summary>
		const int MaximumFileNameExtensionLength = 4;
		
		/// <summary>
		/// Maximum short filename length including extension.
		/// </summary>
		public const int MaximumFileNameLength = MaximumFileNameWithoutExtensionLength + MaximumFileNameExtensionLength;
		
		/// <summary>
		/// Called by Convert to check whether the specified filename exists. If it 
		/// does then the FileNameExists delegate is called repeatedly until it 
		/// finds a filename that does not exist.
		/// </summary>
		public delegate bool NameExists(string name);
		
		ShortFileName()
		{
		}
		
		/// <summary>
		/// Converts a long filename to a short 8.3 filename.
		/// </summary>
		/// <param name="fileName">The long filename.</param>
		/// <returns>The converted 8.3 filename. If the filename includes a
		/// starting path then this is stripped from the returned value.</returns>
		public static string Convert(string fileName, NameExists getFileNameExists)
		{
			if (fileName == null) {
				return String.Empty;
			}
			
			// Remove invalid characters.
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);
			StringBuilder shortFileName = new StringBuilder();
			int length = 0;
			foreach (char ch in fileNameWithoutExtension) {
				if (IsValidShortFileNameChar(ch)) {
					shortFileName.Append(ch);
					++length;
				}
			}
			
			// Get truncated extension.
			if (extension.Length > MaximumFileNameExtensionLength) {
				extension = extension.Substring(0, MaximumFileNameExtensionLength);
			} 
			
			// Truncate filename without extension if it is too long.
			if (length > MaximumFileNameWithoutExtensionLength) {
				shortFileName.Remove(FileNameWithoutExtensionTruncatedLength, length - FileNameWithoutExtensionTruncatedLength);
				return GetTruncatedFileName(shortFileName.ToString(), extension, getFileNameExists);
			}
			
			// Add extension.
			shortFileName.Append(extension);
			return shortFileName.ToString().ToUpperInvariant();
		}
		
		/// <summary>
		/// Converts a long filename to a short 8.3 filename.
		/// </summary>
		/// <param name="fileName">The long filename</param>
		/// <returns>The converted 8.3 filename. If the filename includes a
		/// starting path then this is stripped from the returned value.</returns>
		public static string Convert(string fileName)
		{
			return Convert(fileName, FileNameNeverExists);
		}
		
		/// <summary>
		/// Gets the unique name string by adding a digit until a unique name is
		/// produced. The name string itself will be truncated as the number
		/// increases in size to make sure the total length of the string is 
		/// always equal to start.Length + 1.
		/// </summary>
		/// <param name="start">The string to be used at the start of the </param>
		/// <param name="getNameExists">Method called to check that the
		/// name does not already exist.</param>
		public static string GetUniqueName(string start, NameExists getNameExists)
		{
			return GetUniqueName(start, String.Empty, String.Empty, getNameExists);
		}
		
		/// <summary>
		/// Gets the unique name string by adding a digit until a unique name is
		/// produced. The name string itself will be truncated as the number
		/// increases in size to make sure the total length of the string is 
		/// always equal to start.Length + numberPrefix.Length + 1 + end.Length.
		/// </summary>
		/// <param name="start">The string to be used at the start of the </param>
		/// <param name="numberPrefix">The string to be prefixed to the number.</param>
		/// <param name="end">The string to be added after the number.</param>
		/// <param name="getNameExists">Method called to check that the
		/// name does not already exist.</param>
		public static string GetUniqueName(string start, string numberPrefix, string end, NameExists getNameExists)
		{
			int count = 0;
			string truncatedName;
			int divisor = 10;
			do { 
				++count;
				int removeCharCount = count / divisor;
				if (removeCharCount > 0) {
					start = start.Substring(0, start.Length - 1);
					divisor *= divisor;
				}
				truncatedName = String.Concat(start, numberPrefix, count.ToString(), end);
			} while (getNameExists(truncatedName));

			return truncatedName;
		}
		
		/// <summary>
		/// Determines whether the specified filename is a long name
		/// and should be converted into a short name.
		/// </summary>
		public static bool IsLongFileName(string fileName)
		{
			if (fileName == null) {
				return false;
			}

			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);

			return fileNameWithoutExtension.Length > MaximumFileNameWithoutExtensionLength ||
				extension.Length > MaximumFileNameExtensionLength;
		}
		
		/// <summary>
		/// Truncates the filename start and adds "_N" where N produces a filename that
		/// does not exist.
		/// </summary>
		static string GetTruncatedFileName(string fileNameStart, string extension, NameExists getFileNameExists)
		{
			return GetUniqueName(fileNameStart.ToUpperInvariant(), "_", extension.ToUpperInvariant(), getFileNameExists);
		}
		
		/// <summary>
		/// Default delegate that always returns false to the Convert method.
		/// </summary>
		static bool FileNameNeverExists(string fileName)
		{
			return false;
		}
		
		/// <summary>
		/// Returns whether the character is considered a valid character
		/// for the starting part of the filename (i.e. not including the extension).
		/// </summary>
		/// <remarks>
		/// We include the period character here to avoid ICE03 invalid filename
		/// errors.
		/// 
		/// http://msdn.microsoft.com/library/en-us/msi/setup/ice03.asp 
		/// </remarks>
		static bool IsValidShortFileNameChar(char ch)
		{
			switch (ch) {
				case ' ':
				case '[':
				case ']':
				case ';':
				case '=':
				case ',':
				case '.':
					return false;
			}
			return true;
		}
	}
}
