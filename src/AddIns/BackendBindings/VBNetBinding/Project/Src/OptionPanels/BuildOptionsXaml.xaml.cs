/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.08.2012
 * Time: 19:43
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

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;

namespace ICSharpCode.VBNetBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildOptionsXaml.xaml
	/// </summary>
	public partial class BuildOptionsXaml : ProjectOptionPanel
	{
		public BuildOptionsXaml()
		{
			InitializeComponent();
			DataContext = this;
			
			
			optionExplicitItems = new List<KeyItemPair>();
			optionExplicitItems.Add(new KeyItemPair("Off","Explicit Off"));
			optionExplicitItems.Add(new KeyItemPair("On","Explicit On"));
			OptionExplicitItems = optionExplicitItems;
			
			optionStrictItems = new List<KeyItemPair>();
			optionStrictItems.Add(new KeyItemPair("Off", "Strict Off"));
			optionStrictItems.Add(new KeyItemPair("On", "Strict On"));
			OptionStrictItems = optionStrictItems;
			
			
			optionCompareItems = new List<KeyItemPair>();
			optionCompareItems.Add(new KeyItemPair("Binary", "Compare Binary"));
			optionCompareItems.Add(new KeyItemPair("Text", "Compare Text"));
			OptionCompareItems = optionCompareItems;
			
			optionInferItems = new List<KeyItemPair>();
			optionInferItems.Add(new KeyItemPair("Off", "Infer Off"));
			optionInferItems.Add(new KeyItemPair("On", "Infer On"));
			OptionInferItems = optionInferItems;
			
			IsDirty = false;
		}
		
			
		public ProjectOptionPanel.ProjectProperty<bool> TreatWarningsAsErrors {
			get {
				return GetProperty("TreatWarningsAsErrors", false); }
		}
		
		public ProjectProperty<string> DefineConstants {
			get { return GetProperty("DefineConstants", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<bool> Optimize {
			get { return GetProperty("Optimize", false, PropertyStorageLocations.ConfigurationSpecific); }
		}
		
		public ProjectProperty<string> RemoveIntegerChecks {
			get { return GetProperty("RemoveIntegerChecks", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> OptionExplicit {
			get {return GetProperty("OptionExplicit", "", TextBoxEditMode.EditRawProperty); }	
		}
		
		public ProjectProperty<string> OptionStrict {
			get { return GetProperty("OptionStrict", "Off", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> OptionCompare {
			get { return GetProperty("OptionCompare", "Binary", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> OptionInfer {
			get { return GetProperty("OptionInfer", "Off", TextBoxEditMode.EditRawProperty); }
		}
		
		
		#region OptionItems
		
		List<KeyItemPair> optionExplicitItems;
		
		public List<KeyItemPair> OptionExplicitItems {
			get { return optionExplicitItems; }
			set { optionExplicitItems = value;
				base.RaisePropertyChanged(() => OptionExplicitItems);
			}
		}
	
		
		List<KeyItemPair> optionStrictItems;
		
		public List<KeyItemPair> OptionStrictItems {
			get { return optionStrictItems; }
			set { optionStrictItems = value;
				base.RaisePropertyChanged(() => OptionStrictItems);
			}
		}
		
		
		private List<KeyItemPair> optionCompareItems;
		
		public List<KeyItemPair> OptionCompareItems {
			get { return optionCompareItems; }
			set { optionCompareItems = value;
				base.RaisePropertyChanged(() => OptionCompareItems);
			}
		}
		
		
		List<KeyItemPair> optionInferItems;
		
		public List<KeyItemPair> OptionInferItems {
			get { return optionInferItems; }
			set { optionInferItems = value;
				base.RaisePropertyChanged(()=>OptionInferItems);
			}
		}
		
		
	
		#endregion
		
		#region overrides
		
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
		
//			Console.WriteLine(" 1 - option {0}",TreatWarningsAsErrors.Value);
			
			errorsAndWarnings.SetProjectOptions(this);
			
//			Console.WriteLine(" 2 - option {0}",TreatWarningsAsErrors.Value);
//			Console.WriteLine(" 4 - option {0}",TreatWarningsAsErrors.Value);
//			
		}
		
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			Console.WriteLine(OptionExplicit.Value);
			errorsAndWarnings.SaveTreatWarningAsErrorRadioButtons();
			return base.Save(project, configuration, platform);
		}
		
		#endregion
	}
}