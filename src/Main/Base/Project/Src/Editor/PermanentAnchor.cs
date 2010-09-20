// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.Core;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// A permanent anchor that works even when a file is closed and later reopened.
	/// </summary>
	public sealed class PermanentAnchor : ITextAnchor
	{
		IDocument currentDocument;
		ITextAnchor baseAnchor;
		
		FileName fileName;
		int line, column;
		bool isDeleted;
		bool surviveDeletion;
		AnchorMovementType movementType = AnchorMovementType.BeforeInsertion;
		
		public PermanentAnchor(FileName fileName, int line, int column)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (line < 1)
				throw new ArgumentOutOfRangeException("line");
			if (column < 1)
				throw new ArgumentOutOfRangeException("column");
			
			Gui.WorkbenchSingleton.AssertMainThread();
			
			this.fileName = fileName;
			this.line = line;
			this.column = column;
			PermanentAnchorService.AddAnchor(this);
		}
		
		internal void AttachTo(IDocument document)
		{
			if (isDeleted)
				return;
			Debug.Assert(currentDocument == null && document != null);
			this.currentDocument = document;
			line = Math.Min(line, document.TotalNumberOfLines);
			column = Math.Min(column, document.GetLine(line).Length + 1);
			baseAnchor = document.CreateAnchor(document.PositionToOffset(line, column));
			baseAnchor.MovementType = movementType;
			baseAnchor.SurviveDeletion = surviveDeletion;
			baseAnchor.Deleted += baseAnchor_Deleted;
		}

		internal void Detach()
		{
			if (isDeleted)
				return;
			Debug.Assert(currentDocument != null);
			
			Location loc = baseAnchor.Location;
			line = loc.Line;
			column = loc.Column;
			
			baseAnchor = null;
			currentDocument = null;
		}
		
		internal void SetFileName(FileName newName)
		{
			this.fileName = newName;
		}
		
		internal void FileDeleted()
		{
			Debug.Assert(currentDocument == null);
			if (!surviveDeletion) {
				baseAnchor_Deleted(null, EventArgs.Empty);
			}
		}
		
		void baseAnchor_Deleted(object sender, EventArgs e)
		{
			isDeleted = true;
			baseAnchor = null;
			currentDocument = null;
			
			if (Deleted != null)
				Deleted(this, e);
		}
		
		/// <summary>
		/// Gets the file name of the anchor.
		/// </summary>
		public FileName FileName {
			get { return fileName; }
		}
		
		/// <inheritdoc/>
		public event EventHandler Deleted;
		
		/// <inheritdoc/>
		public Location Location {
			get {
				if (isDeleted)
					throw new InvalidOperationException();
				if (baseAnchor != null)
					return baseAnchor.Location;
				else
					return new Location(column, line);
			}
		}
		
		/// <summary>
		/// Gets the editor to which this anchor currently belongs; or null if the file is not opened in any text editor.
		/// </summary>
		public IDocument CurrentDocument {
			get { return currentDocument; }
		}
		
		/// <summary>
		/// Gets the offset.
		/// Warning: this method is only available while the document anchor is attached to a text editor, otherwise
		/// it will throw an InvalidOperationException.
		/// </summary>
		public int Offset {
			get {
				if (baseAnchor != null)
					return baseAnchor.Offset;
				else
					throw new InvalidOperationException();
			}
		}
		
		/// <inheritdoc/>
		public AnchorMovementType MovementType {
			get { return movementType; }
			set {
				movementType = value;
				if (baseAnchor != null)
					baseAnchor.MovementType = value;
			}
		}
		
		/// <inheritdoc/>
		public bool SurviveDeletion {
			get { return surviveDeletion; }
			set {
				surviveDeletion = value;
				if (baseAnchor != null)
					baseAnchor.SurviveDeletion = value;
			}
		}
		
		/// <inheritdoc/>
		public bool IsDeleted {
			get { return isDeleted; }
		}
		
		/// <inheritdoc/>
		public int Line {
			get {
				if (baseAnchor != null)
					return baseAnchor.Line;
				else
					return line;
			}
		}
		
		/// <inheritdoc/>
		public int Column {
			get {
				if (baseAnchor != null)
					return baseAnchor.Column;
				else
					return column;
			}
		}
	}
	
	public static class PermanentAnchorService
	{
		static WeakCollection<PermanentAnchor> permanentAnchors = new WeakCollection<PermanentAnchor>();
		static Dictionary<FileName, IDocument> openDocuments = new Dictionary<FileName, IDocument>();
		
		internal static void AddAnchor(PermanentAnchor anchor)
		{
			permanentAnchors.Add(anchor);
			
			IDocument doc;
			if (openDocuments.TryGetValue(anchor.FileName, out doc))
				anchor.AttachTo(doc);
		}
		
		/// <summary>
		/// Renames anchors without document.
		/// </summary>
		internal static void FileRenamed(FileRenameEventArgs e)
		{
			FileName sourceFile = new FileName(e.SourceFile);
			FileName targetFile = new FileName(e.TargetFile);
			
			foreach (PermanentAnchor anchor in permanentAnchors) {
				if (anchor.CurrentDocument == null) {
					if (e.IsDirectory) {
						if (FileUtility.IsBaseDirectory(e.SourceFile, anchor.FileName)) {
							anchor.SetFileName(new FileName(FileUtility.RenameBaseDirectory(anchor.FileName, e.SourceFile, e.TargetFile)));
						}
					} else {
						if (anchor.FileName == sourceFile)
							anchor.SetFileName(targetFile);
					}
				}
			}
		}
		
		/// <summary>
		/// Deletes anchors without document.
		/// </summary>
		internal static void FileDeleted(FileEventArgs e)
		{
			FileName fileName = new FileName(e.FileName);
			foreach (PermanentAnchor anchor in permanentAnchors) {
				if (anchor.CurrentDocument == null) {
					if (e.IsDirectory) {
						if (FileUtility.IsBaseDirectory(fileName, anchor.FileName))
							anchor.FileDeleted();
					} else {
						if (fileName == anchor.FileName)
							anchor.FileDeleted();
					}
				}
			}
		}
		
		/// <summary>
		/// Tells detached permanent anchors to attach to the specified text editor.
		/// </summary>
		public static void AttachDocument(FileName fileName, IDocument document)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (document == null)
				throw new ArgumentNullException("document");
			Gui.WorkbenchSingleton.AssertMainThread();
			
			// there may be multiple documents with the same file name - in that case, only attach to one of them
			if (!openDocuments.ContainsKey(fileName)) {
				openDocuments.Add(fileName, document);
				foreach (PermanentAnchor anchor in permanentAnchors) {
					if (anchor.CurrentDocument == null && anchor.FileName == fileName) {
						anchor.AttachTo(document);
					}
				}
			}
		}
		
		/// <summary>
		/// Tells attached permanent anchors to detach from the specified text editor.
		/// </summary>
		public static void DetachDocument(FileName fileName, IDocument document)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");
			if (document == null)
				throw new ArgumentNullException("document");
			Gui.WorkbenchSingleton.AssertMainThread();
			
			IDocument actualDocument;
			if (openDocuments.TryGetValue(fileName, out actualDocument)) {
				// test whether we're detaching the correct document
				if (document == actualDocument) {
					openDocuments.Remove(fileName);
					foreach (PermanentAnchor anchor in permanentAnchors) {
						if (anchor.CurrentDocument == document) {
							anchor.Detach();
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Informs the PermanentAnchorService when the file name of a document has changed.
		/// </summary>
		public static void RenameDocument(FileName oldFileName, FileName newFileName, IDocument document)
		{
			if (oldFileName == null)
				throw new ArgumentNullException("oldFileName");
			if (newFileName == null)
				throw new ArgumentNullException("newFileName");
			if (document == null)
				throw new ArgumentNullException("document");
			Gui.WorkbenchSingleton.AssertMainThread();
			
			IDocument actualDocument;
			if (openDocuments.TryGetValue(oldFileName, out actualDocument)) {
				// test whether we're detaching the correct document
				if (document == actualDocument) {
					if (openDocuments.ContainsKey(newFileName)) {
						// new file name already taken? just detach the old stuff
						DetachDocument(oldFileName, document);
					} else {
						openDocuments.Remove(oldFileName);
						openDocuments.Add(newFileName, document);
						foreach (PermanentAnchor anchor in permanentAnchors) {
							if (anchor.CurrentDocument == document) {
								anchor.SetFileName(newFileName);
							}
						}
					}
				}
			}
		}
	}
}
