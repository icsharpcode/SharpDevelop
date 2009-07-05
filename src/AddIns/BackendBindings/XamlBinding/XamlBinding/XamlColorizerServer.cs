// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.XmlEditor;
using System;
using System.IO;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlColorizerServer.
	/// </summary>
	public static class XamlColorizerServer
	{
		const bool enableHighlighting = false;
		
		public static void InitializeServer()
		{
			WorkbenchSingleton.Workbench.ViewOpened += new ViewContentEventHandler(WorkbenchSingleton_Workbench_ViewOpened);
		}

		static void WorkbenchSingleton_Workbench_ViewOpened(object sender, ViewContentEventArgs e)
		{
			if (e.Content.PrimaryFileName == null)
				return;
			if (!Path.GetExtension(e.Content.PrimaryFileName).Equals(".xaml", StringComparison.OrdinalIgnoreCase))
				return;
			ITextEditorProvider textEditor = e.Content as ITextEditorProvider;
			if (textEditor != null) {
				TextView textView = textEditor.TextEditor.GetService(typeof(TextView)) as TextView;
				
				if (textView != null && enableHighlighting)
					textView.LineTransformers.Add(new XamlColorizer(e.Content, textView));
			}
		}
	}
}