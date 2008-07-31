// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	/// <summary>
	/// Options panel for localization model options.
	/// </summary>
	public class LocalizationModelOptionsPanel : AbstractOptionPanel
	{
		public LocalizationModelOptionsPanel()
		{
		}
		
		#region Windows Forms Designer code
		
		void InitializeComponent()
		{
			System.Windows.Forms.GroupBox modelGroupBox;
			this.assignmentRadioButton = new System.Windows.Forms.RadioButton();
			this.reflectionRadioButton = new System.Windows.Forms.RadioButton();
			this.keepModelCheckBox = new System.Windows.Forms.CheckBox();
			modelGroupBox = new System.Windows.Forms.GroupBox();
			modelGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// modelGroupBox
			// 
			modelGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			modelGroupBox.Controls.Add(this.assignmentRadioButton);
			modelGroupBox.Controls.Add(this.reflectionRadioButton);
			modelGroupBox.Location = new System.Drawing.Point(3, 3);
			modelGroupBox.Name = "modelGroupBox";
			modelGroupBox.Size = new System.Drawing.Size(385, 166);
			modelGroupBox.TabIndex = 0;
			modelGroupBox.TabStop = false;
			modelGroupBox.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
			"ionsPanel.DefaultLocalizationModel}";
			// 
			// assignmentRadioButton
			// 
			this.assignmentRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.assignmentRadioButton.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.assignmentRadioButton.Location = new System.Drawing.Point(6, 91);
			this.assignmentRadioButton.Name = "assignmentRadioButton";
			this.assignmentRadioButton.Size = new System.Drawing.Size(373, 69);
			this.assignmentRadioButton.TabIndex = 1;
			this.assignmentRadioButton.TabStop = true;
			this.assignmentRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
			"ionsPanel.AssignmentRadioButton}";
			this.assignmentRadioButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.assignmentRadioButton.UseVisualStyleBackColor = true;
			// 
			// reflectionRadioButton
			// 
			this.reflectionRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.reflectionRadioButton.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.reflectionRadioButton.Location = new System.Drawing.Point(6, 19);
			this.reflectionRadioButton.Name = "reflectionRadioButton";
			this.reflectionRadioButton.Size = new System.Drawing.Size(373, 66);
			this.reflectionRadioButton.TabIndex = 0;
			this.reflectionRadioButton.TabStop = true;
			this.reflectionRadioButton.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
			"ionsPanel.ReflectionRadioButton}";
			this.reflectionRadioButton.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.reflectionRadioButton.UseVisualStyleBackColor = true;
			// 
			// keepModelCheckBox
			// 
			this.keepModelCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.keepModelCheckBox.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.keepModelCheckBox.Location = new System.Drawing.Point(9, 175);
			this.keepModelCheckBox.Name = "keepModelCheckBox";
			this.keepModelCheckBox.Size = new System.Drawing.Size(373, 95);
			this.keepModelCheckBox.TabIndex = 1;
			this.keepModelCheckBox.Text = "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.OptionPanels.LocalizationModelOpt" +
			"ionsPanel.KeepModelCheckBox}";
			this.keepModelCheckBox.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.keepModelCheckBox.UseVisualStyleBackColor = true;
			// 
			// LocalizationModelOptionsPanel
			// 
			this.Controls.Add(this.keepModelCheckBox);
			this.Controls.Add(modelGroupBox);
			this.Name = "LocalizationModelOptionsPanel";
			this.Size = new System.Drawing.Size(391, 300);
			modelGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		private System.Windows.Forms.CheckBox keepModelCheckBox;
		private System.Windows.Forms.RadioButton reflectionRadioButton;
		private System.Windows.Forms.RadioButton assignmentRadioButton;
		
		#endregion
		
		public override void LoadPanelContents()
		{
			base.LoadPanelContents();
			if (this.Controls.Count == 0) {
				this.InitializeComponent();
				Translate(this);
			}
			
			this.reflectionRadioButton.Checked = (DefaultLocalizationModel == CodeDomLocalizationModel.PropertyReflection);
			this.assignmentRadioButton.Checked = !this.reflectionRadioButton.Checked;
			this.keepModelCheckBox.Checked = KeepLocalizationModel;
		}
		
		static void Translate(Control container) {
			container.Text = StringParser.Parse(container.Text);
			foreach (Control c in container.Controls) {
				Translate(c);
			}
		}
		
		public override bool StorePanelContents()
		{
			if (this.reflectionRadioButton.Checked) {
				DefaultLocalizationModel = CodeDomLocalizationModel.PropertyReflection;
			} else if (this.assignmentRadioButton.Checked) {
				DefaultLocalizationModel = CodeDomLocalizationModel.PropertyAssignment;
			} else {
				MessageService.ShowError("One localization model must be selected!");
				return false;
			}
			
			KeepLocalizationModel = this.keepModelCheckBox.Checked;
			
			return true;
		}
		
		
		public const string DefaultLocalizationModelPropertyName = "FormsDesigner.DesignerOptions.DefaultLocalizationModel";
		public const string KeepLocalizationModelPropertyName = "FormsDesigner.DesignerOptions.KeepLocalizationModel";
		
		const CodeDomLocalizationModel DefaultLocalizationModelDefaultValue = CodeDomLocalizationModel.PropertyReflection;
		const bool KeepLocalizationModelDefaultValue = false;
		
		/// <summary>
		/// Gets or sets the default localization model to be used by the Windows Forms designer.
		/// </summary>
		public static CodeDomLocalizationModel DefaultLocalizationModel {
			get { return GetPropertySafe(DefaultLocalizationModelPropertyName, DefaultLocalizationModelDefaultValue); }
			set { PropertyService.Set(DefaultLocalizationModelPropertyName, value); }
		}
		
		/// <summary>
		/// Gets or sets whether the Windows Forms designer should keep the localization model of existing files.
		/// </summary>
		public static bool KeepLocalizationModel {
			get { return GetPropertySafe(KeepLocalizationModelPropertyName, KeepLocalizationModelDefaultValue); }
			set { PropertyService.Set(KeepLocalizationModelPropertyName, value); }
		}
		
		static T GetPropertySafe<T>(string name, T defaultValue)
		{
			if (PropertyService.Initialized) {
				return PropertyService.Get<T>(name, defaultValue);
			} else {
				return defaultValue;
			}
		}
	}
}
