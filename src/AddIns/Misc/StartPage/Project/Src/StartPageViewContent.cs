// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.StartPage
{
	public class StartPageViewContent : AbstractViewContent
	{
		StartPageControl content = new StartPageControl();
		
		public override object Control {
			get {
				return content;
			}
		}
		
		public StartPageViewContent()
		{
			SetLocalizedTitle("${res:StartPage.StartPageContentName}");
		}
	}
}
