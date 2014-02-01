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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Globals;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of UserControl1.
	/// </summary>
	public partial class LayoutPanelControl : UserControl
	{
		GlobalEnums.ReportLayout reportLayout;
		AvailableFieldsCollection availableFieldsCollection ;
		
		
		
		public LayoutPanelControl()
		{
			InitializeComponent();
			groupBox1.Text = ResourceService.GetString("SharpReport.Wizard.Layout.ReportLayout");
			radioListLayout.Text = ResourceService.GetString("SharpReport.Wizard.Layout.ListLayout");
			radioTableLayout.Text = ResourceService.GetString("SharpReport.Wizard.Layout.TableLayout");
			groupBox2.Text = ResourceService.GetString("SharpReport.Wizard.Layout.Column");
			checkBox1.Text = ResourceService.GetString("SharpReport.Wizard.Layout.Grouping");
		}
		
		
		
		private void SetLayout ()
		{
			switch (reportLayout) {
				case GlobalEnums.ReportLayout.ListLayout:
					this.radioListLayout.Checked = true;
					break;
				case GlobalEnums.ReportLayout.TableLayout:
					this.radioListLayout.Checked = true;
					break;
			}
		}
		
		
		private void GetLayout()
		{
			if (this.radioListLayout.Checked) {
				this.reportLayout = GlobalEnums.ReportLayout.ListLayout;
			}
			else if (this.radioTableLayout.Checked) {
				this.reportLayout = GlobalEnums.ReportLayout.TableLayout;
			}
		}
		
		
		public GlobalEnums.ReportLayout ReportLayout {
			get {
				GetLayout();
				return reportLayout;
			}
			set { reportLayout = value;
				SetLayout();
			}
		}
		
		void CheckBox1CheckedChanged(object sender, System.EventArgs e)
		{
			//comboBox1.Visible = checkBox1.Checked;
			this.groupBox2.Visible = checkBox1.Checked;
		}
		
		
		public AvailableFieldsCollection AvailableFieldsCollection {
			get { return availableFieldsCollection; }
			
			set {
				availableFieldsCollection = value;
				comboBox1.Items.Clear();
				foreach (AbstractColumn ac in this.availableFieldsCollection)
				{
					this.comboBox1.Items.Add(ac.ColumnName);
				}
				
				if (comboBox1.Items.Count > 0) {
					comboBox1.SelectedIndex = 0;
				}
			}
		}
		
		public string GroupName
		{
			get {
				string ret = String.Empty;
				if (checkBox1.Checked) {
					ret =  comboBox1.SelectedItem.ToString();
				}
				return ret;
			}
			
		}
	}
}
