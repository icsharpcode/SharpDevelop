// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.UnitTesting
{
	public class UnitTestingOptionsPanel : AbstractOptionPanel
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
