// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates path names using a relative to the folder containing the addin file.
	/// </summary>
	/// <attribute name="path" use="required">
	/// Path relative to the directory which contains the .addin file defining the codon.
	/// </attribute>
	/// <usage>Where directory paths to a folder inside the addin directory are expected, e.g.
	/// /SharpDevelop/BackendBindings/Templates</usage>
	/// <returns>
	/// A string containing the full path name.
	/// </returns>
	public class DirectoryDoozer : IDoozer
	{
		public bool HandleConditions { get { return false; } }
		
		public object BuildItem(BuildItemArgs args)
		{
			return Path.Combine(Path.GetDirectoryName(args.AddIn.FileName), args.Codon["path"]);
		}
	}
}
