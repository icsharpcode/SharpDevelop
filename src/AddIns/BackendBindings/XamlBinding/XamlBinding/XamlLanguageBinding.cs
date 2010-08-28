// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Xml;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlLanguageBinding.
	/// </summary>
	public class XamlLanguageBinding : XmlEditor.XmlLanguageBinding
	{
		XamlColorizer colorizer;
		TextView textView;
		XamlOutlineContentHost contentHost;

		public override void Attach(ITextEditor editor)
		{
			base.Attach(editor);
			
			// try to access the ICSharpCode.AvalonEdit.Rendering.TextView
			// of this ITextEditor
			this.textView = editor.GetService(typeof(TextView)) as TextView;
			
			// if editor is not an AvalonEdit.TextEditor
			// GetService returns null
			if (textView != null) {
				if (WorkbenchSingleton.Workbench != null) {
					if (XamlBindingOptions.UseAdvancedHighlighting) {
						colorizer = new XamlColorizer(editor, textView);
						// attach the colorizer
						textView.LineTransformers.Add(colorizer);
					}
					// add the XamlOutlineContentHost, which manages the tree view
					contentHost = new XamlOutlineContentHost(editor);
					textView.Services.AddService(typeof(IOutlineContentHost), contentHost);
				}
				// add ILanguageBinding
				textView.Services.AddService(typeof(XamlLanguageBinding), this);
			}
		}

		public override void Detach()
		{
			base.Detach();
			
			// if we added something before
			if (textView != null) {
				// remove and dispose everything we added
				if (colorizer != null) {
					textView.LineTransformers.Remove(colorizer);
					colorizer.Dispose();
				}
				if (contentHost != null) {
					textView.Services.RemoveService(typeof(IOutlineContentHost));
					contentHost.Dispose();
				}
				textView.Services.RemoveService(typeof(XamlLanguageBinding));
			}
		}
	}
}
