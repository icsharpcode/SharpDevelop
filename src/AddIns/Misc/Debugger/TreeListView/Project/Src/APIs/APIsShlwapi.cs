using System;

namespace System.Runtime.InteropServices.APIs
{
	public class APIsShlwapi
	{
		#region PathCompactPath
		/// <summary>
		/// Truncates a file path to fit within a given pixel width by replacing path components with ellipses.
		/// </summary>
		/// <param name="hDC"></param>
		/// <param name="lpszPath"></param>
		/// <param name="dx"></param>
		/// <returns>Returns TRUE if the path was successfully compacted to the specified width. Returns FALSE on failure, or if the base portion of the path would not fit the specified width</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathCompactPath(
			IntPtr hDC,
			System.Text.StringBuilder lpszPath,
			int dx);
		#endregion
		#region PathCompactPathEx
		/// <summary>
		/// Truncates a path to fit within a certain number of characters by replacing path components with ellipses
		/// </summary>
		/// <param name="pszOut"></param>
		/// <param name="pszSrc"></param>
		/// <param name="cchMax"></param>
		/// <param name="dwFlags"></param>
		/// <returns>Returns TRUE if successful, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathCompactPathEx(
			System.Text.StringBuilder pszOut,
			System.Text.StringBuilder pszSrc,
			int cchMax,
			int dwFlags);
		#endregion
		#region PathIsFileSpec
		/// <summary>
		/// Searches a path for any path delimiting characters (for example, ':' or '\' ). If there are no path delimiting characters present, the path is considered to be a File Spec path
		/// </summary>
		/// <param name="path">Path searched</param>
		/// <returns>Returns TRUE if there are no path delimiting characters within the path, or FALSE if there are path delimiting characters</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsFileSpec(
			string path);
		#endregion
		#region PathIsPrefix
		/// <summary>
		/// Searches a path to determine if it contains a valid prefix of the type passed by pszPrefix. A prefix is one of these types: "C:\\", ".", "..", "..\\"
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the compared path is the full prefix for the path, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsPrefix(
			string prefix,
			string path);
		#endregion
		#region PathIsRelative
		/// <summary>
		/// Searches a path and determines if it is relative
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the path is relative, or FALSE if it is absolute</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsRelative(
			string path);
		#endregion
		#region PathIsRoot
		/// <summary>
		/// Parses a path to determine if it is a directory root
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the specified path is a root, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsRoot(
			string path);
		#endregion
		#region PathIsSameRoot
		/// <summary>
		/// Compares two paths to determine if they have a common root component
		/// </summary>
		/// <param name="path1"></param>
		/// <param name="path2"></param>
		/// <returns>Returns TRUE if both strings have the same root component, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsSameRoot(
			string path1,
			string path2);
		#endregion
		#region PathIsUNC
		/// <summary>
		/// Determines if the string is a valid UNC (universal naming convention) for a server and share path
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the string is a valid UNC path, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsUNC(
			string path);
		#endregion
		#region PathIsUNCServer
		/// <summary>
		/// Determines if a string is a valid UNC (universal naming convention) for a server path only
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the string is a valid UNC path for a server only (no share name), or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsUNCServer(
			string path);
		#endregion
		#region PathIsUNCServerShare
		/// <summary>
		/// Determines if a string is a valid universal naming convention (UNC) share path, \\server\share
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the string is in the form \\server\share, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsUNCServerShare(
			string path);
		#endregion
		#region PathIsURL
		/// <summary>
		/// Tests a given string to determine if it conforms to a valid URL format
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if pszPath has a valid URL format, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathIsURL(
			string path);
		#endregion
		#region PathMakePretty
		/// <summary>
		/// Converts a path to all lowercase characters to give the path a consistent appearance
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns TRUE if the path has been converted, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathMakePretty(
			System.Text.StringBuilder path);
		#endregion
		#region PathMatchSpec
		/// <summary>
		/// Searches a string using a DOS wild card match type
		/// </summary>
		/// <param name="fileparam">Contains the path to be searched</param>
		/// <param name="spec">Contains the file type for which to search. For example, to test whether or not pszFileParam is a DOC file, pszSpec should be set to "*.doc"</param>
		/// <returns>Returns TRUE if the string matches, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathMatchSpec(
			string fileparam,
			string spec);
		#endregion
		#region PathParseIconLocation
		/// <summary>
		/// Parses a file location string containing a file location and icon index, and returns separate values
		/// </summary>
		/// <param name="IconFile">Contains a file location string. It should be in the form "pathname,iconindex". When the function returns, pszIconFile will point to the file's pathname</param>
		/// <returns>Returns the valid icon index value</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern int PathParseIconLocation(
			System.Text.StringBuilder IconFile);
		#endregion
		#region PathQuoteSpaces
		/// <summary>
		/// Searches a path for spaces. If spaces are found, the entire path is enclosed in quotation marks
		/// </summary>
		/// <param name="lpsz">Path to search</param>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern void PathQuoteSpaces(
			System.Text.StringBuilder lpsz);
		#endregion
		#region PathRelativePathTo
		/// <summary>
		/// Creates a relative path from one file or folder to another
		/// </summary>
		/// <param name="path"></param>
		/// <param name="from"></param>
		/// <param name="attrFrom"></param>
		/// <param name="to"></param>
		/// <param name="attrTo"></param>
		/// <returns>Returns TRUE if successful, or FALSE otherwise</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern bool PathRelativePathTo(
			System.Text.StringBuilder path,
			string from,
			int attrFrom,
			string to,
			int attrTo);
		#endregion
		#region PathRemoveArgs
		/// <summary>
		/// Removes any arguments from a given path
		/// </summary>
		/// <param name="path"></param>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern void PathRemoveArgs(
			System.Text.StringBuilder path);
		#endregion
		#region PathRemoveBackslash
		/// <summary>
		/// Removes the trailing backslash from a given path
		/// </summary>
		/// <param name="path"></param>
		/// <returns>Returns the address of the NULL that replaced the backslash, or the address of the last character if it's not a backslash</returns>
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern string PathRemoveBackslash(
			string path);
		#endregion
		#region DllGetVersion
		[DllImport("shlwapi.dll", CharSet=CharSet.Auto)]
		static public extern int DllGetVersion(
			ref APIsStructs.DLLVERSIONINFO2 path);
		#endregion
	}
}
