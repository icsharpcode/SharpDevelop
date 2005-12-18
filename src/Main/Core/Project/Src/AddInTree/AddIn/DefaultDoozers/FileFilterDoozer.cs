// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates file filter entries for OpenFileDialogs or SaveFileDialogs.
	/// </summary>
	/// <attribute name="name" use="required">
	/// The name of the file filter entry.
	/// </attribute>
	/// <attribute name="extensions" use="required">
	/// The extensions associated with this file filter entry.
	/// </attribute>
	/// <usage>Only in /SharpDevelop/Workbench/FileFilter</usage>
	/// <returns>
	/// String in the format "name|extensions".
	/// </returns>
	public class FileFilterDoozer : IDoozer
	{
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return StringParser.Parse(codon.Properties["name"]) + "|" + codon.Properties["extensions"];
		}
	}
}
