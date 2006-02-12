using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.NRefactory.PrettyPrinter;

namespace NRefactoryExample
{
	public class MainForm : System.Windows.Forms.Form
	{
		public MainForm()
		{
			// The InitializeComponent() call is required for Windows Forms designer support.
			InitializeComponent();
		}
		
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.Run(new MainForm());
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.inputTextBox = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.runButton = new System.Windows.Forms.Button();
			this.outputTextBox = new System.Windows.Forms.TextBox();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.inputTextBox);
			this.splitContainer1.Panel1.Controls.Add(this.panel1);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.outputTextBox);
			this.splitContainer1.Size = new System.Drawing.Size(804, 422);
			this.splitContainer1.SplitterDistance = 394;
			this.splitContainer1.TabIndex = 0;
			// 
			// inputTextBox
			// 
			this.inputTextBox.AcceptsReturn = true;
			this.inputTextBox.AcceptsTab = true;
			this.inputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.inputTextBox.Location = new System.Drawing.Point(0, 0);
			this.inputTextBox.Multiline = true;
			this.inputTextBox.Name = "inputTextBox";
			this.inputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.inputTextBox.Size = new System.Drawing.Size(394, 372);
			this.inputTextBox.TabIndex = 1;
			this.inputTextBox.Text = resources.GetString("inputTextBox.Text");
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.runButton);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 372);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(394, 50);
			this.panel1.TabIndex = 0;
			// 
			// runButton
			// 
			this.runButton.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.runButton.Location = new System.Drawing.Point(128, 15);
			this.runButton.Name = "runButton";
			this.runButton.Size = new System.Drawing.Size(131, 23);
			this.runButton.TabIndex = 0;
			this.runButton.Text = "Run";
			this.runButton.UseCompatibleTextRendering = true;
			this.runButton.UseVisualStyleBackColor = true;
			this.runButton.Click += new System.EventHandler(this.RunButtonClick);
			// 
			// outputTextBox
			// 
			this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.outputTextBox.Location = new System.Drawing.Point(0, 0);
			this.outputTextBox.Multiline = true;
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.outputTextBox.Size = new System.Drawing.Size(406, 422);
			this.outputTextBox.TabIndex = 0;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(804, 422);
			this.Controls.Add(this.splitContainer1);
			this.Name = "MainForm";
			this.Text = "NRefactory Example";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.Button runButton;
		private System.Windows.Forms.TextBox outputTextBox;
		private System.Windows.Forms.TextBox inputTextBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.SplitContainer splitContainer1;
		#endregion
		
		void RunButtonClick(object sender, EventArgs e)
		{
			StringReader input = new StringReader(inputTextBox.Text);
			IParser parser = ParserFactory.CreateParser(SupportedLanguage.CSharp, input);
			parser.Parse();
			if (parser.Errors.count > 0) {
				outputTextBox.Text = parser.Errors.ErrorOutput;
				return;
			}
			CompilationUnit cu = parser.CompilationUnit;
			
			cu.AcceptVisitor(new WrapperGeneratorVisitor(), null);
			
			IOutputASTVisitor output = new CSharpOutputVisitor(); //new VBNetOutputVisitor();
			cu.AcceptVisitor(output, null);
			outputTextBox.Text = output.Text;
		}
	}
}
