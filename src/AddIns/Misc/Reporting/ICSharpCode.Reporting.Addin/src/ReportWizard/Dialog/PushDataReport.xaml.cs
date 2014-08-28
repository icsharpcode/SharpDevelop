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
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.Reporting.BaseClasses;
using Xceed.Wpf.Toolkit;
using ICSharpCode.Reporting.Addin.Globals;
using ICSharpCode.Reporting.Addin.ReportWizard.ViewModels;
using System.Linq;

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
			var data = new AbstractColumn("MyColumn",typeof(string));
			items.Add(data);
			cboType.ItemsSource = GlobalLists.DataTypeList();
			var definitions = GetTypeDefinitions();
			
			if (definitions != null) {
				_cboTypes.Visibility = System.Windows.Visibility.Visible;
				_cboTypes.ItemsSource = definitions;
				_cboTypes.SelectedIndex = 0;
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
		
			var s = (ITypeDefinition)e.AddedItems[0];
			var l = CreateItemsSource(s);
			if (l.Count > 0) {
				_DataGrid.ItemsSource = l;
			}
		}

		static List<AbstractColumn>  CreateItemsSource(ITypeDefinition s){
			return s.Properties.Select(p => new AbstractColumn(){
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
	
	/*
	private void PopulateTestItems()
        {
            TestItems = new ObservableCollection<TestItem>();
            for (int i = 0; i < 5; i++)
            {
                TestItem ti = new TestItem();
                ti.Name = "TestItem" + i;
                ti.IsSelected = true;
                TestItems.Add(ti);
            }
        }

        private bool _AllSelected;
        public bool AllSelected
        {
            get { return _AllSelected; }
            set
            {
                _AllSelected = value;
                TestItems.ToList().ForEach(x => x.IsSelected = value);
                NotifyPropertyChanged(m => m.AllSelected);
            }
        }

    private ObservableCollection<TestItem> _TestItems;
    public ObservableCollection<TestItem> TestItems
    {
        get { return _TestItems; }
        set
        {
            _TestItems = value;
            NotifyPropertyChanged(m => m.TestItems);
        }
    }*/
}