// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Alpert" email="david@spinthemoose.com"/>
//     <version>$Revision:  $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Asks the user about Extract Interface refactoring options.
	/// </summary>
	/// <returns><see cref="ExtractInterfaceOptions"/></returns>
	public partial class ExtractInterfaceDialog : Form
	{
		ExtractInterfaceOptions options;
		bool hasSetFilenameExplicitly;
		
		public ExtractInterfaceDialog()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			this.Owner = WorkbenchSingleton.MainForm;
			this.possibleInterfaceMembers = new List<IMember>();
			this.hasSetFilenameExplicitly = false;
			
			// recursively passes the Text attribute of each Control on this form through our StringParser
			ParseTextFor(this);
		}
		
		/// <summary>
		/// Recursively passes the Text attribute of a <see cref="Control"/> (and it's
		/// child controls) through our <see cref="StringParser"/>, rewriting the
		/// Text values as it goes.
		/// </summary>
		/// <param name="control">The <see cref="Control"/> to process.</param>
		void ParseTextFor(Control control)
		{
			control.Text = StringParser.Parse(control.Text);
			foreach(Control subControl in control.Controls) {
				ParseTextFor(subControl);
			}
		}
		
		/// <summary>
		/// A custom ShowDialog routine that handles pass-through deserialization and reserialization
		/// of a <see cref="ExtractInterfaceOptions"/> object to encapsulate this operation's
		/// parameters.
		/// </summary>
		/// <param name="options">
		/// A <see cref="ExtractInterfaceOptions"/> containing initial, default values for the operation.
		/// </param>
		/// <returns>
		/// A <see cref="ExtractInterfaceOptions"/> reference encapsulating the dialog's parameters.
		/// </returns>
		public ExtractInterfaceOptions ShowDialog(ExtractInterfaceOptions options)
		{
			InitializeFields(options);
			options.IsCancelled = (base.ShowDialog() == DialogResult.Cancel);
			return options;
		}
		
		List<IMember> possibleInterfaceMembers;
		
		void InitializeFields(ExtractInterfaceOptions o)
		{
			this.options = o;

			this.txtInterfaceName.Text = o.NewInterfaceName;
			this.txtNewFileName.Text = o.SuggestedFileName;
			this.txtGeneratedName.Text = o.FullyQualifiedName;
			
			IClass c = o.ClassEntity;
			foreach (IMethod m in c.Methods) {
				if (m.IsPublic && !m.IsConstructor && !m.IsConst && !m.IsStatic) {
					this.possibleInterfaceMembers.Add(m);
					this.selectMembersListBox.Items.Add(FormatMemberForDisplay(m), CheckState.Checked);
				}
			}
			foreach (IProperty p in c.Properties) {
				if (p.IsPublic && !p.IsConst && !p.IsStatic) {
					this.possibleInterfaceMembers.Add(p);
					this.selectMembersListBox.Items.Add(FormatMemberForDisplay(p), CheckState.Checked);
				}
			}
		}
		
		// TODO: i think these really belong in the model (ExtractInterfaceOptions)
		//       rather than the view's code-behind...
		string FormatMemberForDisplay(IMethod m)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} {1} ( ", m.ReturnType.Name, m.Name);
			foreach (IParameter p in m.Parameters) {
				if (p != m.Parameters[0]) {
					sb.Append(" , ");
				}
				sb.AppendFormat("{0} {1}", p.ReturnType.Name, p.Name);
			}
			sb.Append(" );");
			return sb.ToString();
		}
		
		string FormatMemberForDisplay(IProperty p)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("{0} {1} {{ ", p.ReturnType.Name, p.Name);
			if (p.CanGet) sb.Append("get;");
			if (p.CanSet && p.CanSet) sb.Append(" ");
			if (p.CanSet) sb.Append("set;");
			sb.Append(" }");
			return sb.ToString();
		}
		
		#region Event Handlers
		
		//bool insideCheckAll = false;
		
		void BtnSelectAllClick(object sender, EventArgs e)
		{
			//insideCheckAll = true;
			var numItems = selectMembersListBox.Items.Count;
			for(var i = 0; i<numItems; i++) {
				selectMembersListBox.SetItemCheckState(i, CheckState.Checked);
			}
			
			//insideCheckAll = false;
		}
		
		void BtnOKClick(object sender, EventArgs e)
		{
			this.options.ChosenMembers.Clear();
			foreach (int i in selectMembersListBox.CheckedIndices) {
				this.options.ChosenMembers.Add(this.possibleInterfaceMembers[i]);
			}
			if (this.options.ChosenMembers.Count == 0) {
				MessageService.ShowError("Please select at least one member from the list!");
				return;
			}
			this.options.IncludeComments = cbIncludeComments.CheckState == CheckState.Checked;
			this.options.AddInterfaceToClass = cbAddToClass.CheckState == CheckState.Checked;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}
		
		void BtnCancelClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
		
		void BtnDeselectAllClick(object sender, EventArgs e)
		{
			//insideCheckAll = true;
			var numItems = selectMembersListBox.Items.Count;
			for(var i = 0; i<numItems; i++) {
				selectMembersListBox.SetItemCheckState(i, CheckState.Unchecked);
			}
			
			//insideCheckAll = false;
		}
		
		void TxtInterfaceNameTextChanged(object sender, EventArgs e)
		{
			this.options.NewInterfaceName = this.txtInterfaceName.Text;
			this.txtGeneratedName.Text = this.options.FullyQualifiedName;
			if (!hasSetFilenameExplicitly) {
				this.options.NewFileName = this.txtNewFileName.Text = this.options.SuggestedFileName;
			}
		}
		
		void TxtNewFileNameTextChanged(object sender, EventArgs e)
		{
			// TODO: this logic should really be in the model...
			if (hasSetFilenameExplicitly) {
				this.options.NewFileName = this.txtNewFileName.Text;
			}
		}
		
		void TxtNewFileNameKeyDown(object sender, KeyEventArgs e)
		{
			hasSetFilenameExplicitly = true;
		}
		#endregion
	}
}
