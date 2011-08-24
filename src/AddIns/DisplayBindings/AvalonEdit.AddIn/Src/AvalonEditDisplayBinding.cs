// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

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
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			var fileFilter = ProjectService.GetFileFilters().FirstOrDefault(ff => ff.ContainsExtension(extension));
			
			return fileFilter != null && fileFilter.MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
		{
			return detectedMimeType.StartsWith("text/") ? 0.5 : 0;
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
		
		public bool IsPreferredBindingForFile(string fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(string fileName, Stream fileContent, string detectedMimeType)
		{
			return double.NegativeInfinity;
		}
	}
}
