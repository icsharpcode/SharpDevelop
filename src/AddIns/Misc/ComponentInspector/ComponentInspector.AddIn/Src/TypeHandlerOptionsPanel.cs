// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;
using NoGoop.ObjBrowser;

namespace ICSharpCode.ComponentInspector.AddIn
{
	public class TypeHandlerOptionsPanel : XmlFormsOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream("ICSharpCode.ComponentInspector.AddIn.Resources.TypeHandlerOptionsPanel.xfrm"));
						
			ICollection typeHandlers = TypeHandlerManager.Instance.GetTypeHandlers();
			foreach (TypeHandlerManager.TypeHandlerInfo th in typeHandlers) {
				if (th.HandledType == typeof(IEnumerator)) {
					EnumeratorCheckBox.Checked = th.Enabled;
				} else if (th.HandledType == typeof(IList)) {
					ListCheckBox.Checked = th.Enabled;
				} else if (th.HandledType == typeof(EventHandlerList)) {
					EventHandlerListCheckBox.Checked = th.Enabled;
				}
			}
		}

		public override bool StorePanelContents()
		{
			ICollection typeHandlers = TypeHandlerManager.Instance.GetTypeHandlers();
			foreach (TypeHandlerManager.TypeHandlerInfo th in typeHandlers) {
				if (th.HandledType == typeof(IEnumerator)) {
					th.Enabled = EnumeratorCheckBox.Checked;
				} else if (th.HandledType == typeof(IList)) {
					th.Enabled = ListCheckBox.Checked ;
				} else if (th.HandledType == typeof(EventHandlerList)) {
					th.Enabled = EventHandlerListCheckBox.Checked;
				}
				ComponentInspectorProperties.EnableTypeHandler(th.Name, th.Enabled);
			}
			return true;
		}
		
		CheckBox EventHandlerListCheckBox {
			get {
				return Get<CheckBox>("eventHandlerList");
			}
		}
		
		CheckBox EnumeratorCheckBox {
			get {
				return Get<CheckBox>("enumerator");
			}
		}
		
		CheckBox ListCheckBox {
			get {
				return Get<CheckBox>("list");
			}
		}
	}
}
