/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 29.03.2012
 * Time: 21:01
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

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for SelectStylePanelXaml.xaml
	/// </summary>
	public partial class SelectStylePanel :OptionPanel
	{
		public SelectStylePanel()
		{
			InitializeComponent();
		}
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			
			this.showExtensionsCheckBox.IsChecked =  PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			AddInTreeNode treeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			
			foreach (Codon codon in treeNode.Codons) {
				this.selectAmbienceComboBox.Items.Add(codon.Id);
			}
			
			this.selectAmbienceComboBox.Text = PropertyService.Get("SharpDevelop.UI.CurrentAmbience", "C#");
			
			this.preferProjectAmbienceCheckBox.IsChecked = AmbienceService.UseProjectAmbienceIfPossible;
			
			this.showStatusBarCheckBox.IsChecked = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
			this.showToolBarCheckBox.IsChecked   = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", true);
			this.useProfessionalStyleCheckBox.IsChecked = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", true);
		}
		
		
		public override bool SaveOptions()
		{
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions",this.showExtensionsCheckBox.IsChecked);
			PropertyService.Set("SharpDevelop.UI.CurrentAmbience", this.selectAmbienceComboBox.Text);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", this.showStatusBarCheckBox.IsChecked);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.ToolBarVisible", this.showToolBarCheckBox.IsChecked);
			PropertyService.Set("ICSharpCode.SharpDevelop.Gui.UseProfessionalRenderer", this.useProfessionalStyleCheckBox.IsChecked);
			AmbienceService.UseProjectAmbienceIfPossible = (bool)this.preferProjectAmbienceCheckBox.IsChecked;
			return true;
		}
	}
}