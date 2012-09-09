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
			OptionExplicitItems = new List<StringPair>();
			OptionExplicitItems.Add(new StringPair("Off", "Explicit Off"));
			OptionExplicitItems.Add(new StringPair("On", "Explicit On"));
			OptionExplicitItem = OptionExplicitItems[0];
			
			OptionStrictItems = new List<StringPair>();
			OptionStrictItems.Add( new StringPair("Off", "Strict Off"));
			OptionStrictItems.Add(new StringPair("On", "Strict On"));
			OptionStrictItem = OptionStrictItems[0];
			
			OptionCompareItems = new List<StringPair>();
			OptionCompareItems.Add(new StringPair("Binary", "Compare Binary"));
			 OptionCompareItems.Add(new StringPair("Text", "Compare Text"));
			 OptionCompareItem = OptionCompareItems[0];
			 
			 OptionInferItems = new List<StringPair>();
			OptionInferItems.Add(new StringPair("Off", "Infer Off"));
			 OptionInferItems.Add(new StringPair("On", "Infer On"));
			 OptionInferItem = OptionInferItems[0];
		}
		
		public ProjectProperty<string> DefineConstants {
			get { return GetProperty("DefineConstants", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> Optimize {
			get { return GetProperty("Optimize", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> RemoveIntegerChecks {
			get { return GetProperty("RemoveIntegerChecks", "", TextBoxEditMode.EditRawProperty); }
		}
		
		public ProjectProperty<string> OptionExplicit {
			get { return GetProperty("OptionExplicit", "", TextBoxEditMode.EditRawProperty); }
		}
		
			public ProjectProperty<string> OptionStrict {
			get { return GetProperty("OptionStrict", "", TextBoxEditMode.EditRawProperty); }
		}
		
			public ProjectProperty<string> OptionCompare {
			get { return GetProperty("OptionCompare", "", TextBoxEditMode.EditRawProperty); }
		}
		
			public ProjectProperty<string> OptionInfer {
			get { return GetProperty("OptionInfer", "", TextBoxEditMode.EditRawProperty); }
		}
		
		#region OptionItems
		
			List<StringPair> optionExplicitItems;
			
			public List<StringPair> OptionExplicitItems {
				get { return optionExplicitItems; }
				set { optionExplicitItems = value; 
					base.RaisePropertyChanged(() => OptionExplicitItems);
				}
			}
			
			private StringPair optionExplicitItem;
			
			public KeyValuePair<string, string> OptionExplicitItem {
				get { return optionExplicitItem; }
				set { optionExplicitItem = value;
					base.RaisePropertyChanged(() => OptionExplicitItem);
				}
			}
			
			
			List<StringPair> optionStrictItems;
			
			public List<KeyValuePair<string, string>> OptionStrictItems {
				get { return optionStrictItems; }
				set { optionStrictItems = value;
					base.RaisePropertyChanged(() => OptionStrictItems);
				}
			}
			
			private StringPair optionStrictItem;
			
			public KeyValuePair<string, string> OptionStrictItem {
				get { return optionStrictItem; }
				set { optionStrictItem = value;
					base.RaisePropertyChanged(() => OptionStrictItem);
				}
			}
			
			private List<StringPair> optionCompareItems;
			
			public List<KeyValuePair<string, string>> OptionCompareItems {
				get { return optionCompareItems; }
				set { optionCompareItems = value;
					base.RaisePropertyChanged(() => OptionCompareItems);
				}
			}
			
			private StringPair optionCompareItem;
			
			public KeyValuePair<string, string> OptionCompareItem {
				get { return optionCompareItem; }
				set { optionCompareItem = value;
					base.RaisePropertyChanged(() => OptionCompareItem);
				}
			}
			
			List<StringPair> optionInferItems;
			
			public List<KeyValuePair<string, string>> OptionInferItems {
				get { return optionInferItems; }
				set { optionInferItems = value;
					base.RaisePropertyChanged(()=>OptionInferItems);
				}
			}
			
			
		private StringPair optionInferItem;
			
			public KeyValuePair<string, string> OptionInferItem {
				get { return optionInferItem; }
				set { optionInferItem = value;
					base.RaisePropertyChanged(() => OptionInferItem);
				}
			}
		#endregion
	}
}