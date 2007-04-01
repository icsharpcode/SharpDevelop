// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Dickon Field" email=""/>
//     <version>$Revision$</version>
// </file>

namespace SharpDbTools.Forms
{
	partial class ConnectionStringDefinitionDialog : System.Windows.Forms.Form
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
			this.connStringPropertyGrid = new System.Windows.Forms.PropertyGrid();
			this.buttonsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.testButton = new System.Windows.Forms.Button();
			this.submitButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.providerTypeComboBox = new System.Windows.Forms.ComboBox();
			this.dataSourceTypeLabel = new System.Windows.Forms.Label();
			this.connStringResult = new System.Windows.Forms.TextBox();
			this.connectionStringLabel = new System.Windows.Forms.Label();
			this.progressTimer = new System.Windows.Forms.Timer(this.components);
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.outputMessageTabControl = new System.Windows.Forms.TabControl();
			this.connectionStringTab = new System.Windows.Forms.TabPage();
			this.testResultTab = new System.Windows.Forms.TabPage();
			this.testResultTextBox = new System.Windows.Forms.TextBox();
			this.buttonsFlowLayoutPanel.SuspendLayout();
			this.outputMessageTabControl.SuspendLayout();
			this.connectionStringTab.SuspendLayout();
			this.testResultTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// connStringPropertyGrid
			// 
			this.connStringPropertyGrid.Location = new System.Drawing.Point(0, 39);
			this.connStringPropertyGrid.Name = "connStringPropertyGrid";
			this.connStringPropertyGrid.Size = new System.Drawing.Size(547, 300);
			this.connStringPropertyGrid.TabIndex = 0;
			// 
			// buttonsFlowLayoutPanel
			// 
			this.buttonsFlowLayoutPanel.Controls.Add(this.testButton);
			this.buttonsFlowLayoutPanel.Controls.Add(this.submitButton);
			this.buttonsFlowLayoutPanel.Controls.Add(this.cancelButton);
			this.buttonsFlowLayoutPanel.Location = new System.Drawing.Point(3, 447);
			this.buttonsFlowLayoutPanel.Name = "buttonsFlowLayoutPanel";
			this.buttonsFlowLayoutPanel.Size = new System.Drawing.Size(312, 34);
			this.buttonsFlowLayoutPanel.TabIndex = 1;
			// 
			// testButton
			// 
			this.testButton.Location = new System.Drawing.Point(3, 3);
			this.testButton.Name = "testButton";
			this.testButton.Size = new System.Drawing.Size(75, 23);
			this.testButton.TabIndex = 0;
			this.testButton.Text = "Test";
			this.testButton.UseVisualStyleBackColor = true;
			this.testButton.Click += new System.EventHandler(this.TestButtonClick);
			// 
			// submitButton
			// 
			this.submitButton.Location = new System.Drawing.Point(84, 3);
			this.submitButton.Name = "submitButton";
			this.submitButton.Size = new System.Drawing.Size(75, 23);
			this.submitButton.TabIndex = 1;
			this.submitButton.Text = "Submit";
			this.submitButton.UseVisualStyleBackColor = true;
			this.submitButton.Click += new System.EventHandler(this.SubmitButtonClick);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(165, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// providerTypeComboBox
			// 
			this.providerTypeComboBox.FormattingEnabled = true;
			this.providerTypeComboBox.Location = new System.Drawing.Point(117, 12);
			this.providerTypeComboBox.Name = "providerTypeComboBox";
			this.providerTypeComboBox.Size = new System.Drawing.Size(195, 21);
			this.providerTypeComboBox.TabIndex = 2;
			this.providerTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ProviderTypeSelectedIndexChanged);
			// 
			// dataSourceTypeLabel
			// 
			this.dataSourceTypeLabel.Location = new System.Drawing.Point(3, 9);
			this.dataSourceTypeLabel.Name = "dataSourceTypeLabel";
			this.dataSourceTypeLabel.Size = new System.Drawing.Size(108, 23);
			this.dataSourceTypeLabel.TabIndex = 3;
			this.dataSourceTypeLabel.Text = "Data Source Type:";
			this.dataSourceTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// connStringResult
			// 
			this.connStringResult.Enabled = false;
			this.connStringResult.Location = new System.Drawing.Point(0, 0);
			this.connStringResult.Multiline = true;
			this.connStringResult.Name = "connStringResult";
			this.connStringResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.connStringResult.Size = new System.Drawing.Size(413, 74);
			this.connStringResult.TabIndex = 4;
			// 
			// connectionStringLabel
			// 
			this.connectionStringLabel.Location = new System.Drawing.Point(12, 348);
			this.connectionStringLabel.Name = "connectionStringLabel";
			this.connectionStringLabel.Size = new System.Drawing.Size(100, 23);
			this.connectionStringLabel.TabIndex = 5;
			this.connectionStringLabel.Text = "Connection String:";
			// 
			// progressTimer
			// 
			this.progressTimer.Interval = 1000;
			this.progressTimer.Tick += new System.EventHandler(this.ProgressTimerTick);
			// 
			// statusStrip
			// 
			this.statusStrip.Location = new System.Drawing.Point(0, 478);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(547, 22);
			this.statusStrip.TabIndex = 6;
			this.statusStrip.Text = "statusStrip1";
			// 
			// testResultTab
			// 
			this.outputMessageTabControl.Controls.Add(this.connectionStringTab);
			this.outputMessageTabControl.Controls.Add(this.testResultTab);
			this.outputMessageTabControl.Location = new System.Drawing.Point(118, 345);
			this.outputMessageTabControl.Name = "testResultTab";
			this.outputMessageTabControl.SelectedIndex = 0;
			this.outputMessageTabControl.Size = new System.Drawing.Size(417, 100);
			this.outputMessageTabControl.TabIndex = 7;
			// 
			// tabPage1
			// 
			this.connectionStringTab.Controls.Add(this.connStringResult);
			this.connectionStringTab.Location = new System.Drawing.Point(4, 22);
			this.connectionStringTab.Name = "tabPage1";
			this.connectionStringTab.Padding = new System.Windows.Forms.Padding(3);
			this.connectionStringTab.Size = new System.Drawing.Size(409, 74);
			this.connectionStringTab.TabIndex = 0;
			this.connectionStringTab.Text = "Connection String";
			this.connectionStringTab.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.testResultTab.Controls.Add(this.testResultTextBox);
			this.testResultTab.Location = new System.Drawing.Point(4, 22);
			this.testResultTab.Name = "tabPage2";
			this.testResultTab.Padding = new System.Windows.Forms.Padding(3);
			this.testResultTab.Size = new System.Drawing.Size(409, 74);
			this.testResultTab.TabIndex = 1;
			this.testResultTab.Text = "Test Result Message";
			this.testResultTab.UseVisualStyleBackColor = true;
			// 
			// testResultTextBox
			// 
			this.testResultTextBox.Location = new System.Drawing.Point(-5, 0);
			this.testResultTextBox.Multiline = true;
			this.testResultTextBox.Name = "testResultTextBox";
			this.testResultTextBox.Size = new System.Drawing.Size(418, 77);
			this.testResultTextBox.TabIndex = 0;
			// 
			// ConnectionStringDefinitionDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(547, 500);
			this.Controls.Add(this.outputMessageTabControl);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.connectionStringLabel);
			this.Controls.Add(this.dataSourceTypeLabel);
			this.Controls.Add(this.providerTypeComboBox);
			this.Controls.Add(this.buttonsFlowLayoutPanel);
			this.Controls.Add(this.connStringPropertyGrid);
			this.Name = "ConnectionStringDefinitionDialog";
			this.Text = "Set up Connection String";
			this.buttonsFlowLayoutPanel.ResumeLayout(false);
			this.outputMessageTabControl.ResumeLayout(false);
			this.connectionStringTab.ResumeLayout(false);
			this.connectionStringTab.PerformLayout();
			this.testResultTab.ResumeLayout(false);
			this.testResultTab.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.TabControl outputMessageTabControl;
		private System.Windows.Forms.TextBox testResultTextBox;
		private System.Windows.Forms.TabPage testResultTab;
		private System.Windows.Forms.TabPage connectionStringTab;
		
		private System.Windows.Forms.Timer progressTimer;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.TextBox connStringResult;
		private System.Windows.Forms.Label connectionStringLabel;
		private System.Windows.Forms.Label dataSourceTypeLabel;
		private System.Windows.Forms.ComboBox providerTypeComboBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button submitButton;
		private System.Windows.Forms.Button testButton;
		private System.Windows.Forms.FlowLayoutPanel buttonsFlowLayoutPanel;
		private System.Windows.Forms.PropertyGrid connStringPropertyGrid;		
	}
}
