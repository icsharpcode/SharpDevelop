// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class OutlineViewPad : AbstractPadContent
	{
		ContentControl host = new ContentControl();
		TextBlock notAvailableTextBlock = new TextBlock {
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.OutlinePad.NotAvailable}"),
			TextWrapping = TextWrapping.Wrap
		};
		
		public OutlineViewPad()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
			WorkbenchActiveViewContentChanged(null, null);
		}

		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			WpfViewContent wpfView = WorkbenchSingleton.Workbench.ActiveViewContent as WpfViewContent;
			if (wpfView != null) {
				host.Content = wpfView.Outline;
			} else {
				host.Content = notAvailableTextBlock;
			}
		}
		
		public override object Content {
			get {
				return host;
			}
		}
	}
}
