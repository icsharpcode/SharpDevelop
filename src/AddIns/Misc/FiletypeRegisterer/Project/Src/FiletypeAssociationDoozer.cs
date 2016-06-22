// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.FiletypeRegisterer
{
	public class FiletypeAssociation
	{
		string id;
		string icon;
		string text;
		
		public FiletypeAssociation(string id, string icon, string text)
		{
			this.id = id;
			this.icon = icon;
			this.text = text;
		}
		
		public string Extension {
			get {
				return id;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
		}
		
		public string Text {
			get {
				return text;
			}
		}
	}
	
	/// <summary>
	/// Creates FiletypeAssociation instances.
	/// </summary>
	/// <attribute name="id" use="required">
	/// The extension (without dot) to be registered.
	/// </attribute>
	/// <attribute name="icon" use="required">
	/// The full path to a .ico file on the disk.
	/// </attribute>
	/// <attribute name="text" use="required">
	/// The description text.
	/// </attribute>
	/// <returns>
	/// A FiletypeAssociation describing the specified values.
	/// </returns>
	public class FiletypeAssociationDoozer : IDoozer
	{
		public static List<FiletypeAssociation> GetList()
		{
			return AddInTree.BuildItems<FiletypeAssociation>("/AddIns/FileTypeRegisterer/FileTypes", null, true);
		}
		
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
			Codon codon = args.Codon;
			return new FiletypeAssociation(codon.Id,
			                               StringParser.Parse(codon.Properties["icon"]),
			                               StringParser.Parse(codon.Properties["text"]));
		}
	}
}
