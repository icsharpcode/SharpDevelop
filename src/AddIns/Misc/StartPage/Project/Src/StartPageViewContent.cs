// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.StartPage
{
	public class StartPageViewContent : AbstractViewContent
	{
		StartPageControl content = new StartPageControl();
		
		public override object Content {
			get {
				return content;
			}
		}
		
		public StartPageViewContent()
		{
			this.TitleName = StringParser.Parse("${res:StartPage.StartPageContentName}");
		}
	}
}
