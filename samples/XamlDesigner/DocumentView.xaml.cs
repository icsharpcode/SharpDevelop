using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ICSharpCode.WpfDesign.Designer.Services;
using System.Windows.Threading;
using ICSharpCode.Xaml;
using ICSharpCode.WpfDesign.Designer;
using ICSharpCode.TextEditor;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.WpfDesign;
using ICSharpCode.TextEditor.Undo;

namespace ICSharpCode.XamlDesigner
{
	public partial class DocumentView
	{
		public DocumentView(Document doc)
		{
			InitializeComponent();

			ShellDocument = doc;
			Shell.Instance.Views[doc] = this;			

			uxTextEditor.SetHighlighting("XML");
			uxTextEditor.DataBindings.Add("Text", doc, "Text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
			ShellDocument.Context.AddService(typeof(ITextContainer), uxTextEditor);

			uxTextEditor.Document.UndoStack.OperationPushed += UndoStack_OperationPushed;
			uxTextEditor.Document.DocumentChanged += Document_DocumentChanged;
			uxTextEditor.Document.DocumentAboutToBeChanged += Document_DocumentAboutToBeChanged;
		}

		public Document ShellDocument { get; private set; }

		IUndoableOperation lastOperation;
		bool textValid;

		void Document_DocumentAboutToBeChanged(object sender, DocumentEventArgs e)
		{
			textValid = false;
		}

		void Document_DocumentChanged(object sender, DocumentEventArgs e)
		{
			textValid = true;
			TryUpdateDesignUndoStack();
		}

		void UndoStack_OperationPushed(object sender, OperationEventArgs e)
		{
			lastOperation = e.Operation;
			TryUpdateDesignUndoStack();
		}

		void TryUpdateDesignUndoStack()
		{
			if (textValid && lastOperation != null) {
				ShellDocument.Context.UndoService.Done(new TextAction(lastOperation));
				lastOperation = null;
			}
		}

		public DesignSurface DesignSurface
		{
			get
			{
				return uxDesignSurface;
			}
		}

		public void JumpToError(XamlDocumentError error)
		{
			ShellDocument.Mode = DocumentMode.Xaml;
			Dispatcher.BeginInvoke(new Action(delegate {
				uxTextEditor.ActiveTextAreaControl.JumpTo(error.LineNumber - 1, error.LinePosition - 1);
			}), DispatcherPriority.Background);
		}
	}

	class TextEditorWithoutUndo : TextEditorControl, ITextContainer
	{
		public TextEditorWithoutUndo()
		{
			editactions.Remove(Keys.Control | Keys.Z);
			editactions.Remove(Keys.Control | Keys.Y);
		}

		public override void EndUpdate()
		{
			base.EndUpdate();
		}
	}

	class TextAction : ITextAction
	{
		public TextAction(IUndoableOperation op)
		{
			this.op = op;
		}

		IUndoableOperation op;

		public IEnumerable<DesignItem> AffectedItems
		{
			get { yield break; }
		}

		public string Title
		{
			get { return "Text Editing"; }
		}

		public void Do()
		{
			op.Redo();
		}

		public void Undo()
		{
			op.Undo();
		}
	}
}
