// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.Refactoring;

namespace SharpRefactoring.Forms
{
	/// <summary>
	/// Description of ExtractMethodForm.
	/// </summary>
	public partial class ExtractMethodForm : Form
	{
		Func<IOutputAstVisitor> generator;
		MethodDeclaration declaration;
		BlockStatement body;
		bool cancelUnload = false;
		
		public ExtractMethodForm(MethodDeclaration declaration, Func<IOutputAstVisitor> generator)
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			SetTranslation(this);
			
			this.declaration = declaration;
			this.generator = generator;
			IOutputAstVisitor visitor = this.generator.Invoke();
			body = declaration.Body;
			declaration.Body = new BlockStatement();
			
			declaration.AcceptVisitor(visitor, null);
			
			this.txtName.Text = this.declaration.Name;
			this.txtPreview.Text = visitor.Text;
			
			this.txtName.SelectAll();
		}
		
		void SetTranslation(Control c)
		{
			c.Text = StringParser.Parse(c.Text);
			foreach (Control ctrl in c.Controls)
				SetTranslation(ctrl);
		}

		void btnOKClick(object sender, EventArgs e)
		{
			if (FindReferencesAndRenameHelper.CheckName(this.txtName.Text, string.Empty)) {
				this.Text = this.txtName.Text;
				this.DialogResult = DialogResult.OK;
				this.declaration.Body = body;
				cancelUnload = false;
			} else
				cancelUnload = true;
		}

		void btnCancelClick(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
		}

		void txtNameTextChanged(object sender, EventArgs e)
		{
			declaration.Name = this.txtName.Text;
			IOutputAstVisitor visitor = this.generator.Invoke();
			declaration.AcceptVisitor(visitor, null);
			this.txtPreview.Text = visitor.Text;
		}
		
		void ExtractMethodFormFormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = cancelUnload;
		}
	}
}
