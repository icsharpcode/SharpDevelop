
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System;

namespace AlternateEditor
{
	public class AlternateEditorDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public bool CanCreateContentForLanguage(string language)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(string fileName)
		{
			Editor editor = new Editor();
			editor.Load(fileName);
			return editor;
		}
		
		public IViewContent CreateContentForLanguage(string language, string content)
		{
			return new Editor();
		}
	}
}
