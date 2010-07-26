/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 03.10.2008
 * Zeit: 17:52
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Reports.Core;

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
			comboBox1.Visible = checkBox1.Checked;
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
				return comboBox1.SelectedItem.ToString();
			}
		}
	}
}
