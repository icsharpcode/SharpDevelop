// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Forms;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Dialogs
{
	
	public class DummyEditorDialog:IStringBasedEditorDialog
	{
		string textValue;
		
		public DummyEditorDialog(string textValue)
		{
			this.textValue = textValue;
		}
		
		
		public string TextValue {
			get {
				return this.textValue;
			}
		}
		
		public System.Windows.Forms.DialogResult ShowDialog()
		{
			return DialogResult.OK;
		}
		
	}
}
