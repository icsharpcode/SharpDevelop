// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace RubyBinding.Tests.Utils
{
	/// <summary>
	/// Mock implementation of the IEditable and IViewContent.
	/// </summary>
	public class MockEditableViewContent : MockViewContent, IEditable
	{
		string text = String.Empty;
		
		public MockEditableViewContent()
		{
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}		
	}
}
