// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Text;
using ICSharpCode.AvalonEdit.Document;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Extra information about a TextDocument that was loaded from an OpenedFile.
	/// </summary>
	public class DocumentFileModelInfo
	{
		OpenedFile file;
		TextDocument document;
		
		public DocumentFileModelInfo()
		{
			this.Encoding = SD.FileService.DefaultFileEncoding;
		}
		
		/// <summary>
		/// Gets whether the model is (re)loading.
		/// </summary>
		internal bool isLoading;
		
		/// <summary>
		/// Gets whether the model is stale.
		/// </summary>
		public bool IsStale { get; internal set; }
		
		/// <summary>
		/// Gets the encoding of the model.
		/// </summary>
		public Encoding Encoding { get; set; }
		
		internal void NotifyLoaded(OpenedFile file, TextDocument document)
		{
			if (this.file != null)
				throw new InvalidOperationException("Duplicate NotifyLoaded() call");
			this.file = file;
			this.document = document;
			document.TextChanged += OnDocumentTextChanged;
			file.IsDirtyChanged += OnFileIsDirtyChanged;
			UpdateUndoStackIsOriginalFile();
		}
		
		internal void NotifyUnloaded()
		{
			document.TextChanged -= OnDocumentTextChanged;
			file.IsDirtyChanged -= OnFileIsDirtyChanged;
			this.IsStale = true;
			this.file = null;
			this.document = null;
		}
		
		void OnDocumentTextChanged(object sender, EventArgs e)
		{
			if (!isLoading) {
				// Set dirty flag on OpenedFile according to UndoStack.IsOriginalFile
				if (document.UndoStack.IsOriginalFile && file.IsDirty) {
					// reset dirty marker
					file.ReplaceModel(FileModels.TextDocument, document, ReplaceModelMode.SetAsValid);
				} else if (!document.UndoStack.IsOriginalFile) {
					file.MakeDirty(FileModels.TextDocument);
				}
			}
		}

		void OnFileIsDirtyChanged(object sender, EventArgs e)
		{
			UpdateUndoStackIsOriginalFile();
		}
		
		void UpdateUndoStackIsOriginalFile()
		{
			if (file.IsDirty) {
				document.UndoStack.DiscardOriginalFileMarker();
			} else {
				document.UndoStack.MarkAsOriginalFile();
			}
		}
	}
}
