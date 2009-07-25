// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui;
using System;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.XmlEditor;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlLanguageBinding.
	/// </summary>
	public class XamlLanguageBinding : XmlLanguageBinding
	{
		XamlColorizer colorizer;
		TextView textView;

		public override void Attach(ITextEditor editor)
		{
			base.Attach(editor);
			
			// try to access the ICSharpCode.AvalonEdit.Rendering.TextView
			// of this ITextEditor
			this.textView = editor.GetService(typeof(TextView)) as TextView;
			
			// if editor is not an AvalonEdit.TextEditor
			// GetService returns null
			if (textView != null) {
				colorizer = new XamlColorizer(editor, textView);
				// attach the colorizer
				textView.LineTransformers.Add(colorizer);
				// add the XamlOutlineContentHost, which manages the tree view
				textView.Services.AddService(typeof(IOutlineContentHost), new XamlOutlineContentHost(editor));
			}
		}

		public override void Detach()
		{
			base.Detach();
			
			// if we added something before
			if (textView != null && colorizer != null) {
				// remove and dispose everything we added
				textView.LineTransformers.Remove(colorizer);
				textView.Services.RemoveService(typeof(IOutlineContentHost));
				colorizer.Dispose();
			}
		}
	}
	
	class XamlOutlineContentHost : IOutlineContentHost
	{
		ITextEditor editor;
		
		public XamlOutlineContentHost(ITextEditor editor)
		{
			this.editor = editor;
		}
		
		public object OutlineContent {
			get {
				return "Hello from XAML " + editor.FileName;
			}
		}
	}
}
