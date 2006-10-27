// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
	public sealed class LocalizedPropertyAttribute : Attribute
	{
		string name        = String.Empty;
		string description = String.Empty;
		string category    = String.Empty;
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		public string Category {
			get {
				return category;
			}
			set {
				category = value;
			}
		}
		
		public LocalizedPropertyAttribute(string name)
		{
			this.name = name;
		}
	}
}
