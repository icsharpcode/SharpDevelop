// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

using Plugins.Wizards.MessageBoxBuilder.Generator;

namespace Plugins.Wizards.MessageBoxBuilder.DialogPanels
{
	/// <summary>
	/// Summary description for Form3.
	/// </summary>
	public class CodeGenerationPanel : AbstractWizardPanel
	{
		MessageBoxGenerator generator  = null;
		Properties         customizer = null;
		string              language   = null;
		
		public override object CustomizationObject {
			get {
				return customizer;
			}
			set {
				this.customizer = (Properties)value;
				generator = (MessageBoxGenerator)customizer.Get("Generator");
				language = customizer.Get("Language").ToString();
				generator.Changed += new EventHandler(OnGeneratorChanged);
			}
		}
		
		void OnGeneratorChanged(object sender, EventArgs e)
		{
			ControlDictionary["richTextBox"].Text = generator.GenerateCode(language);
		}
		
		void ChangedEvent(object sender, EventArgs e)
		{
			generator.GenerateReturnValue = ((CheckBox)ControlDictionary["generateReturnValueCheckBox"]).Checked;
			generator.VariableName        = ControlDictionary["variableNameTextBox"].Text;
			generator.GenerateSwitchCase  = ((CheckBox)ControlDictionary["generateSwitchCaseCheckBox"]).Checked;
			
			SetEnabledStatus(((CheckBox)ControlDictionary["generateReturnValueCheckBox"]).Checked, "variableNameTextBox", "generateSwitchCaseCheckBox");
		}

		public CodeGenerationPanel()
		{
			InitializeComponent();
		}
		
		void InitializeComponent()
		{
			SetupFromXmlStream(Assembly.GetCallingAssembly().GetManifestResourceStream("CodeGenerationPanel.xfrm"));
			
			((CheckBox)ControlDictionary["generateReturnValueCheckBox"]).CheckedChanged += new EventHandler(ChangedEvent);
			((CheckBox)ControlDictionary["generateSwitchCaseCheckBox"]).CheckedChanged += new EventHandler(ChangedEvent);
			ControlDictionary["variableNameTextBox"].Text = "result";
			
			((TextBox)ControlDictionary["variableNameTextBox"]).TextChanged     += new EventHandler(ChangedEvent);
			
			ControlDictionary["richTextBox"].Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			
			SetEnabledStatus(false, "variableNameTextBox", "generateSwitchCaseCheckBox");
		}
	}
}
