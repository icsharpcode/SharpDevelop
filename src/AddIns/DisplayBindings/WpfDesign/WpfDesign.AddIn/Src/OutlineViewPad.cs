// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.WpfDesign.AddIn
{
	/*
	public class OutlineViewPad : AbstractPadContent
	{
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
            host.ViewContent = wpfView;
			if (wpfView != null) {
				host.Child = wpfView.Outline;
			} else {
				host.Child = notAvailableTextBlock;
			}
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override System.Windows.Forms.Control Control {
			get {
				return host;
			}
		}
		
		/// <summary>
		/// Cleans up all used resources
		/// </summary>
		public override void Dispose()
		{
			host.Dispose();
			base.Dispose();
		}
	}
	*/
}
