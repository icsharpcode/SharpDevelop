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
using System.Windows.Forms;
using ICSharpCode.Core;
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
