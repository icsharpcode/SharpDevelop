// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

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
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return Path.Combine(Path.GetDirectoryName(codon.AddIn.FileName), codon.Properties["path"]);
		}
	}
}
