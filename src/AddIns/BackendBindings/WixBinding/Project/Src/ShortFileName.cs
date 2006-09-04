// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
		public delegate bool FileNameExists(string fileName);
		
		ShortFileName()
		{
		}
		
		/// <summary>
		/// Converts a long filename to a short 8.3 filename.
		/// </summary>
		/// <param name="fileName">The long filename.</param>
		/// <returns>The converted 8.3 filename. If the filename includes a
		/// starting path then this is stripped from the returned value.</returns>
		public static string Convert(string fileName, FileNameExists getFileNameExists)
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
		/// Truncates the filename start and adds "_N" where N produces a filename that
		/// does not exist.
		/// </summary>
		static string GetTruncatedFileName(string fileNameStart, string extension, FileNameExists getFileNameExists)
		{
			int count = 0;
			string truncatedFileName;
			int divisor = 10;
			do { 
				++count;
				int removeCharCount = count / divisor;
				if (removeCharCount > 0) {
					fileNameStart = fileNameStart.Substring(0, fileNameStart.Length - 1);
					divisor *= divisor;
				}
				truncatedFileName = String.Concat(fileNameStart, "_", count.ToString(), extension).ToUpperInvariant();
			} while (getFileNameExists(truncatedFileName));
			
			return truncatedFileName;
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
