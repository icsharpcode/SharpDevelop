// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
