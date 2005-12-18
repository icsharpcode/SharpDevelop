// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Creates ParserDescriptor objects for the parsing service.
	/// </summary>
	/// <attribute name="supportedextensions">
	/// Semicolon-separated list of file extensions for which the parser is used. (e.g. ".boo")
	/// </attribute>
	/// <attribute name="projectfileextension">
	/// File extension of project files. (e.g. ".booproj")
	/// </attribute>
	/// <attribute name="class">
	/// Name of the IParser class.
	/// </attribute>
	/// <usage>Only in /Workspace/Parser</usage>
	/// <returns>
	/// An ParserDescriptor object that wraps the IParser object.
	/// </returns>
	public class ParserDoozer : IDoozer
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
		
		/// <summary>
		/// Creates an item with the specified sub items. And the current
		/// Condition status for this item.
		/// </summary>
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new ParserDescriptor(codon);
		}
	}
}
