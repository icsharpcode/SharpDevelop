// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2229 $</version>
// </file>

using System;
using ICSharpCode.XmlEditor;

namespace XmlEditor.Tests.Utils
{
	/// <summary>
	/// Derived version of the AddXmlNodeDialog which allows us to test
	/// various protected methods.
	/// </summary>
	public class DerivedAddXmlNodeDialog : AddXmlNodeDialog
	{
		public DerivedAddXmlNodeDialog(string[] names) : base(names)
		{
		}
		
		/// <summary>
		/// Calls the base class's NamesListBoxSelectedIndexChanged method.
		/// </summary>
		public void CallNamesListBoxSelectedIndexChanged()
		{
			base.NamesListBoxSelectedIndexChanged(this, new EventArgs());
		}
		
		/// <summary>
		/// Calls the base class's CustomNameTextBoxTextChanged method.
		/// </summary>
		public void CallCustomNameTextBoxTextChanged()
		{
			base.CustomNameTextBoxTextChanged(this, new EventArgs());
		}
	}
}
