// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
