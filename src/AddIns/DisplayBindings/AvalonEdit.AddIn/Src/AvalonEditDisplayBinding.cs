// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditDisplayBinding : IDisplayBinding
	{
		static bool addInHighlightingDefinitionsRegistered;
		
		internal static void RegisterAddInHighlightingDefinitions()
		{
			WorkbenchSingleton.AssertMainThread();
			if (!addInHighlightingDefinitionsRegistered) {
				foreach (AddInTreeSyntaxMode syntaxMode in AddInTree.BuildItems<AddInTreeSyntaxMode>(SyntaxModeDoozer.Path, null, false)) {
					syntaxMode.Register(HighlightingManager.Instance);
				}
				addInHighlightingDefinitionsRegistered = true;
			}
		}
		
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new AvalonEditViewContent(file);
		}
	}
	
	public class ChooseEncodingDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			ChooseEncodingDialog dlg = new ChooseEncodingDialog();
			dlg.Owner = WorkbenchSingleton.MainWindow;
			using (Stream stream = file.OpenRead()) {
				using (StreamReader reader = FileReader.OpenStream(stream, FileService.DefaultFileEncoding.GetEncoding())) {
					reader.Peek(); // force reader to auto-detect encoding
					dlg.Encoding = reader.CurrentEncoding;
				}
			}
			if (dlg.ShowDialog() == true) {
				return new AvalonEditViewContent(file, dlg.Encoding);
			} else {
				return null;
			}
		}
	}
}
