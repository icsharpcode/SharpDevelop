// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;

namespace EmbeddedImageAddIn
{
	// SharpDevelop creates one instance of EmbeddedImageLanguageBinding for each text editor.
	public class EmbeddedImageLanguageBinding : DefaultLanguageBinding
	{
		TextView textView;
		ImageElementGenerator g;
		
		public override void Attach(ITextEditor editor)
		{
			base.Attach(editor);
			// ITextEditor is SharpDevelop's abstraction of the text editor.
			// We use GetService() to get the underlying AvalonEdit instance.
			textView = editor.GetService(typeof(TextView)) as TextView;
			if (textView != null) {
				g = new ImageElementGenerator(Path.GetDirectoryName(editor.FileName));
				textView.ElementGenerators.Add(g);
			}
		}
		
		public override void Detach()
		{
			if (textView != null) {
				textView.ElementGenerators.Remove(g);
			}
			base.Detach();
		}
	}
}
