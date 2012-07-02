/*
 * Created by SharpDevelop.
 * User: Peter Forstmeier
 * Date: 06/09/2012
 * Time: 18:27
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Gui.Dialogs.OptionPanels.ProjectOptions;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeAnalysis
{
	/// <summary>
	/// Interaction logic for AnalysisProjectOptionsPanelXaml.xaml
	/// </summary>
	
	
	public partial class AnalysisProjectOptionsPanelXaml : ProjectOptionPanel
	{
		private bool initSuccess;
		private bool userCheck;
		private Dictionary<string, RuleTreeNode> rules = new Dictionary<string, RuleTreeNode>();
		
		private List<Tuple<Icon,string,int>> bla = new List<Tuple<Icon,string,int>>();
		
		public AnalysisProjectOptionsPanelXaml()
		{
			InitializeComponent();
			DataContext = this;
			bla.Add(Tuple.Create<Icon,string,int>(SystemIcons.Warning,ResourceService.GetString("Global.WarningText"),0));
			bla.Add(Tuple.Create<Icon,string,int>(SystemIcons.Error,ResourceService.GetString("Global.ErrorText"),1));
//			bla.Add(Tuple.Create<Icon,string>(null,"None"));
		}
		
		
		public ProjectProperty<bool> RunCodeAnalysis {
			get { return GetProperty("RunCodeAnalysis", false); }
		}
		
		public ProjectProperty<string> CodeAnalysisRuleAssemblies {
			get { return GetProperty("CodeAnalysisRuleAssemblies","",TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> CodeAnalysisRules {
			get { return GetProperty("CodeAnalysisRules","",TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		#region Rule Assemblies Property
		
		string ruleAssemblies;
		const string DefaultRuleAssemblies = @"$(FxCopDir)\rules";
		
		public string RuleAssemblies {
			get {
				return ruleAssemblies;
			}
			set {
				if (string.IsNullOrEmpty(value)) {
					value = DefaultRuleAssemblies;
				}
				if (ruleAssemblies != value) {
					ruleAssemblies = value;
					
					if (initSuccess) {
						ReloadRuleList();
					}
				}
			}
		}
		
		#endregion
		
		#region Rule string Property
		
		string CreateRuleString()
		{
			StringBuilder b = new StringBuilder();
			foreach (SharpTreeNode category in ruleTreeView.Items) {
				foreach (RuleTreeNode rule in category.Children) {
					if (!(bool)rule.IsChecked || rule.isError) {
						if (b.Length > 0)
							b.Append(';');
						if ((bool)rule.IsChecked)
							b.Append('+');
						else
							b.Append('-');
						if (rule.isError)
							b.Append('!');
						b.Append(rule.Identifier);
					}
				}
			}
			return b.ToString();
		}
		
		
		void ReadRuleString()
		{
			userCheck = false;
			foreach (SharpTreeNode cat in ruleTreeView.Root.Children) {
				foreach (RuleTreeNode rtn in cat.Children) {
					rtn.IsChecked = true;
					rtn.isError = false;
				}
			}
			foreach (string rule2 in ruleString.Split(';')) {
				string rule = rule2;
				if (rule.Length == 0) continue;
				bool active = true;
				bool error = false;
				if (rule.StartsWith("-")) {
					active = false;
					rule = rule.Substring(1);
				} else if (rule.StartsWith("+")) {
					rule = rule.Substring(1);
				}
				if (rule.StartsWith("!")) {
					error = true;
					rule = rule.Substring(1);
				}
				RuleTreeNode ruleNode;
				if (rules.TryGetValue(rule, out ruleNode)) {
					ruleNode.IsChecked = active;
					ruleNode.isError = error;
				}
			}
			userCheck = true;
//			SetCategoryIcon();
			SetCategoryIcon();
		}

		/*
		void oldSetCategoryIcon()
		{
			foreach (CategoryTreeNode element in ruleTreeView.Root.Children) {
				Console.WriteLine(" {0} - errorstate {1}",element.Text, element.ErrorState.ToString());
				if (element.ErrorState > -1) {
					element.Index = element.ErrorState;
				}
				Console.WriteLine("-------------------------");
			}
			
		}
		 */
		
		
		void SetCategoryIcon() {
			foreach (CategoryTreeNode element in ruleTreeView.Root.Children) {
//				Console.WriteLine(" {0} ",element.Text);
				if (!element.NewErrorState.HasValue) {
					Console.WriteLine (" {0} is Mixed Mode",element.Text);
				} else{
					if (element.NewErrorState == true) {
						Console.WriteLine (" {0} is Error",element.Text);
						element.Index = 1;
					} else {
						Console.WriteLine (" {0} is Warning",element.Text);
					}
				}
			}
		}
		
		string ruleString = "";
		
		public string RuleString {
			get {
				if (initSuccess)
					return CreateRuleString();
				else
					return ruleString;
			}
			set {
				ruleString = value;
				if (initSuccess) {
					ReadRuleString();
				}
			}
		}
		
		#endregion
		
		#region overrides
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			RuleString = this.CodeAnalysisRules.Value;
			RuleAssemblies = CodeAnalysisRuleAssemblies.Value;
			ReloadRuleList();
		}
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			this.CodeAnalysisRules.Value = RuleString;
			return base.Save(project, configuration, platform);
		}

		#endregion
		
		
		#region RuleList
		
		void ReloadRuleList()
		{
			ruleTreeView.Root = new SharpTreeNode();
			FxCopWrapper.GetRuleList(GetRuleAssemblyList(true), Callback);
			if (ruleTreeView.Root.Children.Count == 0) {
				ruleTreeView.Root.Children.Add(new MessageNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.LoadingRules}")));
			}
		}
		
		
		void Callback(List<FxCopCategory> ruleList)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall((Action<List<FxCopCategory>>)Callback, ruleList);
			} else {
				ruleTreeView.Root = new SharpTreeNode();
				
				
				rules.Clear();
				if (ruleList == null || ruleList.Count == 0) {
					ruleTreeView.Root.Children.Add(new MessageNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.CannotFindFxCop}")));
					ruleTreeView.Root.Children.Add(new MessageNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.SpecifyFxCopPath}")));
				} else {
					foreach (FxCopCategory cat in ruleList) {
						CategoryTreeNode catNode = new CategoryTreeNode(cat,bla);
						catNode.PropertyChanged += OnPropertyChanged;
						ruleTreeView.Root.Children.Add(catNode);
						foreach (RuleTreeNode ruleNode in catNode.Children) {
							ruleNode.PropertyChanged += OnPropertyChanged;
							rules[ruleNode.Identifier] = ruleNode;
						}
					}
					
					ReadRuleString();
					initSuccess = true;
				}
			}
		}
		
		
		private void OnPropertyChanged(object sender,System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (initSuccess) {
				base.IsDirty = true;
				if (e.PropertyName == "Index") {
					RuleTreeNode r = sender as RuleTreeNode;
					if (r != null) {
						var p = r.Parent;
						if (p != null) {
							SetCategoryIcon();
						}
					}
				}
			}
		}
		
		
		private string[] GetRuleAssemblyList(bool replacePath)
		{
			List<string> list = new List<string>();
			string fxCopPath = FxCopWrapper.FindFxCopPath();
			foreach (string dir in ruleAssemblies.Split(';')) {
				if (string.Equals(dir, "$(FxCopDir)\\rules", StringComparison.OrdinalIgnoreCase))
					continue;
				if (string.Equals(dir, "$(FxCopDir)/rules", StringComparison.OrdinalIgnoreCase))
					continue;
				if (replacePath && !string.IsNullOrEmpty(fxCopPath)) {
					list.Add(Regex.Replace(dir, @"\$\(FxCopDir\)", fxCopPath, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase));
				} else {
					list.Add(dir);
				}
			}
			return list.ToArray();
		}
		
		
		private void ChangeRuleAssembliesButtonClick( object sender, RoutedEventArgs e)
		{
			var  stringListDialog = new StringListEditorDialog();
			stringListDialog.BrowseForDirectory = true;
			stringListDialog.TitleText = StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.ChooseRuleAssemblyDirectory}");
			stringListDialog.LoadList(GetRuleAssemblyList(false));
			stringListDialog.ShowDialog();
			if (stringListDialog.DialogResult ?? false) {
				StringBuilder b = new StringBuilder(DefaultRuleAssemblies);
				foreach (string asm in stringListDialog.GetList()) {
					b.Append(';');
					b.Append(asm);
				}
				bool oldInitSuccess = initSuccess;
				initSuccess = true;
				try {
					this.RuleAssemblies = b.ToString();
				} finally {
					initSuccess = oldInitSuccess;
				}
			}
		}
		
		#endregion
	}
	

	public class TypeSelector : DataTemplateSelector
	{
		
		public DataTemplate ComboTemplate { get; set; }
		public DataTemplate TxtTemplate { get; set; }
		
		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			var rule = item as RuleTreeNode;
			
			if (rule != null) {
				return ComboTemplate;
			} 
		
			var cat = item as CategoryTreeNode;
			
			if (cat != null) {
				if (!cat.NewErrorState.HasValue) {
					//Mixed Mode
					return TxtTemplate;
				} else {
					//All childs has same value
					return ComboTemplate;
				}
			}
			return ComboTemplate;
		}	
	}
	
	
	[ValueConversion(typeof(System.Drawing.Icon), typeof(System.Windows.Media.ImageSource))]
	public class ImageConverter : IValueConverter
	{
		public object Convert(object value, Type targetType,
		                      object parameter, CultureInfo culture)
		{
			// empty images are empty...
			if (value == null) { return null; }
			var image = (System.Drawing.Icon)value;
			// Winforms Image we want to get the WPF Image from...
			var bitmap = new System.Windows.Media.Imaging.BitmapImage();
			bitmap.BeginInit();
			MemoryStream memoryStream = new MemoryStream();
			// Save to a memory stream...
			image.Save(memoryStream);
			// Rewind the stream...
			memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
			bitmap.StreamSource = memoryStream;
			bitmap.EndInit();
			return bitmap;
		}
		public object ConvertBack(object value, Type targetType,
		                          object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}