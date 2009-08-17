// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Document;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Xml;

namespace XmlDOM
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		AXmlParser parser;
		
		bool textDirty = true;
		
		public Window1()
		{
			InitializeComponent();
		}
		
		TextMarkerService markerService;
		List<DocumentChangeEventArgs> changes = new List<DocumentChangeEventArgs>();
		
		protected override void OnInitialized(EventArgs e)
		{
			markerService = new TextMarkerService(editor.TextArea);
			
			editor.TextArea.TextView.MouseMove += new MouseEventHandler(editor_TextArea_TextView_MouseMove);
			
			editor.Document.Changed += delegate(object sender, DocumentChangeEventArgs e2) {
				textDirty = true;
				changes.Add(e2);
			};
			parser = new AXmlParser();

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(0.5);
			timer.Tick += delegate { Button_Click(null, null); };
			timer.Start();
			
			base.OnInitialized(e);
		}

		void editor_TextArea_TextView_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = editor.TextArea.TextView.GetPosition(e.GetPosition(editor.TextArea.TextView));
			if (pos.HasValue) {
				int offset = editor.Document.GetOffset(new TextLocation(pos.Value.Line, pos.Value.Column));
				var marker = markerService.markers.FindSegmentsContaining(offset).FirstOrDefault();
				if (marker != null) {
					errorText.Text = (string)marker.Tag;
				} else {
					errorText.Text = string.Empty;
				}
			}
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			if (!textDirty) return;
			AXmlDocument doc;
			parser.Lock.EnterWriteLock();
			try {
				doc = parser.Parse(editor.Document.Text, changes);
				changes.Clear();
			} finally {
				parser.Lock.ExitWriteLock();
			}
			if (treeView.Items.Count == 0) {
				treeView.Items.Add(doc);
			}
			PrettyPrintAXmlVisitor visitor = new PrettyPrintAXmlVisitor();
			visitor.VisitDocument(doc);
			string prettyPrintedText = visitor.Output;
			if (prettyPrintedText != editor.Document.Text) {
				MessageBox.Show("Error - Original and pretty printed version of XML differ");
			}
			markerService.RemoveAll(m => true);
			foreach(var error in doc.SyntaxErrors) {
				var marker = markerService.Create(error.StartOffset, error.EndOffset - error.StartOffset);
				marker.Tag = error.Message;
				marker.BackgroundColor = Color.FromRgb(255, 150, 150);
			}
			textDirty = false;
		}

		void BindObject(object sender, EventArgs e)
		{
			TextBlock textBlock = (TextBlock)sender;
			AXmlObject node = (AXmlObject)textBlock.DataContext;
			node.Changed += delegate {
				BindingOperations.GetBindingExpression(textBlock, TextBlock.TextProperty).UpdateTarget();
				textBlock.Background = new SolidColorBrush(Colors.LightGreen);
				Storyboard sb = ((Storyboard)this.FindResource("anim"));
				Storyboard.SetTarget(sb, textBlock);
				sb.Begin();
			};
		}
	}
}