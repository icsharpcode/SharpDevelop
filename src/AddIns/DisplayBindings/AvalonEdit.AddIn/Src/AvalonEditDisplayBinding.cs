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
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.AvalonEdit.AddIn
{
	public class AvalonEditDisplayBinding : IDisplayBinding
	{
		static bool addInHighlightingDefinitionsRegistered;
		
		internal static void RegisterAddInHighlightingDefinitions()
		{
			SD.MainThread.VerifyAccess();
			if (!addInHighlightingDefinitionsRegistered) {
				foreach (AddInTreeSyntaxMode syntaxMode in AddInTree.BuildItems<AddInTreeSyntaxMode>(SyntaxModeDoozer.Path, null, false)) {
					syntaxMode.Register(HighlightingManager.Instance);
				}
				addInHighlightingDefinitionsRegistered = true;
			}
		}
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new AvalonEditViewContent(file);
		}
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			string extension = Path.GetExtension(fileName);
			var fileFilter = ProjectService.GetFileFilters().FirstOrDefault(ff => ff.ContainsExtension(extension));
			
			return fileFilter != null && fileFilter.MimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase);
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return detectedMimeType.StartsWith("text/", StringComparison.Ordinal) ? 0.5 : 0;
		}
	}
	
	public class ChooseEncodingDisplayBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			ChooseEncodingDialog dlg = new ChooseEncodingDialog();
			dlg.Owner = SD.Workbench.MainWindow;
			using (Stream stream = file.OpenRead()) {
				using (StreamReader reader = FileReader.OpenStream(stream, SD.FileService.DefaultFileEncoding)) {
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
		
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return false;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return double.NegativeInfinity;
		}
	}
}
