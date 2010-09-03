// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using ICSharpCode.SharpDevelop.Gui;

namespace MSHelpSystem.Controls
{
	public class Help3TocPad : AbstractPadContent
	{
		public Help3TocPad()
		{
		}
		
		TocPadControl toc = new TocPadControl();
		
		public override object Control
		{
			get { return toc; }
		}		
	}

	public class Help3SearchPad : AbstractPadContent
	{
		public Help3SearchPad()
		{
		}

		SearchPadControl search = new SearchPadControl();

		public override object Control
		{
			get { return search; }
		}
	}
}
