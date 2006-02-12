/*
 * Created by SharpDevelop.
 * User: Forstmeier Helmut
 * Date: 08.02.2006
 * Time: 15:24
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace ReportSamples
{
	partial class MainForm : System.Windows.Forms.Form
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.formSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.simpleFormsSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pullMpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.employeeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.missingConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pushModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.emlpoyeesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.formSheetToolStripMenuItem,
									this.pullMpToolStripMenuItem,
									this.pushModelToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(518, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// formSheetToolStripMenuItem
			// 
			this.formSheetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.simpleFormsSheetToolStripMenuItem});
			this.formSheetToolStripMenuItem.Name = "formSheetToolStripMenuItem";
			this.formSheetToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
			this.formSheetToolStripMenuItem.Text = "FormSheet";
			// 
			// simpleFormsSheetToolStripMenuItem
			// 
			this.simpleFormsSheetToolStripMenuItem.Name = "simpleFormsSheetToolStripMenuItem";
			this.simpleFormsSheetToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.simpleFormsSheetToolStripMenuItem.Text = "SimpleFormsSheet";
			this.simpleFormsSheetToolStripMenuItem.Click += new System.EventHandler(this.SimpleFormsSheetClick);
			// 
			// pullMpToolStripMenuItem
			// 
			this.pullMpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.customersToolStripMenuItem,
									this.employeeToolStripMenuItem,
									this.missingConnectionToolStripMenuItem});
			this.pullMpToolStripMenuItem.Name = "pullMpToolStripMenuItem";
			this.pullMpToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
			this.pullMpToolStripMenuItem.Text = "PullModell";
			// 
			// customersToolStripMenuItem
			// 
			this.customersToolStripMenuItem.Name = "customersToolStripMenuItem";
			this.customersToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.customersToolStripMenuItem.Text = "Customers";
			this.customersToolStripMenuItem.Click += new System.EventHandler(this.CustomersClick);
			// 
			// employeeToolStripMenuItem
			// 
			this.employeeToolStripMenuItem.Name = "employeeToolStripMenuItem";
			this.employeeToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.employeeToolStripMenuItem.Text = "Employee";
			this.employeeToolStripMenuItem.Click += new System.EventHandler(this.EmployeeClick);
			// 
			// missingConnectionToolStripMenuItem
			// 
			this.missingConnectionToolStripMenuItem.Name = "missingConnectionToolStripMenuItem";
			this.missingConnectionToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.missingConnectionToolStripMenuItem.Text = "MissingConnection";
			this.missingConnectionToolStripMenuItem.Click += new System.EventHandler(this.MissingConnectionClick);
			// 
			// pushModelToolStripMenuItem
			// 
			this.pushModelToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.emlpoyeesToolStripMenuItem});
			this.pushModelToolStripMenuItem.Name = "pushModelToolStripMenuItem";
			this.pushModelToolStripMenuItem.Size = new System.Drawing.Size(70, 20);
			this.pushModelToolStripMenuItem.Text = "PushModel";
			// 
			// emlpoyeesToolStripMenuItem
			// 
			this.emlpoyeesToolStripMenuItem.Name = "emlpoyeesToolStripMenuItem";
			this.emlpoyeesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
			this.emlpoyeesToolStripMenuItem.Text = "Employees-Push";
			this.emlpoyeesToolStripMenuItem.Click += new System.EventHandler(this.EmployeesPushClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(518, 273);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "ReportSamples";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem emlpoyeesToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem missingConnectionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem employeeToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem customersToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem simpleFormsSheetToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pushModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pullMpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem formSheetToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
	}
}
