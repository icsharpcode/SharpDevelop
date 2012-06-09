/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 09.06.2012
 * Time: 17:03
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.CodeCoverage
{
	/// <summary>
	/// Interaction logic for CodeCoverageOptionsPanelXaml.xaml
	/// </summary>
	public partial class CodeCoverageOptionsPanelXaml : OptionPanel
	{
		public CodeCoverageOptionsPanelXaml()
		{
			InitializeComponent();
			DisplayItems = new ObservableCollection<CodeCoverageDisplayItem>();
			DataContext = this;
			DisplayItems.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodeCovered}"), CodeCoverageOptions.VisitedColorProperty, CodeCoverageOptions.VisitedColor, CodeCoverageOptions.VisitedForeColorProperty, CodeCoverageOptions.VisitedForeColor));
			DisplayItems.Add(new CodeCoverageDisplayItem(StringParser.Parse("${res:ICSharpCode.CodeCoverage.CodeNotCovered}"), CodeCoverageOptions.NotVisitedColorProperty, CodeCoverageOptions.NotVisitedColor, CodeCoverageOptions.NotVisitedForeColorProperty, CodeCoverageOptions.NotVisitedForeColor));
		}
		
		public ObservableCollection<CodeCoverageDisplayItem> DisplayItems {get; private set;}
	}
}