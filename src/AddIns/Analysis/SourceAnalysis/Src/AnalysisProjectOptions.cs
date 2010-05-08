// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matt Everson" email="ti.just.me@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace MattEverson.SourceAnalysis
{
	public partial class AnalysisProjectOptions
	{
		public AnalysisProjectOptions()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			enableCheckBox.Text = StringParser.Parse(enableCheckBox.Text);
		}
		
		#region ConfigurationGuiBinding
		public CheckBox EnableCheckBox {
			get {
				return enableCheckBox;
			}
		}
		
		public TextBox SettingsFileTextBox {
		    get {
		        return settingsFileTextBox;
		    }
		}
		
        public System.Windows.Forms.Button BrowseButton {
            get { return browseButton; }
        }
	    
        public System.Windows.Forms.Button ModifyStyleCopSettingsButton {
            get { return modifyStyleCopSettingsButton; }
        }
		
		
		public ConfigurationGuiBinding CreateBinding()
		{
			return new ConfigBinding(this);
		}
		
		class ConfigBinding : ConfigurationGuiBinding
		{
			readonly AnalysisProjectOptions po;
			
			public ConfigBinding(AnalysisProjectOptions po)
			{
				this.po = po;
				this.TreatPropertyValueAsLiteral = false;
				po.OptionChanged += delegate {
					Helper.IsDirty = true;
				};
			}
			
			public override void Load()
			{
				
			}
			
			public override bool Save()
			{
				return true;
			}
		}
		#endregion
		
		public event EventHandler OptionChanged;
		
		protected virtual void OnOptionChanged(EventArgs e)
		{
			if (OptionChanged != null) {
				OptionChanged(this, e);
			}
		}		
		
		void BrowseButtonClick(object sender, EventArgs e)
		{
		    if(openFileDialog1.ShowDialog() == DialogResult.OK)
		    {
		        settingsFileTextBox.Text = openFileDialog1.FileName;
		    }
		}
	}
}
