// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Editor;
using System;
using System.Windows.Controls;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Implement this interface to make a view content display tools in the outline pad.
	/// </summary>
	public interface IOutlineContentHost
	{
		/// <summary>
		/// Gets the control to display in the outline pad.
		/// </summary>
		object OutlineContent { get; }
	}
	
	/// <summary>
	/// A pad that shows a single child control determined by the document that currently has the focus.
	/// </summary>
	public class OutlinePad : AbstractPadContent
	{
		ContentPresenter contentControl = new ContentPresenter();
		
		public override object Control {
			get {
				return contentControl;
			}
		}
		
		public OutlinePad()
		{
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += WorkbenchActiveContentChanged;
			WorkbenchActiveContentChanged(null, null);
		}
		
		void WorkbenchActiveContentChanged(object sender, EventArgs e)
		{
			IViewContent view = WorkbenchSingleton.Workbench.ActiveViewContent;
			if(view!=null){
				IOutlineContentHost content = view.GetService(typeof(IOutlineContentHost)) as IOutlineContentHost;
				if(content!=null){
					contentControl.SetContent(content.OutlineContent, content);
					return ;
				}
			}
			
			ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
			if (provider != null) {
				IOutlineContentHost content = provider.TextEditor.GetService(typeof(IOutlineContentHost)) as IOutlineContentHost;
				if (content != null) {
					contentControl.SetContent(content.OutlineContent, content);
					return;
				}
			}
			contentControl.SetContent(StringParser.Parse("${res:MainWindow.Windows.OutlinePad.NoContentAvailable}"));
		}
	}
}
