// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
