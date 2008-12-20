// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CodeAnalysis
{
	public partial class AnalysisProjectOptions
	{
		Dictionary<string, RuleTreeNode> rules = new Dictionary<string, RuleTreeNode>();
		
		public AnalysisProjectOptions()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			enableCheckBox.Text = StringParser.Parse(enableCheckBox.Text);
			ruleLabel.Text = StringParser.Parse(ruleLabel.Text);
			warningOrErrorLabel.Text = StringParser.Parse(warningOrErrorLabel.Text);
			changeRuleAssembliesButton.Text = StringParser.Parse(changeRuleAssembliesButton.Text);
			
			ruleLabel.SizeChanged += delegate { ruleTreeView.Invalidate(); };
		}
		
		bool initSuccess;
		
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
			
			if (ruleTreeView.Nodes.Count == 0 && this.Visible) {
				ReloadRuleList();
			}
		}
		
		void ReloadRuleList()
		{
			ruleTreeView.Nodes.Clear();
			FxCopWrapper.GetRuleList(GetRuleAssemblyList(true), Callback);
			if (ruleTreeView.Nodes.Count == 0) {
				ruleTreeView.Nodes.Add(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.LoadingRules}"));
			}
		}
		
		void Callback(List<FxCopCategory> ruleList)
		{
			if (WorkbenchSingleton.InvokeRequired) {
				WorkbenchSingleton.SafeThreadAsyncCall((Action<List<FxCopCategory>>)Callback, ruleList);
			} else {
				ruleTreeView.Nodes.Clear();
				rules.Clear();
				if (ruleList == null || ruleList.Count == 0) {
					ruleTreeView.Nodes.Add(new TreeNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.CannotFindFxCop}")));
					ruleTreeView.Nodes.Add(new TreeNode(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.SpecifyFxCopPath}")));
				} else {
					foreach (FxCopCategory cat in ruleList) {
						CategoryTreeNode catNode = new CategoryTreeNode(cat);
						ruleTreeView.Nodes.Add(catNode);
						foreach (RuleTreeNode ruleNode in catNode.Nodes) {
							rules[ruleNode.Identifier] = ruleNode;
						}
					}
					initSuccess = true;
					ReadRuleString();
				}
			}
		}
		
		#region TreeView drawing
		class CategoryTreeNode : TreeNode
		{
			internal FxCopCategory category;
			
			public CategoryTreeNode(FxCopCategory category)
			{
				this.category = category;
				this.Text = category.DisplayName;
				foreach (FxCopRule rule in category.Rules) {
					this.Nodes.Add(new RuleTreeNode(rule));
				}
			}
			
			internal int ErrorState {
				get {
					bool allWarn = true;
					bool allErr = true;
					foreach (RuleTreeNode tn in Nodes) {
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
		
		class RuleTreeNode : TreeNode
		{
			internal FxCopRule rule;
			internal bool isError;
			
			public RuleTreeNode(FxCopRule rule)
			{
				this.rule = rule;
				this.Text = rule.CheckId + " : " + rule.DisplayName;
			}
			
			public string Identifier {
				get {
					return rule.CategoryName + "#" + rule.CheckId;
				}
			}
		}
		
		void RuleTreeViewDrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			if (e.Bounds.X < 5) return;
			int state;
			if (e.Node is CategoryTreeNode) {
				state = (e.Node as CategoryTreeNode).ErrorState;
			} else if (e.Node is RuleTreeNode) {
				state = (e.Node as RuleTreeNode).isError ? 1 : 0;
			} else {
				e.DrawDefault = true;
				return;
			}
			e.DrawDefault = false;
			if ((e.State & TreeNodeStates.Selected) == TreeNodeStates.Selected) {
				e.Graphics.DrawString(e.Node.Text, ruleTreeView.Font, SystemBrushes.HighlightText, e.Bounds.Location);
			} else {
				e.Graphics.DrawString(e.Node.Text, ruleTreeView.Font, SystemBrushes.WindowText, e.Bounds.Location);
			}
			e.Graphics.DrawLine(SystemPens.WindowFrame, ruleLabel.Width, e.Bounds.Top, ruleLabel.Width, e.Bounds.Bottom);
			if (state == 0) {
				// Warning
				e.Graphics.DrawIcon(SystemIcons.Warning, new Rectangle(ruleLabel.Width + 4, e.Bounds.Y, 16, 16));
				e.Graphics.DrawString(ResourceService.GetString("Global.WarningText"), ruleTreeView.Font, SystemBrushes.WindowText, ruleLabel.Width + 24, e.Bounds.Y);
			} else if (state == 1) {
				// Error
				e.Graphics.DrawIcon(SystemIcons.Error, new Rectangle(ruleLabel.Width + 4, e.Bounds.Y, 16, 16));
				e.Graphics.DrawString(ResourceService.GetString("Global.ErrorText"), ruleTreeView.Font, SystemBrushes.WindowText, ruleLabel.Width + 24, e.Bounds.Y);
			} else {
				// Mixed
				e.Graphics.DrawString(StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.WarningErrorMixed}"),
				                      ruleTreeView.Font, SystemBrushes.WindowText, ruleLabel.Width + 24, e.Bounds.Y);
			}
		}
		#endregion
		
		#region Rule String Property
		string CreateRuleString()
		{
			StringBuilder b = new StringBuilder();
			foreach (TreeNode category in ruleTreeView.Nodes) {
				foreach (RuleTreeNode rule in category.Nodes) {
					if (!rule.Checked || rule.isError) {
						if (b.Length > 0)
							b.Append(';');
						if (rule.Checked)
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
			foreach (TreeNode cat in ruleTreeView.Nodes) {
				foreach (RuleTreeNode rtn in cat.Nodes) {
					rtn.Checked = true;
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
					ruleNode.Checked = active;
					ruleNode.isError = error;
				}
			}
			foreach (TreeNode cat in ruleTreeView.Nodes) {
				bool noneChecked = true;
				foreach (RuleTreeNode rtn in cat.Nodes) {
					if (rtn.Checked) {
						noneChecked = false;
						break;
					}
				}
				cat.Checked = !noneChecked;
			}
			ruleTreeView.Invalidate();
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
						OnOptionChanged(EventArgs.Empty);
						ReloadRuleList();
					}
				}
			}
		}
		#endregion
		
		#region ConfigurationGuiBinding
		public CheckBox EnableCheckBox {
			get {
				return enableCheckBox;
			}
		}
		
		public ConfigurationGuiBinding CreateBinding()
		{
			return new ConfigBinding(this);
		}
		
		class ConfigBinding : ConfigurationGuiBinding
		{
			readonly AnalysisProjectOptions po;
			
			public ConfigBinding(AnalysisProjectOptions po)
			{
				this.po = po;
				this.TreatPropertyValueAsLiteral = false;
				po.OptionChanged += delegate {
					Helper.IsDirty = true;
				};
			}
			
			public override void Load()
			{
				po.RuleString = Get("");
				po.RuleAssemblies = Helper.GetProperty("CodeAnalysisRuleAssemblies", "", false);
			}
			
			public override bool Save()
			{
				Set(po.RuleString);
				Helper.SetProperty("CodeAnalysisRuleAssemblies", 
				                   (po.RuleAssemblies == DefaultRuleAssemblies) ? "" : po.RuleAssemblies,
				                   false,
				                   Location);
				return true;
			}
		}
		#endregion
		
		public event EventHandler OptionChanged;
		
		protected virtual void OnOptionChanged(EventArgs e)
		{
			if (OptionChanged != null) {
				OptionChanged(this, e);
			}
		}
		
		void RuleTreeViewMouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && e.X > ruleLabel.Width) {
				TreeNode node = ruleTreeView.GetNodeAt(20, e.Y);
				if (node != null) {
					if (node is RuleTreeNode) {
						((RuleTreeNode)node).isError = !((RuleTreeNode)node).isError;
					} else if (node is CategoryTreeNode) {
						if ((node as CategoryTreeNode).ErrorState == 0) {
							foreach (RuleTreeNode rtn in node.Nodes) {
								rtn.isError = true;
							}
						} else {
							foreach (RuleTreeNode rtn in node.Nodes) {
								rtn.isError = false;
							}
						}
					}
					ruleTreeView.Invalidate();
					OnOptionChanged(EventArgs.Empty);
				}
			}
		}
		
		bool userCheck;
		
		void RuleTreeViewAfterCheck(object sender, TreeViewEventArgs e)
		{
			if (userCheck) {
				if (e.Node is CategoryTreeNode) {
					userCheck = false;
					foreach (TreeNode subNode in e.Node.Nodes) {
						subNode.Checked = e.Node.Checked;
					}
					userCheck = true;
				} else if (e.Node is RuleTreeNode) {
					userCheck = false;
					bool anyChecked = false;
					foreach (TreeNode sibling in e.Node.Parent.Nodes) {
						if (sibling.Checked)
							anyChecked = true;
					}
					e.Node.Parent.Checked = anyChecked;
					userCheck = true;
				}
				OnOptionChanged(EventArgs.Empty);
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
		
		void ChangeRuleAssembliesButtonClick(object sender, EventArgs e)
		{
			using (Form frm = new Form()) {
				frm.Text = changeRuleAssembliesButton.Text;
				
				StringListEditor ed = new StringListEditor();
				ed.Dock = DockStyle.Fill;
				ed.ManualOrder = false;
				ed.BrowseForDirectory = true;
				ed.AutoAddAfterBrowse = true;
				ed.TitleText = StringParser.Parse("${res:ICSharpCode.CodeAnalysis.ProjectOptions.ChooseRuleAssemblyDirectory}");
				
				ed.LoadList(GetRuleAssemblyList(false));
				FlowLayoutPanel p = new FlowLayoutPanel();
				p.Dock = DockStyle.Bottom;
				p.FlowDirection = FlowDirection.RightToLeft;
				
				Button btn = new Button();
				p.Height = btn.Height + 8;
				btn.DialogResult = DialogResult.Cancel;
				btn.Text = ResourceService.GetString("Global.CancelButtonText");
				frm.CancelButton = btn;
				p.Controls.Add(btn);
				
				btn = new Button();
				btn.DialogResult = DialogResult.OK;
				btn.Text = ResourceService.GetString("Global.OKButtonText");
				frm.AcceptButton = btn;
				p.Controls.Add(btn);
				
				frm.Controls.Add(ed);
				frm.Controls.Add(p);
				
				frm.FormBorderStyle = FormBorderStyle.FixedDialog;
				frm.MaximizeBox = false;
				frm.MinimizeBox = false;
				frm.ClientSize = new Size(400, 300);
				frm.StartPosition = FormStartPosition.CenterParent;
				
				if (frm.ShowDialog(FindForm()) == DialogResult.OK) {
					StringBuilder b = new StringBuilder(DefaultRuleAssemblies);
					foreach (string asm in ed.GetList()) {
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
		}
	}
}
