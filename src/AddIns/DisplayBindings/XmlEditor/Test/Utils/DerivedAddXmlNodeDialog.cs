// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
