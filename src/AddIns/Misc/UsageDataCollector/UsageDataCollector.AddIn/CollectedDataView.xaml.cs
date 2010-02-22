// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.AvalonEdit.Folding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;

namespace ICSharpCode.UsageDataCollector
{
	/// <summary>
	/// Interaction logic for CollectedDataView.xaml
	/// </summary>
	public partial class CollectedDataView : Window
	{
		public CollectedDataView(string collectedXml)
		{
			InitializeComponent();
			textEditor.Text = collectedXml;
			textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("XML");
			FoldingManager foldingManager = FoldingManager.Install(textEditor.TextArea);
			new XmlFoldingStrategy().UpdateFoldings(foldingManager, textEditor.Document);
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
