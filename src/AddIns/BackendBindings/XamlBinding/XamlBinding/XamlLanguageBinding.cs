// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding
{
	/// <summary>
	/// Description of XamlTextEditorExtension.
	/// </summary>
	public class XamlTextEditorExtension : XmlEditor.XmlTextEditorExtension
	{
//		XamlColorizer colorizer;
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
				if (SD.Workbench != null) {
//					if (XamlBindingOptions.UseAdvancedHighlighting) {
//						colorizer = new XamlColorizer(editor, textView);
//						// attach the colorizer
//						textView.LineTransformers.Add(colorizer);
//					}
					// add the XamlOutlineContentHost, which manages the tree view
					contentHost = new XamlOutlineContentHost(editor);
					textView.Services.AddService(typeof(IOutlineContentHost), contentHost);
				}
				// add ILanguageBinding
				textView.Services.AddService(typeof(XamlTextEditorExtension), this);
			}
			
			SD.ParserService.ParseInformationUpdated += ParseInformationUpdated;
		}

		public override void Detach()
		{
			base.Detach();
			
			// if we added something before
			if (textView != null) {
				// remove and dispose everything we added
//				if (colorizer != null) {
//					textView.LineTransformers.Remove(colorizer);
//					colorizer.Dispose();
//				}
				if (contentHost != null) {
					textView.Services.RemoveService(typeof(IOutlineContentHost));
					contentHost.Dispose();
				}
				textView.Services.RemoveService(typeof(XamlTextEditorExtension));
			}
			
			SD.ParserService.ParseInformationUpdated -= ParseInformationUpdated;
		}

		void ParseInformationUpdated(object sender, ICSharpCode.SharpDevelop.Parser.ParseInformationEventArgs e)
		{
			if (!e.FileName.Equals(textView.Document.FileName)) return;
			ITextMarkerService markerService = textView.GetService<ITextMarkerService>();
			if (markerService == null) return;
			markerService.RemoveAll(m => m.Tag is Error);
			foreach (Error error in e.NewUnresolvedFile.Errors) {
				var offset = textView.Document.GetOffset(error.Region.Begin);
				var endOffset = textView.Document.GetOffset(error.Region.End);
				int length = endOffset - offset;
				
				if (length < 2) {
					// marker should be at least 2 characters long, but take care that we don't make
					// it longer than the document
					length = Math.Min(2, textView.Document.TextLength - offset);
				}
				var marker = markerService.Create(offset, length);
				switch (error.ErrorType) {
					case ErrorType.Unknown:
						marker.MarkerColor = Colors.Blue;
						break;
					case ErrorType.Error:
						marker.MarkerColor = Colors.Red;
						break;
					case ErrorType.Warning:
						marker.MarkerColor = Colors.Orange;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
				marker.ToolTip = error.Message;
				marker.Tag = error;
			}
		}
	}
	
	public class XamlLanguageBinding : XmlEditor.XmlLanguageBinding
	{
		
	}
}
