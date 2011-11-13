// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// Creates ParserDescriptor objects for the parsing service.
	/// </summary>
	/// <attribute name="supportedextensions">
	/// Semicolon-separated list of file extensions for which the parser is used. (e.g. ".boo")
	/// </attribute>
	/// <attribute name="class">
	/// Name of the IParser class.
	/// </attribute>
	/// <usage>Only in /Workspace/Parser</usage>
	/// <returns>
	/// An ParserDescriptor object that wraps the IParser object.
	/// </returns>
	public sealed class ParserDoozer : IDoozer
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
		public object BuildItem(BuildItemArgs args)
		{
			return new ParserDescriptor(args.Codon);
		}
	}
}
