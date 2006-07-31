// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WixBinding
{
	/// <summary>
	/// Represents a name value pair of the form 'name=value'.
	/// </summary>
	public class NameValuePair
	{
		string name = String.Empty;
		string value = String.Empty;
		
		public NameValuePair(string name, string value)
		{
			this.name = name;
			this.value = value;
		}
		
		public NameValuePair(string name) : this(name, String.Empty)
		{
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Value {
			get {
				return value;
			}
		}
		
		public override string ToString()
		{
			return String.Concat(name, "=", value);
		}
	}
}
