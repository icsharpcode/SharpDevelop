/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 01.05.2012
 * Time: 18:31
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for GeneralOptionsPanelXaml.xaml
	/// </summary>
	public partial class GeneralOptionsPanel : OptionPanel
	{
		public GeneralOptionsPanel()
		{
			InitializeComponent();
		}
		
		public static bool UseSmartTags {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.UseSmartTags", true);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.UseSmartTags", value);
			}
		}
		
		public static bool SmartTagAutoShow {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", true);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", value);
			}
		}
		
		public static bool InsertTodoComment {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.InsertTodoComment", false);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.InsertTodoComment", value);
			}
		}
		
		public static bool GenerateVisualStudioStyleEventHandlers {
			get {
				return PropertyService.Get("FormsDesigner.DesignerOptions.GenerateVisualStudioStyleEventHandlers", false);
			}
			set {
				PropertyService.Set("FormsDesigner.DesignerOptions.GenerateVisualStudioStyleEventHandlers", value);
			}
		}
		
		
		public override bool SaveOptions()
		{
			
			PropertyService.Set("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", sortAlphabeticalCheckBox.IsChecked);
			PropertyService.Set("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", optimizedCodeGenerationCheckBox.IsChecked);
			SmartTagAutoShow = (bool)this.smartTagAutoShowCheckBox.IsChecked;
			PropertyService.Set("FormsDesigner.DesignerOptions.EnableInSituEditing", inPlaceEditCheckBox.IsChecked);
			UseSmartTags = (bool)useSmartTagsCheckBox.IsChecked;
			InsertTodoComment = (bool)insertTodoCommentCheckBox.IsChecked;
			GenerateVisualStudioStyleEventHandlers = (bool)generateVSStyleHandlersCheckBox.IsChecked;
			
			return true;
		}
		
		
		public override void LoadOptions()
		{
			base.LoadOptions();
			Initialize();
		}
		
		private void Initialize()
		{
			this.sortAlphabeticalCheckBox.IsChecked =  PropertyService.Get("FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
			this.optimizedCodeGenerationCheckBox.IsChecked = PropertyService.Get("FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", true);
			this.smartTagAutoShowCheckBox.IsChecked        = SmartTagAutoShow;
			this.inPlaceEditCheckBox.IsChecked             = PropertyService.Get("FormsDesigner.DesignerOptions.EnableInSituEditing", true);
			this.useSmartTagsCheckBox.IsChecked            = UseSmartTags;
			this.insertTodoCommentCheckBox.IsChecked       = InsertTodoComment;
			this.generateVSStyleHandlersCheckBox.IsChecked = GenerateVisualStudioStyleEventHandlers;
		}
	}
}