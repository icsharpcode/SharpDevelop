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
using System.Windows.Threading;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Undo;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace SharpDevelop.Samples.XamlDesigner
{
	public partial class ShellDocumentView : System.Windows.Controls.UserControl
	{
		public ShellDocumentView(ShellDocument doc)
		{
			InitializeComponent();

			ShellDocument = doc;

			uxTextEditor.SetHighlighting("XML");
			uxTextEditor.Document.UndoStack.OperationPushed += UndoStack_OperationPushed;
			uxTextEditor.Document.DocumentChanged += Document_DocumentChanged;
			uxTextEditor.Document.DocumentAboutToBeChanged += Document_DocumentAboutToBeChanged;
		}

		public ShellDocument ShellDocument { get; private set; }

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
				ShellDocument.Context.UndoManager.Done(new TextAction(lastOperation));
				lastOperation = null;
			}
		}

		//public void JumpToError(XamlDocumentError error)
		//{
		//    ShellDocument.Mode = DocumentMode.Xaml;
		//    Dispatcher.BeginInvoke(new Action(delegate {
		//        uxTextEditor.ActiveTextAreaControl.JumpTo(error.LineNumber - 1, error.LinePosition - 1);
		//    }), DispatcherPriority.Background);
		//}
	}
}
