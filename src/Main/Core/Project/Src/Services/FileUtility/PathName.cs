// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Represents a path to a file or directory.
	/// </summary>
	public abstract class PathName
	{
		protected readonly string normalizedPath;
		
		protected PathName(string path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			if (path.Length == 0)
				throw new ArgumentException("The empty string is not a valid path");
			this.normalizedPath = FileUtility.NormalizePath(path);
		}
		
		protected PathName(PathName path)
		{
			if (path == null)
				throw new ArgumentNullException("path");
			this.normalizedPath = path.normalizedPath;
		}
		
		public static implicit operator string(PathName path)
		{
			if (path != null)
				return path.normalizedPath;
			else
				return null;
		}
		
		public override string ToString()
		{
			return normalizedPath;
		}
		
		/// <summary>
		/// Gets whether this path is relative.
		/// </summary>
		public bool IsRelative {
			get { return !Path.IsPathRooted(normalizedPath); }
		}
		
		/// <summary>
		/// Gets the directory name.
		/// </summary>
		/// <remarks>
		/// Corresponds to <c>System.IO.Path.GetDirectoryName</c>
		/// </remarks>
		public DirectoryName GetParentDirectory()
		{
			if (normalizedPath.Length < 2 || normalizedPath[1] != ':')
				return DirectoryName.Create(Path.Combine(normalizedPath, ".."));
			else
				return DirectoryName.Create(Path.GetDirectoryName(normalizedPath));
		}
	}
}
