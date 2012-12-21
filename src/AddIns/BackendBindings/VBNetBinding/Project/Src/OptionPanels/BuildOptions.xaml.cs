/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06.08.2012
 * Time: 19:43
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.VBNetBinding.OptionPanels
{
	/// <summary>
	/// Interaction logic for BuildOptionsXaml.xaml
	/// </summary>
	public partial class BuildOptions : ProjectOptionPanel
	{
		public BuildOptions()
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
		
		
		protected override void Initialize()
		{
			base.Initialize();
			buildOutput.Initialize(this);
			this.buildAdvanced.Initialize(this);
			this.errorsAndWarnings.Initialize(this);
			this.treatErrorsAndWarnings.Initialize(this);
		}
		#endregion
	}
}