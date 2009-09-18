// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.NRefactory.Visitors;

namespace NRefactoryDemo
{
	/// <summary>
	/// Graphical application to demonstrate NRefactory.
	/// </summary>
	public partial class MainForm
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		AstView astView = new AstView();
		
		public MainForm()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
			astView.Dock = DockStyle.Fill;
			astPanel.Controls.Add(astView);
			ParseCSharpButtonClick(null, null);
			transformationComboBox.SelectedIndex = 0;
		}
		
		void ClearSpecialsButtonClick(object sender, EventArgs e)
		{
			SetSpecials(new ISpecial[0]);
		}
		
		IList<ISpecial> savedSpecialsList;
		
		void SetSpecials(IList<ISpecial> specialsList)
		{
			savedSpecialsList = specialsList;
			if (specialsList.Count == 1)
				specialsLabel.Text = "1 special saved";
			else
				specialsLabel.Text = specialsList.Count + " specials saved";
		}
		
		void ArrowDownPictureBoxPaint(object sender, PaintEventArgs e)
		{
			Size size = arrowDownPictureBox.Size;
			e.Graphics.DrawLine(Pens.Black, size.Width / 2, 0, size.Width / 2, size.Height);
			e.Graphics.DrawLine(Pens.Black, 0, size.Height - size.Width / 2, size.Width / 2, size.Height);
			e.Graphics.DrawLine(Pens.Black, size.Width - 1, size.Height - size.Width / 2, size.Width / 2, size.Height);
		}
		
		void ArrowUpPictureBoxPaint(object sender, PaintEventArgs e)
		{
			Size size = arrowUpPictureBox.Size;
			e.Graphics.DrawLine(Pens.Black, size.Width / 2, 0, size.Width / 2, size.Height);
			e.Graphics.DrawLine(Pens.Black, 0, size.Width / 2, size.Width / 2, 0);
			e.Graphics.DrawLine(Pens.Black, size.Width - 1, size.Width / 2, size.Width / 2, 0);
		}
		
		void ParseCSharpButtonClick(object sender, EventArgs e)
		{
			Parse(SupportedLanguage.CSharp, codeTextBox.Text);
		}
		
		void ParseVBButtonClick(object sender, EventArgs e)
		{
			Parse(SupportedLanguage.VBNet, codeTextBox.Text);
		}
		
		void Parse(SupportedLanguage language, string text)
		{
			using (IParser parser = ParserFactory.CreateParser(language, new StringReader(text))) {
				parser.Parse();
				// this allows retrieving comments, preprocessor directives, etc. (stuff that isn't part of the syntax)
				SetSpecials(parser.Lexer.SpecialTracker.RetrieveSpecials());
				// this retrieves the root node of the result AST
				astView.Unit = parser.CompilationUnit;
				if (parser.Errors.Count > 0) {
					MessageBox.Show(parser.Errors.ErrorOutput, "Parse errors");
				}
			}
		}
		
		void GenerateCSharpButtonClick(object sender, EventArgs e)
		{
			GenerateCode(new CSharpOutputVisitor());
		}
		
		void GenerateVBButtonClick(object sender, EventArgs e)
		{
			GenerateCode(new VBNetOutputVisitor());
		}
		
		void GenerateCode(IOutputAstVisitor outputVisitor)
		{
			// re-insert the comments we saved from the parser into the output
			using (SpecialNodesInserter.Install(savedSpecialsList, outputVisitor)) {
				astView.Unit.AcceptVisitor(outputVisitor, null);
			}
			codeTextBox.Text = outputVisitor.Text.Replace("\t", "  ");
		}
		
		void DeleteSelectedNodeClick(object sender, EventArgs e)
		{
			astView.DeleteSelectedNode();
		}
		
		void ApplyTransformationClick(object sender, EventArgs e)
		{
			try {
				string typeName = typeof(ToCSharpConvertVisitor).Namespace + "." + transformationComboBox.SelectedItem.ToString();
				Type type = typeof(ToCSharpConvertVisitor).Assembly.GetType(typeName);
				astView.ApplyTransformation((IAstVisitor)Activator.CreateInstance(type));
			} catch (Exception ex) {
				MessageBox.Show(ex.ToString(), "Error");
				astView.Unit = astView.Unit; // complete refresh
			}
		}
		
		void EditNodeButtonClick(object sender, EventArgs e)
		{
			astView.EditSelectedNode();
		}
	}
}
