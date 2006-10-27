// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Describes an object that can find localized resources that belong to a
	/// master resource.
	/// </summary>
	public interface ILocalizedResourcesFinder
	{
		/// <summary>
		/// Gets localized resources that belong to the master resource file.
		/// </summary>
		/// <param name="fileName">The name of the master resource file.</param>
		/// <returns>A dictionary of culture names and associated resource file contents, or <c>null</c>, if there are none.</returns>
		IDictionary<string, IResourceFileContent> GetLocalizedContents(string fileName);
	}
}
