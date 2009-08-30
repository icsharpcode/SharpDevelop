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

namespace ICSharpCode.XamlDesigner
{
	public partial class DocumentView
	{
		public DocumentView(Document doc)
		{
			InitializeComponent();

			Document = doc;
			Shell.Instance.Views[doc] = this;			

			uxTextEditor.SetHighlighting("XML");
			uxTextEditor.DataBindings.Add("Text", doc, "Text", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged);
			Document.Mode = DocumentMode.Design;
		}

		public Document Document { get; private set; }

		public void JumpToError(XamlError error)
		{
			Document.Mode = DocumentMode.Xaml;
			Dispatcher.BeginInvoke(new Action(delegate {
				uxTextEditor.ActiveTextAreaControl.JumpTo(error.Line - 1, error.Column - 1);
			}), DispatcherPriority.Background);
		}
	}
}
