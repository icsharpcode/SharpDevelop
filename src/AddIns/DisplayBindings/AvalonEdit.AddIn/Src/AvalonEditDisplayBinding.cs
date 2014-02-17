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
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Editor;
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
		
		static Encoding DetectExistingEncoding(OpenedFile file)
		{
			var existingTextModel = file.GetModel(FileModels.TextDocument, GetModelOptions.DoNotLoad);
			if (existingTextModel != null)
				return existingTextModel.GetFileModelInfo().Encoding;
			using (Stream stream = file.GetModel(FileModels.Binary).OpenRead()) {
				using (StreamReader reader = FileReader.OpenStream(stream, SD.FileService.DefaultFileEncoding)) {
					reader.Peek();
					// force reader to auto-detect encoding
					return reader.CurrentEncoding;
				}
			}
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			ChooseEncodingDialog dlg = new ChooseEncodingDialog();
			dlg.Owner = SD.Workbench.MainWindow;
			dlg.Encoding = DetectExistingEncoding(file);
			if (dlg.ShowDialog() == true) {
				LoadWithEncoding(file, dlg.Encoding);
				return new AvalonEditViewContent(file);
			} else {
				return null;
			}
		}
		
		static void LoadWithEncoding(OpenedFile file, Encoding encoding)
		{
			var doc = file.GetModel(FileModels.TextDocument, GetModelOptions.DoNotLoad | GetModelOptions.AllowStale);
			if (doc == null)
				doc = new TextDocument();
			doc.GetFileModelInfo().Encoding = encoding;
			string newText;
			using (Stream stream = file.GetModel(FileModels.Binary).OpenRead()) {
				using (StreamReader reader = new StreamReader(stream, encoding, detectEncodingFromByteOrderMarks: false)) {
					newText = reader.ReadToEnd();
				}
			}
			FileModels.TextDocument.ReloadDocument(doc, new StringTextSource(newText));
			file.ReplaceModel(FileModels.TextDocument, doc, ReplaceModelMode.SetAsValid);
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
						