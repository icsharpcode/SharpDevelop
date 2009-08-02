// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
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

using ICSharpCode.AvalonEdit.XmlParser;

namespace XmlDOM
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		XmlParser parser;
		
		public Window1()
		{
			InitializeComponent();
		}
		
		protected override void OnInitialized(EventArgs e)
		{
			parser = new XmlParser(editor.Document);

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(0.25);
			timer.Tick += delegate { Button_Click(null, null); };
			timer.Start();
			
			base.OnInitialized(e);
		}
		
		void Button_Click(object sender, RoutedEventArgs e)
		{
			RawDocument doc = parser.Parse();
			if (treeView.Items.Count == 0) {
				treeView.Items.Add(doc);
			}
		}
		
		void BindObject(object sender, EventArgs e)
		{
			TextBlock textBlock = (TextBlock)sender;
			RawObject node = (RawObject)textBlock.DataContext;
			node.LocalDataChanged += delegate {
				BindingOperations.GetBindingExpression(textBlock, TextBlock.TextProperty).UpdateTarget();
				textBlock.Background = new SolidColorBrush(Colors.LightGreen);
				Storyboard sb = ((Storyboard)this.FindResource("anim"));
				Storyboard.SetTarget(sb, textBlock);
				sb.Begin();
			};
		}
		
		void BindElement(object sender, EventArgs e)
		{
			TextBlock textBlock = (TextBlock)sender;
			RawElement node = (RawElement)textBlock.DataContext;
			node.StartTag.LocalDataChanged += delegate {
				BindingOperations.GetBindingExpression(textBlock, TextBlock.TextProperty).UpdateTarget();
				textBlock.Background = new SolidColorBrush(Colors.LightGreen);
				Storyboard sb = ((Storyboard)this.FindResource("anim"));
				Storyboard.SetTarget(sb, textBlock);
				sb.Begin();
			};
		}
	}
}