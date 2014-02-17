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
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop.Widgets.MyersDiff;

namespace ICSharpCode.SharpDevelop.Workbench
{
	public sealed class TextDocumentFileModelProvider : IFileModelProvider<TextDocument>
	{
		/// <summary>
		/// Internal ctor: FileModels.TextDocument should be the only instance.
		/// </summary>
		internal TextDocumentFileModelProvider()
		{
		}
		
		// use explicit interface implementations because these methods are supposed to only
		// be called from the OpenedFile infrastructure
		TextDocument IFileModelProvider<TextDocument>.Load(OpenedFile file)
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
				ReloadDocument(document, new StringTextSource(textContent));
				info.IsStale = false;
			} else {
				document = new TextDocument(textContent);
				// Store info for the new document (necessary so that we don't forget the encoding we just used)
				document.GetRequiredService<IServiceContainer>().AddService(typeof(DocumentFileModelInfo), info);
			}
			return document;
		}
		
		public void ReloadDocument(TextDocument document, ITextSource newContent)
		{
			var info = document.GetFileModelInfo();
			var diff = new MyersDiffAlgorithm(new StringSequence(document.Text), new StringSequence(newContent.Text));
			if (info != null)
				info.isLoading = true;
			try {
				document.Replace(0, document.TextLength, newContent, ToOffsetChangeMap(diff.GetEdits()));
				document.UndoStack.ClearAll();
			} finally {
				if (info != null)
					info.isLoading = false;
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
		
		void IFileModelProvider<TextDocument>.Save(OpenedFile file, TextDocument model, FileSaveOptions options)
		{
			MemoryStream ms = new MemoryStream();
			SaveTo(ms, model, options);
			file.ReplaceModel(FileModels.Binary, new BinaryFileModel(ms.ToArray()), ReplaceModelMode.TransferDirty);
		}
		
		void IFileModelProvider<TextDocument>.SaveCopyAs(OpenedFile file, TextDocument model, FileName outputFileName, FileSaveOptions options)
		{
			using (Stream s = SD.FileSystem.OpenWrite(outputFileName)) {
				SaveTo(s, model, options);
			}
		}
		
		static void SaveTo(Stream stream, TextDocument model, FileSaveOptions options)
		{
			var info = model.GetFileModelInfo();
			if (!CanSaveWithEncoding(model, info.Encoding)) {
				if (ConfirmSaveWithDataLoss(info.Encoding, options)) {
					// continue saving with data loss
					MemoryStream ms = new MemoryStream();
					using (StreamWriter w = new StreamWriter(ms, info.Encoding)) {
						model.WriteTextTo(w);
					}
					ms.Position = 0;
					ms.WriteTo(stream);
					ms.Position = 0;
					// Read back the version we just saved to show the data loss to the user (he'll be able to press Undo).
					using (StreamReader reader = new StreamReader(ms, info.Encoding, false)) {
						model.Text = reader.ReadToEnd();
					}
					return;
				} else {
					info.Encoding = new UTF8Encoding(false);
				}
			}
			using (StreamWriter w = new StreamWriter(stream, info.Encoding)) {
				model.WriteTextTo(w);
			}
		}
		
		/// <summary>
		/// Gets if the document can be saved with the current encoding without losing data.
		/// </summary>
		static bool CanSaveWithEncoding(IDocument document, Encoding encoding)
		{
			if (encoding == null || FileReader.IsUnicode(encoding))
				return true;
			// not a unicode codepage
			string text = document.Text;
			return encoding.GetString(encoding.GetBytes(text)) == text;
		}
		
		static bool ConfirmSaveWithDataLoss(Encoding encoding, FileSaveOptions options)
		{
			if ((options & FileSaveOptions.AllowUserInteraction) == 0)
				return false; // default to UTF-8 if we can't ask the user
			int r = MessageService.ShowCustomDialog(
				"${res:Dialog.Options.IDEOptions.TextEditor.General.FontGroupBox.FileEncodingGroupBox}",
				StringParser.Parse("${res:AvalonEdit.FileEncoding.EncodingCausesDataLoss}",
					new StringTagPair("encoding", encoding.EncodingName)),
				0, -1,
				"${res:AvalonEdit.FileEncoding.EncodingCausesDataLoss.UseUTF8}",
				"${res:AvalonEdit.FileEncoding.EncodingCausesDataLoss.Continue}");
			return r == 1;
		}
		
		bool IFileModelProvider<TextDocument>.CanLoadFrom<U>(IFileModelProvider<U> otherProvider)
		{
			return otherProvider == FileModels.Binary || FileModels.Binary.CanLoadFrom(otherProvider);
		}
		
		void IFileModelProvider<TextDocument>.NotifyRename(OpenedFile file, TextDocument model, FileName oldName, FileName newName)
		{
			model.FileName = newName;
		}
		
		void IFileModelProvider<TextDocument>.NotifyStale(OpenedFile file, TextDocument model)
		{
			model.GetFileModelInfo().IsStale = true;
		}
		
		void IFileModelProvider<TextDocument>.NotifyLoaded(OpenedFile file, TextDocument model)
		{
			model.GetFileModelInfo().NotifyLoaded(file, model);
		}
		
		void IFileModelProvider<TextDocument>.NotifyUnloaded(OpenedFile file, TextDocument model)
		{
			model.GetFileModelInfo().NotifyUnloaded();
		}
	}
}
