// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.WpfDesign.Designer.ThumbnailView;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class ThumbnailViewPad : AbstractPadContent
	{
		ContentPresenter contentControl = new ContentPresenter();

		ThumbnailView thumbnailView = new ThumbnailView();

		TextBlock notAvailableTextBlock = new TextBlock {
			Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.OutlinePad.NotAvailable}"),
			TextWrapping = TextWrapping.Wrap
		};
		
		public ThumbnailViewPad()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveViewContentChanged;
			WorkbenchActiveViewContentChanged(null, null);
		}

		void WorkbenchActiveViewContentChanged(object sender, EventArgs e)
		{
			WpfViewContent wpfView = WorkbenchSingleton.Workbench.ActiveViewContent as WpfViewContent;
			if (wpfView != null)
			{
				thumbnailView.DesignSurface = wpfView.DesignSurface;
				contentControl.SetContent(thumbnailView);
			}
			else
			{
				contentControl.SetContent(notAvailableTextBlock);
			}
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad
		/// </summary>
		public override object Control {
			get {
				return contentControl;
			}
		}
	}
}
