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
