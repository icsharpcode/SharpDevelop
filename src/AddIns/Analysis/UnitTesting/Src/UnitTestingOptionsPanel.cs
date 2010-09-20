// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Drawing;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestingOptionsPanel : XmlFormsOptionPanel
	{
		UnitTestingOptions options;
		CheckBox labelsCheckBox;
		CheckBox showLogoCheckBox;
		CheckBox showProgressCheckBox;
		CheckBox threadCheckBox;
		CheckBox shadowCopyCheckBox;
		CheckBox createXmlOutputFileCheckBox;
		
		public UnitTestingOptionsPanel() : this(new UnitTestingOptions())
		{
		}
		
		public UnitTestingOptionsPanel(UnitTestingOptions options)
		{
			this.options = options;
		}
		
		public override void LoadPanelContents()
		{
			SetupFromManifestResource("ICSharpCode.UnitTesting.Resources.UnitTestingOptionsPanel.xfrm");
			
			labelsCheckBox = (CheckBox)ControlDictionary["labelsCheckBox"];
			labelsCheckBox.Checked = options.Labels;

			showLogoCheckBox = (CheckBox)ControlDictionary["showLogoCheckBox"];
			showLogoCheckBox.Checked = !options.NoLogo;

			showProgressCheckBox = (CheckBox)ControlDictionary["showProgressCheckBox"];
			showProgressCheckBox.Checked = !options.NoDots;
			
			shadowCopyCheckBox = (CheckBox)ControlDictionary["shadowCopyCheckBox"];
			shadowCopyCheckBox.Checked = !options.NoShadow;

			threadCheckBox = (CheckBox)ControlDictionary["threadCheckBox"];
			threadCheckBox.Checked = !options.NoThread;
		
			createXmlOutputFileCheckBox = (CheckBox)ControlDictionary["createXmlOutputFileCheckBox"];
			createXmlOutputFileCheckBox.Checked = options.CreateXmlOutputFile;
		}

		public override bool StorePanelContents()
		{
			options.Labels = labelsCheckBox.Checked;
			options.NoLogo = !showLogoCheckBox.Checked;
			options.NoDots = !showProgressCheckBox.Checked;
			options.NoShadow = !shadowCopyCheckBox.Checked;
			options.NoThread = !threadCheckBox.Checked;
			options.CreateXmlOutputFile = createXmlOutputFileCheckBox.Checked;
			
			return true;
		}
		
		/// <summary>
		/// Calls SetupFromXmlStream after creating a stream from the current
		/// assembly using the specified manifest resource name.
		/// </summary>
		/// <param name="resource">The manifest resource name used to create the stream.</param>
		protected virtual void SetupFromManifestResource(string resource)
		{
			SetupFromXmlStream(typeof(UnitTestingOptionsPanel).Assembly.GetManifestResourceStream(resource));
		}		
	}
}
