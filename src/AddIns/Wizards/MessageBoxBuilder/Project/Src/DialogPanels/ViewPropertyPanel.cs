// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.Resources;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

using Plugins.Wizards.MessageBoxBuilder.Generator;

namespace Plugins.Wizards.MessageBoxBuilder.DialogPanels {
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
	public class ViewPropertyPanel : AbstractWizardPanel
	{
		MessageBoxGenerator generator = null;
		Properties customizer        = null;
		
		public override object CustomizationObject {
			get {
				return customizer;
			}
			set {
				this.customizer = (Properties)value;
				generator = (MessageBoxGenerator)customizer.Get("Generator");
			}
		}
		
		void ChangedEvent(object sender, EventArgs e)
		{
			generator.Text    = ControlDictionary["messageTextBox"].Text;
			generator.Caption = ControlDictionary["captionTextBox"].Text;
			generator.MessageBoxButtons       = (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), ControlDictionary["buttonsComboBox"].Text);
			generator.MessageBoxDefaultButton = (MessageBoxDefaultButton)Enum.Parse(typeof(MessageBoxDefaultButton), ControlDictionary["defaultButtonComboBox"].Text);
			
			if (((RadioButton)ControlDictionary["radioButton1"]).Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Error;
			} else if (((RadioButton)ControlDictionary["radioButton2"]).Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Question;
			} else if (((RadioButton)ControlDictionary["radioButton3"]).Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Information;
			} else if (((RadioButton)ControlDictionary["radioButton4"]).Checked) {
				generator.MessageBoxIcon = MessageBoxIcon.Exclamation;
			} else {
				generator.MessageBoxIcon = MessageBoxIcon.None;
			}
		}
		
		void PreviewButton(object sender, EventArgs e)
		{
			generator.PreviewMessageBox();
		}
		
		public ViewPropertyPanel()
		{
			InitializeComponent();
		}
		
		void InitializeComponent()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("VisiblePropertiesPanel.xfrm"));
			((TextBox)ControlDictionary["captionTextBox"]).TextChanged += new EventHandler(ChangedEvent);
			((TextBox)ControlDictionary["messageTextBox"]).TextChanged += new EventHandler(ChangedEvent);
			
			ControlDictionary["previewButton"].Click += new EventHandler(PreviewButton);
			
			((ComboBox)ControlDictionary["buttonsComboBox"]).Items.AddRange(Enum.GetNames(typeof(MessageBoxButtons)));
			((ComboBox)ControlDictionary["buttonsComboBox"]).SelectedIndex = 0;
			((ComboBox)ControlDictionary["buttonsComboBox"]).SelectedIndexChanged += new EventHandler(ChangedEvent);
			
			((ComboBox)ControlDictionary["defaultButtonComboBox"]).Items.AddRange(Enum.GetNames(typeof(MessageBoxDefaultButton)));
			((ComboBox)ControlDictionary["defaultButtonComboBox"]).SelectedIndex = 0;
			((ComboBox)ControlDictionary["defaultButtonComboBox"]).SelectedIndexChanged += new EventHandler(ChangedEvent);
			
			
			((RadioButton)ControlDictionary["radioButton1"]).Image = new Bitmap(SystemIcons.Error.ToBitmap(), 16, 16);
			((RadioButton)ControlDictionary["radioButton1"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["radioButton1"]).CheckedChanged += new EventHandler(ChangedEvent);
			
			((RadioButton)ControlDictionary["radioButton2"]).Image = new Bitmap(SystemIcons.Question.ToBitmap(), 16, 16);
			((RadioButton)ControlDictionary["radioButton2"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["radioButton2"]).CheckedChanged += new EventHandler(ChangedEvent);
			
			((RadioButton)ControlDictionary["radioButton3"]).Image = new Bitmap(SystemIcons.Information.ToBitmap(), 16, 16);
			((RadioButton)ControlDictionary["radioButton3"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["radioButton3"]).CheckedChanged += new EventHandler(ChangedEvent);
			
			((RadioButton)ControlDictionary["radioButton4"]).Image = new Bitmap(SystemIcons.Exclamation.ToBitmap(), 16, 16);
			((RadioButton)ControlDictionary["radioButton4"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["radioButton4"]).CheckedChanged += new EventHandler(ChangedEvent);
			
			((RadioButton)ControlDictionary["radioButton5"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["radioButton5"]).CheckedChanged += new EventHandler(ChangedEvent);
			
		}
	}
}
