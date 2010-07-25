// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NoGoop.ObjBrowser;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class GeneralOptionsPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.ComponentInspector.AddIn.Resources.GeneralOptionsPanel.xfrm"));
		
			// ActiveX/COM options
			GetRunningComObjectsAtStartupCheckBox.Checked = ComponentInspectorProperties.AddRunningComObjects;
			AutomaticallyGetComPropertiesCheckBox.Checked = ComponentInspectorProperties.AutoInvokeProperties;
			
			// Panel options.
			ShowAssemblyPanelCheckBox.Checked = ComponentInspectorProperties.ShowAssemblyPanel;
			ShowControlsPanelCheckBox.Checked = ComponentInspectorProperties.ShowControlPanel;
			ShowGacPanelCheckBox.Checked = ComponentInspectorProperties.ShowGacPanel;
		}

		public override bool StorePanelContents()
		{
			// ActiveX/COM options.
			ComponentInspectorProperties.AddRunningComObjects = GetRunningComObjectsAtStartupCheckBox.Checked;
			ComponentInspectorProperties.AutoInvokeProperties = AutomaticallyGetComPropertiesCheckBox.Checked;			
			
			// Panel options.
			ComponentInspectorProperties.ShowAssemblyPanel = ShowAssemblyPanelCheckBox.Checked;
			ComponentInspectorProperties.ShowControlPanel = ShowControlsPanelCheckBox.Checked;
			ComponentInspectorProperties.ShowGacPanel = ShowGacPanelCheckBox.Checked;
			
			return true;
		}
		
		CheckBox GetRunningComObjectsAtStartupCheckBox {
			get {
				return Get<CheckBox>("getRunningComObjectsAtStartup");
			}
		}
		
		CheckBox AutomaticallyGetComPropertiesCheckBox {
			get {
				return Get<CheckBox>("automaticallyGetComProperties");
			}
		}
		
		CheckBox ShowAssemblyPanelCheckBox {
			get {
				return Get<CheckBox>("showAssemblyPanel");
			}
		}
		
		CheckBox ShowControlsPanelCheckBox {
			get {
				return Get<CheckBox>("showControlsPanel");
			}
		}
		
		CheckBox ShowGacPanelCheckBox {
			get {
				return Get<CheckBox>("showGacPanel");
			}
		}
		
		
	}
}
