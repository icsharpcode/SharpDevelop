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
	/// Describes the content of multiple resource files.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi")]
	public interface IMultiResourceFileContent : IResourceFileContent
	{
		
		/// <summary>
		/// Gets the file name of the resource file the specified key is in.
		/// </summary>
		/// <returns>The name of the resource file the specified key is in, or <c>null</c> if the key cannot be found in any resource file this instance represents.</returns>
		string GetFileNameForKey(string key);
		
	}
}
