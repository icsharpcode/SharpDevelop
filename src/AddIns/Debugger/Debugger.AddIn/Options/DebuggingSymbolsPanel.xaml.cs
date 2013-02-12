/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 12.01.2013
 * Time: 17:34
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for DebuggingSymbolsPanelXaml.xaml
	/// </summary>
	public partial class DebuggingSymbolsPanel : OptionPanel
	{
		public DebuggingSymbolsPanel()
		{
			InitializeComponent();
			editor.TitleText = StringParser.Parse("${res:Global.Folder}:");
			editor.ListCaption = StringParser.Parse("${res:Dialog.Options.IDEOptions.Debugging.Symbols.ListCaption}");
			this.DataContext = this;
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			editor.LoadList(DebuggingOptions.Instance.SymbolsSearchPaths);
		}
		
		
		public override bool SaveOptions()
		{
			DebuggingOptions.Instance.SymbolsSearchPaths = editor.GetList();
			if (WindowsDebugger.CurrentDebugger != null)
				WindowsDebugger.CurrentDebugger.ReloadOptions();
			return base.SaveOptions();
		}
	}
}