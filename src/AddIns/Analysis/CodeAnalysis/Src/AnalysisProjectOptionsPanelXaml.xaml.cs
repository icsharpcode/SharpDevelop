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
		
		private List<KeyItemPair> bla = new List<KeyItemPair>();
		
		public AnalysisProjectOptionsPanelXaml()
		{
			InitializeComponent();
			DataContext = this;
			
			bla.Add(new KeyItemPair("Warning","Warning"));
			bla.Add(new KeyItemPair("Error","Error"));
			bla.Add(new KeyItemPair("None","None"));
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
						////						OnOptionChanged(EventArgs.Empty);
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
			foreach (SharpTreeNode cat in ruleTreeView.Root.Children) {
				bool noneChecked = true;
				foreach (RuleTreeNode rtn in cat.Children) {
					if ((bool)rtn.IsChecked) {
						noneChecked = false;
						break;
					}
				}
				cat.IsChecked = !noneChecked;
			}
			userCheck = true;
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
						ruleTreeView.Root.Children.Add(catNode);
						foreach (RuleTreeNode ruleNode in catNode.Children) {
							rules[ruleNode.Identifier] = ruleNode;
						}
					}
					initSuccess = true;
					ReadRuleString();
				}
			}
		}
		
		string[] GetRuleAssemblyList(bool replacePath)
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
		
	
		#endregion
	
		
		#region TreeNodes
		
		class BaseTree:SharpTreeNode
		{
			private Icon ruleIcon;
			private int index;
			
			public BaseTree(IEnumerable<KeyItemPair> ruleState)
			{
				this.RuleState = ruleState;
				RuleIcon = SystemIcons.Warning;
			}
			
			public IEnumerable<KeyItemPair> RuleState {get;set;}
			
			
			public Icon RuleIcon {
				get { return ruleIcon; }
				set { ruleIcon = value;
					base.RaisePropertyChanged("RuleIcon");
				}
			}
			
			
			public int Index {
				get { return index; }
				set { index = value;
					switch (index) {
						case 0:
								RuleIcon = SystemIcons.Warning;
							break;
						case  1:
								RuleIcon = SystemIcons.Error;
							break;
						case 2:
								RuleIcon = null;
							break;
						default:
							
							break;
					}
					base.RaisePropertyChanged("Index");
				}
			}
		}
		
		
		class CategoryTreeNode : BaseTree
		{
			internal FxCopCategory category;
			
			public CategoryTreeNode(FxCopCategory category,IEnumerable<KeyItemPair> bla):base(bla)
			{
				this.category = category;
				foreach (FxCopRule rule in category.Rules) {
					this.Children.Add(new RuleTreeNode(rule,bla));
				}
			
			}
			
			
			public override bool IsCheckable {
				get { return true; }
			}
			
			public override object Text {
				get { return category.DisplayName; }
			}
			
			
			internal int ErrorState {
				get {
					bool allWarn = true;
					bool allErr = true;
					foreach (RuleTreeNode tn in Children) {
						if (tn.isError)
							allWarn = false;
						else
							allErr = false;
					}
					if (allErr)
						return 1;
					else if (allWarn)
						return 0;
					else 
						return -1;
				}
			}
		}
		
		
		class RuleTreeNode :BaseTree
		{
			internal FxCopRule rule;
			internal bool isError;
			
			public RuleTreeNode(FxCopRule rule,IEnumerable<KeyItemPair> ruleState):base(ruleState)
			{
				this.rule = rule;
			}
			
			public override bool IsCheckable {
				get { return true; }
			}
			
			public override object Text {
				get { return rule.CheckId + " : " + rule.DisplayName; }
			}
			
			public string Identifier {
				get {
					return rule.CategoryName + "#" + rule.CheckId;
				}
			}
		}
		
		class MessageNode : SharpTreeNode
		{
			private string message;
			
			public MessageNode (string message)
			{
				this.message = message;
			}
			
			public override object Text {
				get { return message; }
			}
		}
		#endregion
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