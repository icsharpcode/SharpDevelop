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

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Creates resource file contents for .resources and .resx files.
	/// </summary>
	public class DefaultBclResourceFileContentFactory : IResourceFileContentFactory
	{
		/// <summary>
		/// Determines whether this factory can create a resource file content
		/// for the specified file.
		/// </summary>
		/// <param name="fileName">The file name to examine.</param>
		/// <returns><c>true</c>, if this factory can create a resource file content for the specified file, otherwise <c>false</c>.</returns>
		public bool CanCreateContentForFile(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			
			if (ext.Equals(".resources", StringComparison.OrdinalIgnoreCase) ||
			    ext.Equals(".resx", StringComparison.OrdinalIgnoreCase)) {
				return true;
			}
			
			return false;
		}
		
		/// <summary>
		/// Creates a resource file content for the specified file.
		/// </summary>
		/// <param name="fileName">The name of the file to create the resource file content for.</param>
		/// <returns>A new instance of a class that implements <see cref="IResourceFileContent"/> and represents the content of the specified file, or <c>null</c>, if this class cannot handle the file format.</returns>
		public IResourceFileContent CreateContentForFile(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			
			if (ext.Equals(".resources", StringComparison.OrdinalIgnoreCase)) {
				return new ResourcesResourceFileContent(fileName);
			} else if (ext.Equals(".resx", StringComparison.OrdinalIgnoreCase)) {
				return new ResXResourceFileContent(fileName);
			}
			
			return null;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultBclResourceFileContentFactory"/> class.
		/// </summary>
		public DefaultBclResourceFileContentFactory()
		{
		}
	}
}
