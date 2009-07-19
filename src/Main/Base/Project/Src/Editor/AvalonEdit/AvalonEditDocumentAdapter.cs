// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Editor.AvalonEdit
{
	/// <summary>
	/// Wraps the AvalonEdit TextDocument to provide the IDocument interface.
	/// </summary>
	public class AvalonEditDocumentAdapter : IDocument
	{
		readonly TextDocument document;
		readonly IServiceProvider parentServiceProvider;
		
		/// <summary>
		/// Creates a new AvalonEditDocumentAdapter instance.
		/// </summary>
		/// <param name="document">The document to wrap.</param>
		/// <param name="parentServiceProvider">The service provider used for GetService calls.</param>
		public AvalonEditDocumentAdapter(TextDocument document, IServiceProvider parentServiceProvider)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			this.parentServiceProvider = parentServiceProvider;
		}
		
		/// <summary>
		/// Used in Unit Tests
		/// </summary>
		public AvalonEditDocumentAdapter()
		{
			this.document = new TextDocument();
		}
		
		/// <summary>
		/// Used in Unit Tests
		/// </summary>
		public AvalonEditDocumentAdapter(IServiceProvider parentServiceProvider)
		{
			this.document = new TextDocument();
			this.parentServiceProvider = parentServiceProvider;
		}
		
		sealed class LineAdapter : IDocumentLine
		{
			readonly DocumentLine line;
			
			public LineAdapter(DocumentLine line)
			{
				Debug.Assert(line != null);
				this.line = line;
			}
			
			public int Offset {
				get { return line.Offset; }
			}
			
			public int Length {
				get { return line.Length; }
			}
			
			public int TotalLength {
				get { return line.TotalLength; }
			}
			
			public int DelimiterLength {
				get { return line.DelimiterLength; }
			}
			
			public int LineNumber {
				get { return line.LineNumber; }
			}
			
			public string Text {
				get { return line.Text; }
			}
		}
		
		public int TextLength {
			get { return document.TextLength; }
		}
		
		public int TotalNumberOfLines {
			get { return document.LineCount; }
		}
		
		public string Text {
			get { return document.Text; }
			set { document.Text = value; }
		}
		
		public event EventHandler TextChanged {
			add { document.TextChanged += value; }
			remove { document.TextChanged -= value; }
		}
		
		public IDocumentLine GetLine(int lineNumber)
		{
			return new LineAdapter(document.GetLineByNumber(lineNumber));
		}
		
		public IDocumentLine GetLineForOffset(int offset)
		{
			return new LineAdapter(document.GetLineByOffset(offset));
		}
		
		public int PositionToOffset(int line, int column)
		{
			return document.GetOffset(new TextLocation(line, column));
		}
		
		public Location OffsetToPosition(int offset)
		{
			return ToLocation(document.GetLocation(offset));
		}
		
		public static Location ToLocation(TextLocation position)
		{
			return new Location(position.Column, position.Line);
		}
		
		public static TextLocation ToPosition(Location location)
		{
			return new TextLocation(location.Line, location.Column);
		}
		
		public void Insert(int offset, string text)
		{
			document.Insert(offset, text);
		}
		
		public void Remove(int offset, int length)
		{
			document.Remove(offset, length);
		}
		
		public void Replace(int offset, int length, string newText)
		{
			document.Replace(offset, length, newText);
		}
		
		public char GetCharAt(int offset)
		{
			return document.GetCharAt(offset);
		}
		
		public string GetText(int offset, int length)
		{
			return document.GetText(offset, length);
		}
		
		public System.IO.TextReader CreateReader()
		{
			return CreateSnapshot().CreateReader();
		}
		
		public ITextBuffer CreateSnapshot()
		{
			return new AvalonEditTextSourceAdapter(document.CreateSnapshot());
		}
		
		public void StartUndoableAction()
		{
			document.BeginUpdate();
		}
		
		public void EndUndoableAction()
		{
			document.EndUpdate();
		}
		
		public IDisposable OpenUndoGroup()
		{
			return document.RunUpdate();
		}
		
		public object GetService(Type serviceType)
		{
			if (serviceType == typeof(TextDocument))
				return document;
			if (parentServiceProvider != null)
				return parentServiceProvider.GetService(serviceType);
			else
				return null;
		}
		
		public ITextAnchor CreateAnchor(int offset)
		{
			return new AnchorAdapter(document.CreateAnchor(offset));
		}
		
		#region AnchorAdapter
		sealed class AnchorAdapter : ITextAnchor
		{
			readonly TextAnchor anchor;
			
			public AnchorAdapter(TextAnchor anchor)
			{
				this.anchor = anchor;
			}
			
			#region Forward Deleted Event
			EventHandler deleted;
			
			public event EventHandler Deleted {
				add {
					// we cannot simply forward the event handler because
					// that would raise the event with an incorrect sender
					if (deleted == null && value != null)
						anchor.Deleted += OnDeleted;
					deleted += value;
				}
				remove {
					deleted -= value;
					if (deleted == null)
						anchor.Deleted -= OnDeleted;
				}
			}
			
			void OnDeleted(object sender, EventArgs e)
			{
				// raise event with correct sender
				if (deleted != null)
					deleted(this, e);
			}
			#endregion
			
			public Location Location {
				get { return ToLocation(anchor.Location); }
			}
			
			public int Offset {
				get { return anchor.Offset; }
			}
			
			public ICSharpCode.SharpDevelop.Editor.AnchorMovementType MovementType {
				get {
					switch (anchor.MovementType) {
						case ICSharpCode.AvalonEdit.Document.AnchorMovementType.AfterInsertion:
							return ICSharpCode.SharpDevelop.Editor.AnchorMovementType.AfterInsertion;
						case ICSharpCode.AvalonEdit.Document.AnchorMovementType.BeforeInsertion:
							return ICSharpCode.SharpDevelop.Editor.AnchorMovementType.BeforeInsertion;
						default:
							throw new NotSupportedException();
					}
				}
				set {
					switch (value) {
						case ICSharpCode.SharpDevelop.Editor.AnchorMovementType.AfterInsertion:
							anchor.MovementType = ICSharpCode.AvalonEdit.Document.AnchorMovementType.AfterInsertion;
							break;
						case ICSharpCode.SharpDevelop.Editor.AnchorMovementType.BeforeInsertion:
							anchor.MovementType = ICSharpCode.AvalonEdit.Document.AnchorMovementType.BeforeInsertion;
							break;
						default:
							throw new NotSupportedException();
					}
				}
			}
			
			public bool SurviveDeletion {
				get { return anchor.SurviveDeletion; }
				set { anchor.SurviveDeletion = value; }
			}
			
			public bool IsDeleted {
				get { return anchor.IsDeleted; }
			}
			
			public int Line {
				get { return anchor.Line; }
			}
			
			public int Column {
				get { return anchor.Column; }
			}
		}
		#endregion
	}
}
