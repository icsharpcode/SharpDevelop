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
