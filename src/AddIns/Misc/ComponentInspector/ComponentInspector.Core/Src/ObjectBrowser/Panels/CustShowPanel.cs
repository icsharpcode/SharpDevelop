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
	internal class CustShowPanel : Panel, ICustPanel
	{
		protected CheckBox          _showAssyPanelCheck;
		protected CheckBox          _showControlPanelCheck;
		protected CheckBox          _showGacPanelCheck;

		protected const int         FIELD_WIDTH = 250;

		internal CustShowPanel()
		{
			Label l;

			Text = StringParser.Parse("${res:ComponentInspector.GeneralOptionsPanel.PanelsGroupBox}");

			_showAssyPanelCheck = new CheckBox();
			_showAssyPanelCheck.Location = new Point(10, 10);
			_showAssyPanelCheck.Width = FIELD_WIDTH;
			_showAssyPanelCheck.Text = StringParser.Parse("${res:ComponentInspector.GeneralOptionsPanel.ShowAssemblyPanelCheckBox}");
			Controls.Add(_showAssyPanelCheck);

			_showControlPanelCheck = new CheckBox();
			_showControlPanelCheck.Location = new Point(10, 30);
			_showControlPanelCheck.Width = FIELD_WIDTH;
			_showControlPanelCheck.Text = StringParser.Parse("${res:ComponentInspector.GeneralOptionsPanel.ShowControlsPanelCheckBox}");
			Controls.Add(_showControlPanelCheck);

			_showGacPanelCheck = new CheckBox();
			_showGacPanelCheck.Location = new Point(10, 50);
			_showGacPanelCheck.Width = FIELD_WIDTH;
			_showGacPanelCheck.Text = StringParser.Parse("${res:ComponentInspector.GeneralOptionsPanel.ShowGacPanelCheckBox}");
			Controls.Add(_showGacPanelCheck);

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
			_showAssyPanelCheck.Checked = ComponentInspectorProperties.ShowAssemblyPanel;
			_showControlPanelCheck.Checked = ComponentInspectorProperties.ShowControlPanel;
			_showGacPanelCheck.Checked = ComponentInspectorProperties.ShowGacPanel;
		}

		public bool AfterShow()
		{
			// Set the properties
			ComponentInspectorProperties.ShowAssemblyPanel = _showAssyPanelCheck.Checked;
			ComponentInspectorProperties.ShowControlPanel = _showControlPanelCheck.Checked;
			ComponentInspectorProperties.ShowGacPanel = _showGacPanelCheck.Checked;
			return true;
		}
	}
}
