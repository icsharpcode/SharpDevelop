// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Text;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Utils;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.SharpDevelop.Workbench
{
	sealed class TextDocumentFileModelProvider : IFileModelProvider<TextDocument>
	{
		public TextDocument Load(OpenedFile file)
		{
			TextDocument document = file.GetModel(this, GetModelOptions.AllowStale | GetModelOptions.DoNotLoad);
			var info = document != null ? document.GetFileModelInfo() : new DocumentFileModelInfo();
			string textContent;
			using (Stream stream = file.GetModel(FileModels.Binary).OpenRead()) {
				using (StreamReader reader = FileReader.OpenStream(stream, SD.FileService.DefaultFileEncoding)) {
					textContent = reader.ReadToEnd();
					info.Encoding = reader.CurrentEncoding;
				}
			}
			if (document != null) {
				// Reload document
				var diff = new MyersDiffAlgorithm(new StringSequence(document.Text), new StringSequence(textContent));
				document.Replace(0, document.TextLength, textContent, ToOffsetChangeMap(diff.GetEdits()));
				document.UndoStack.ClearAll();
				info.IsStale = false;
				return document;
			} else {
				document = new TextDocument(textContent);
				document.GetRequiredService<IServiceContainer>().AddService(typeof(DocumentFileModelInfo), info);
			}
		}
		
		OffsetChangeMap ToOffsetChangeMap(IEnumerable<Edit> edits)
		{
			var map = new OffsetChangeMap();
			int diff = 0;
			foreach (var edit in edits) {
				Debug.Assert(edit.EditType != ChangeType.None && edit.EditType != ChangeType.Unsaved);
				int offset = edit.BeginA + diff;
				int removalLength = edit.EndA - edit.BeginA;
				int insertionLength = edit.EndB - edit.BeginB;
				
				diff += (insertionLength - removalLength);
				map.Add(new OffsetChangeMapEntry(offset, removalLength, insertionLength));
			}
			return map;
		}
		
		public void Save(OpenedFile file, TextDocument model)
		{
			throw new NotImplementedException();
		}
		
		public void SaveCopyAs(OpenedFile file, TextDocument model, FileName outputFileName)
		{
			throw new NotImplementedException();
		}
		
		bool IFileModelProvider<TextDocument>.CanLoadFrom<U>(IFileModelProvider<U> otherProvider)
		{
			return otherProvider == FileModels.Binary || FileModels.Binary.CanLoadFrom(otherProvider);
		}
		
		public void NotifyRename(OpenedFile file, TextDocument model, FileName oldName, FileName newName)
		{
			model.FileName = newName;
		}
		
		public void NotifyStale(OpenedFile file, TextDocument model)
		{
			model.GetFileModelInfo().IsStale = true;
		}
		
		public void NotifyUnloaded(OpenedFile file, TextDocument model)
		{
			model.GetFileModelInfo().IsStale = true;
		}
	}
}
