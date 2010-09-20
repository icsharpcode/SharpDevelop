// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.XmlForms 
{
	/// <summary>
	/// This class wrapps a string to an object, the string is accessible
	/// through the 'Text' property. This class was written for setting the
	/// items in a combobox inside a xml definition.
	/// </summary>
	public class StringWrapper 
	{
		string text;
		
		/// <summary>
		/// Get/Set the string.
		/// </summary>
		public string Text {
			get {
				return text;
			}
			set {
				text = value;
			}
		}
		
		/// <summary>
		/// returns <code>Text</code>
		/// </summary>
		public override string ToString()
		{
			return text;
		}
	}
}
