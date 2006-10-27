// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Describes an object that can create instances of classes that
	/// implement <see cref="IResourceFileContent"/>.
	/// </summary>
	public interface IResourceFileContentFactory
	{
		/// <summary>
		/// Determines whether this factory can create a resource file content
		/// for the specified file.
		/// </summary>
		/// <param name="fileName">The file name to examine.</param>
		/// <returns><c>true</c>, if this factory can create a resource file content for the specified file, otherwise <c>false</c>.</returns>
		bool CanCreateContentForFile(string fileName);
		
		/// <summary>
		/// Creates a resource file content for the specified file.
		/// </summary>
		/// <param name="fileName">The name of the file to create the resource file content for.</param>
		/// <returns>A new instance of a class that implements <see cref="IResourceFileContent"/> and represents the content of the specified file, or <c>null</c>, if this class cannot handle the file format.</returns>
		IResourceFileContent CreateContentForFile(string fileName);
	}
}
