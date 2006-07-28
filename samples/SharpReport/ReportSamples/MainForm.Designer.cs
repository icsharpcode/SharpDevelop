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
			this.components = new System.ComponentModel.Container();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.formSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.simpleFormsSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unboundFormSheetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pullMpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.customersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.employeeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.missingConnectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pushModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.emlpoyeesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unboundToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unboundToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.unboundPullModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.multipageUnboundPullModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unboundPuskModelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.listDatasourceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.simpleListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eventLoggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.menuStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.formSheetToolStripMenuItem,
									this.pullMpToolStripMenuItem,
									this.pushModelToolStripMenuItem,
									this.unboundToolStripMenuItem,
									this.listDatasourceToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(518, 24);
			this.menuStrip1.TabIndex = 0;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// formSheetToolStripMenuItem
			// 
			this.formSheetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.simpleFormsSheetToolStripMenuItem,
									this.unboundFormSheetToolStripMenuItem});
			this.formSheetToolStripMenuItem.Name = "formSheetToolStripMenuItem";
			this.formSheetToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
			this.formSheetToolStripMenuItem.Text = "FormSheet";
			// 
			// simpleFormsSheetToolStripMenuItem
			// 
			this.simpleFormsSheetToolStripMenuItem.Name = "simpleFormsSheetToolStripMenuItem";
			this.simpleFormsSheetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.simpleFormsSheetToolStripMenuItem.Text = "SimpleFormsSheet";
			this.simpleFormsSheetToolStripMenuItem.Click += new System.EventHandler(this.SimpleFormsSheetClick);
			// 
			// unboundFormSheetToolStripMenuItem
			// 
			this.unboundFormSheetToolStripMenuItem.Name = "unboundFormSheetToolStripMenuItem";
			this.unboundFormSheetToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.unboundFormSheetToolStripMenuItem.Text = "UnboundFormSheet";
			this.unboundFormSheetToolStripMenuItem.Click += new System.EventHandler(this.UnboundFormSheetToolStripMenuItemClick);
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
			// unboundToolStripMenuItem
			// 
			this.unboundToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.unboundToolStripMenuItem1,
									this.unboundPullModelToolStripMenuItem,
									this.multipageUnboundPullModelToolStripMenuItem,
									this.unboundPuskModelToolStripMenuItem});
			this.unboundToolStripMenuItem.Name = "unboundToolStripMenuItem";
			this.unboundToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
			this.unboundToolStripMenuItem.Text = "Unbound";
			// 
			// unboundToolStripMenuItem1
			// 
			this.unboundToolStripMenuItem1.Name = "unboundToolStripMenuItem1";
			this.unboundToolStripMenuItem1.Size = new System.Drawing.Size(218, 22);
			this.unboundToolStripMenuItem1.Text = "Unbound";
			this.unboundToolStripMenuItem1.Click += new System.EventHandler(this.UnboundToolStripMenuItem1Click);
			// 
			// unboundPullModelToolStripMenuItem
			// 
			this.unboundPullModelToolStripMenuItem.Name = "unboundPullModelToolStripMenuItem";
			this.unboundPullModelToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			this.unboundPullModelToolStripMenuItem.Text = "UnboundPullModel";
			this.unboundPullModelToolStripMenuItem.Click += new System.EventHandler(this.UnboundPullModelToolStripMenuItemClick);
			// 
			// multipageUnboundPullModelToolStripMenuItem
			// 
			this.multipageUnboundPullModelToolStripMenuItem.Name = "multipageUnboundPullModelToolStripMenuItem";
			this.multipageUnboundPullModelToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			this.multipageUnboundPullModelToolStripMenuItem.Text = "MultipageUnboundPullModel";
			this.multipageUnboundPullModelToolStripMenuItem.Click += new System.EventHandler(this.MultipageUnboundPullModelToolStripMenuItemClick);
			// 
			// unboundPuskModelToolStripMenuItem
			// 
			this.unboundPuskModelToolStripMenuItem.Name = "unboundPuskModelToolStripMenuItem";
			this.unboundPuskModelToolStripMenuItem.Size = new System.Drawing.Size(218, 22);
			this.unboundPuskModelToolStripMenuItem.Text = "UnboundPushModel";
			this.unboundPuskModelToolStripMenuItem.Click += new System.EventHandler(this.UnboundPushModelToolStripMenuItemClick);
			// 
			// listDatasourceToolStripMenuItem
			// 
			this.listDatasourceToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.simpleListToolStripMenuItem,
									this.eventLoggerToolStripMenuItem});
			this.listDatasourceToolStripMenuItem.Name = "listDatasourceToolStripMenuItem";
			this.listDatasourceToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
			this.listDatasourceToolStripMenuItem.Text = "ListDatasource";
			// 
			// simpleListToolStripMenuItem
			// 
			this.simpleListToolStripMenuItem.Name = "simpleListToolStripMenuItem";
			this.simpleListToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			this.simpleListToolStripMenuItem.Text = "ContributersList";
			this.simpleListToolStripMenuItem.Click += new System.EventHandler(this.ListDatasourceToolStripMenuItemClick);
			// 
			// eventLoggerToolStripMenuItem
			// 
			this.eventLoggerToolStripMenuItem.Name = "eventLoggerToolStripMenuItem";
			this.eventLoggerToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
			this.eventLoggerToolStripMenuItem.Text = "EventLogger";
			this.eventLoggerToolStripMenuItem.Click += new System.EventHandler(this.EventLoggerToolStripMenuItemClick);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
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
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem eventLoggerToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem simpleListToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem listDatasourceToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unboundFormSheetToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unboundPuskModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem multipageUnboundPullModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unboundPullModelToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem unboundToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem unboundToolStripMenuItem;
		private System.Windows.Forms.ErrorProvider errorProvider1;
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
