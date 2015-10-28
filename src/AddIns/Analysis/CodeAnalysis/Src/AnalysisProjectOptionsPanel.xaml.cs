// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.TreeView;

namespace ICSharpCode.CodeAnalysis
{
	public partial class AnalysisProjectOptionsPanel : ProjectOptionPanel
	{
		private bool initSuccess;
		private Dictionary<string, RuleTreeNode> rules = new Dictionary<string, RuleTreeNode>();
		
		public AnalysisProjectOptionsPanel()
		{
			InitializeComponent();
			DataContext = this;
		}
		
		public ProjectProperty<bool> RunCodeAnalysis {
			get { return GetProperty("RunCodeAnalysis", false); }
		}
		
		public ProjectProperty<string> CodeAnalysisRuleAssemblies {
			get { return GetProperty("CodeAnalysisRuleAssemblies", "", TextBoxEditMode.EditEvaluatedProperty); }
		}
		
		
		public ProjectProperty<string> CodeAnalysisRules {
			get { return GetProperty("CodeAnalysisRules", "", TextBoxEditMode.EditRawProperty); }
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
					if (error) {
						ruleNode.Index = 1;
					} else {
						ruleNode.Index = 0;
					}
//					ruleNode.Index = 1;
				}
			}
			SetCategoryIcon();
		}
		
	
		private void SetCategoryIcon() {
			foreach (CategoryTreeNode categoryNode in ruleTreeView.Root.Children) {
				categoryNode.CheckMode();
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
			if (SD.MainThread.InvokeRequired) {
				SD.MainThread.InvokeAsync(() => Callback(ruleList));
			} else {
				ruleTreeView.Root = new SharpTreeNode();
				
				rules.Clear();
				if (ruleList == null || ruleList.Count == 0) {
					ruleTreeView.Root.Children.Add(new MessageNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.CannotFindFxCop}")));
					ruleTreeView.Root.Children.Add(new MessageNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.SpecifyFxCopPath}")));
				} else {
					foreach (FxCopCategory cat in ruleList) {
						CategoryTreeNode catNode = new CategoryTreeNode(cat);
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
				if (e.PropertyName == "Index") {
					initSuccess = false;
					var categoryNode = sender as CategoryTreeNode;
					if (categoryNode != null) {
						foreach (RuleTreeNode rule in categoryNode.Children) {
							rule.Index = categoryNode.Index;
						}
						base.IsDirty = true;
						initSuccess = true;
					}
					var ruleNode = sender as RuleTreeNode;
					if (ruleNode != null) {
						CategoryTreeNode parent = ruleNode.Parent as CategoryTreeNode;
						parent.CheckMode();
						base.IsDirty = true;
						initSuccess = true;
					}
				}
				
				if (e.PropertyName == "IsChecked") {
					base.IsDirty = true;
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
			stringListDialog.ShowBrowse = true;
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
					IsDirty = true;
				} finally {
					initSuccess = oldInitSuccess;
				}
			}
		}
		
		#endregion
	}
}
