/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 07.08.2014
 * Time: 20:13
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.Reporting.BaseClasses;
using Xceed.Wpf.Toolkit;
using ICSharpCode.Reporting.Addin.Globals;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;

namespace ICSharpCode.Reporting.Addin.ReportWizard.Dialog
{
	/// <summary>
	/// Interaction logic for PushDataReport.xaml
	/// </summary>
	public partial class PushDataReport : WizardPage,IHasContext
	{
		List<AbstractColumn> items;
		PushModelContext context;
		
		public PushDataReport()
		{
			InitializeComponent();
			items = new List<AbstractColumn>();
			_DataGrid.ItemsSource = items;
			this.context = new PushModelContext();
			cboType.ItemsSource = GlobalLists.DataTypeList();
			
			var definitions = GetTypeDefinitions();
			if (definitions.Any()) {
				_cboTypes.Visibility = System.Windows.Visibility.Visible;
				_availTxt.Visibility = System.Windows.Visibility.Visible;
				_cboTypes.ItemsSource = definitions;
				_cboTypes.SelectedIndex = 0;
			} else {
				var data = new AbstractColumn("MyColumn", typeof(string));
				items.Add(data);
				_projTxt.Text = ResourceService.GetString("SharpReport.Wizard.PushModel.NoProject");
			}
		}

		static IEnumerable<ITypeDefinition> GetTypeDefinitions()
		{
			var currentProject = SharpDevelop.SD.ProjectService.CurrentProject;
			var compilation = SharpDevelop.SD.ParserService.GetCompilation(currentProject);
			var definitions = compilation.MainAssembly.TopLevelTypeDefinitions.Where(x => x.Properties.Any());
			return definitions;
		}
		
		
		#region Combo
		
		void _cboTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e){
		
			var typeDefinition = (ITypeDefinition)e.AddedItems[0];
			var itemsList = CreateItemsSource(typeDefinition);
			if (itemsList.Count > 0) {
				_DataGrid.ItemsSource = itemsList;
			}
		}

		
		static List<AbstractColumn>  CreateItemsSource(ITypeDefinition typeDefinitions){
			return typeDefinitions.Properties.Select(p => new AbstractColumn(){
				                        	ColumnName = p.Name,
				                        	DataTypeName = p.ReturnType.ReflectionName
				                        }).ToList();
		}
		#endregion
		
		void UpdateContext(){
			context.Items = (List<AbstractColumn>)_DataGrid.ItemsSource;
		}
		
		
		#region IHasContext implementation

		public IWizardContext Context {
			get {
				UpdateContext();
				return context;
			}
		}

		public WizardPageType ReportPageType {
			get {return WizardPageType.PushModelPage;}	
		}
		
		#endregion
	}
	
}