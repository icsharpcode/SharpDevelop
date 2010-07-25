// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace NoGoop.ObjBrowser.Panels
{
	internal class CustActiveXPanel : Panel, ICustPanel
	{
		protected CheckBox          _getRunningPanelCheck;
		protected CheckBox          _autoInvokeCheck;
		protected const int         FIELD_WIDTH = 250;

		internal CustActiveXPanel()
		{
			Label l;

			Text = "ActiveX/COM";

			_getRunningPanelCheck = new CheckBox();
			_getRunningPanelCheck.Location = new Point(10, 10);
			_getRunningPanelCheck.Width = FIELD_WIDTH;
			_getRunningPanelCheck.Text = StringParser.Parse("${res:ComponentInspector.CustomActiveXPanel.GetRunningComObjectsCheckBox}");
			Controls.Add(_getRunningPanelCheck);

			_autoInvokeCheck = new CheckBox();
			_autoInvokeCheck.Location = new Point(10, 30);
			_autoInvokeCheck.Width = FIELD_WIDTH;
			_autoInvokeCheck.Text = StringParser.Parse("${res:ComponentInspector.CustomActiveXPanel.AutomaticallyGetPropertiesCheckBox}");
			Controls.Add(_autoInvokeCheck);

			// Padding
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 5;
			Controls.Add(l);
		}
		
		Size ICustPanel.PreferredSize {
			get {
				return Size.Empty;
			}
		}

		public void BeforeShow()
		{
			// The state of the check box needs to be refreshed
			// from the current state in the event the dialog
			// was previously cancelled.
			_getRunningPanelCheck.Checked = ComponentInspectorProperties.AddRunningComObjects;
			_autoInvokeCheck.Checked = ComponentInspectorProperties.AutoInvokeProperties;
		}

		public bool AfterShow()
		{
			// Set the properties
			ComponentInspectorProperties.AddRunningComObjects = _getRunningPanelCheck.Checked;
			ComponentInspectorProperties.AutoInvokeProperties = _autoInvokeCheck.Checked;
			return true;
		}
	}
}
