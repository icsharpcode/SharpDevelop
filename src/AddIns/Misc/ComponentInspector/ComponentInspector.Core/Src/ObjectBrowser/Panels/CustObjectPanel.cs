// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.Win32;

using NoGoop.Controls;
using NoGoop.ObjBrowser.Types;
using NoGoop.Win32;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Panels
{
	internal class CustObjectPanel : Panel, ICustPanel
	{
		protected CheckBox          _showPropMethCheck;
		protected CheckBox          _showFieldsCheck;
		protected CheckBox          _showPropertiesCheck;
		protected CheckBox          _showMethodsCheck;
		protected CheckBox          _showEventsCheck;
		protected CheckBox          _showBaseClassesCheck;
		protected CheckBox          _showPublicOnlyCheck;
		protected CheckBox          _showBaseClassNamesCheck;
		protected CheckBox          _showMemberCategoriesCheck;
		protected CheckBox          _showObjectAsBaseCheck;
		protected CheckBox          _showBaseCategoriesCheck;
		protected CheckBox          _hexDisplayCheck;
		protected TextBox           _showCategoriesCount;

		protected Panel             _showCatCountPanel;

		protected const int         FIELD_WIDTH = 270;

		internal CustObjectPanel()
		{
			Panel panel;
			Label l;

			Text = "Object Tree";

			// Other panel
			panel = new Panel();
			panel.Dock = DockStyle.Top;
			panel.Height = 50;
			Controls.Add(panel);

			_hexDisplayCheck = new CheckBox();
			_hexDisplayCheck.Location = new Point(10, 0);
			_hexDisplayCheck.Width = FIELD_WIDTH;
			_hexDisplayCheck.Text = "Display integers in hex";
			panel.Controls.Add(_hexDisplayCheck);

			_showBaseClassNamesCheck = new CheckBox();
			_showBaseClassNamesCheck.Location = new Point(10, 20);
			_showBaseClassNamesCheck.Width = FIELD_WIDTH;
			_showBaseClassNamesCheck.Text = "Show base class names";
			panel.Controls.Add(_showBaseClassNamesCheck);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = "Other";
			l.AutoSize = true;
			Controls.Add(l);


			// Categories panel
			panel = new Panel();
			panel.Dock = DockStyle.Top;
			panel.Height = 95;
			Controls.Add(panel);

			_showMemberCategoriesCheck = new CheckBox();
			_showMemberCategoriesCheck.Location = new Point(10, 0);
			_showMemberCategoriesCheck.Width = FIELD_WIDTH;
			_showMemberCategoriesCheck.Text = "Show member categories";
			_showMemberCategoriesCheck.Click +=  new EventHandler(ShowCatClicked);
			panel.Controls.Add(_showMemberCategoriesCheck);

			_showBaseCategoriesCheck = new CheckBox();
			_showBaseCategoriesCheck.Location = new Point(10, 20);
			_showBaseCategoriesCheck.Width = FIELD_WIDTH;
			_showBaseCategoriesCheck.Text = "Show base class category";
			_showBaseCategoriesCheck.Click += new EventHandler(ShowCatClicked);
			panel.Controls.Add(_showBaseCategoriesCheck);

			_showCatCountPanel = new Panel();
			_showCatCountPanel.Location = new Point(27, 45);
			_showCatCountPanel.Width = FIELD_WIDTH;
			_showCategoriesCount = new NumericTextBox();
			_showCategoriesCount.Width = 30;
			_showCategoriesCount.Height = 25;
			_showCategoriesCount.Dock = DockStyle.Left;
			_showCatCountPanel.Controls.Add(_showCategoriesCount);
			l = new Label();
			l.Dock = DockStyle.Left;
			l.Text = "When number of members exceeds";
			l.AutoSize = true;
			//l.Width = 150;
			_showCatCountPanel.Controls.Add(l);
			_showCatCountPanel.Height = _showCategoriesCount.Height;
			panel.Controls.Add(_showCatCountPanel);

			_showObjectAsBaseCheck = new CheckBox();
			_showObjectAsBaseCheck.Location = new Point(10, 65);
			_showObjectAsBaseCheck.Width = FIELD_WIDTH;
			_showObjectAsBaseCheck.Text = "Show Object members in base class category";
			panel.Controls.Add(_showObjectAsBaseCheck);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = "Categories";
			l.AutoSize = true;
			Controls.Add(l);


			// Show panel
			panel = new Panel();
			panel.Dock = DockStyle.Top;
			panel.Height = 70;
			Controls.Add(panel);

			_showBaseClassesCheck = new CheckBox();
			_showBaseClassesCheck.Location = new Point(10, 0);
			_showBaseClassesCheck.Width = FIELD_WIDTH;
			_showBaseClassesCheck.Text = "Base class members";
			panel.Controls.Add(_showBaseClassesCheck);

			_showPublicOnlyCheck = new CheckBox();
			_showPublicOnlyCheck.Location = new Point(10, 20);
			_showPublicOnlyCheck.Width = FIELD_WIDTH;
			_showPublicOnlyCheck.Text = "Public members only";
			panel.Controls.Add(_showPublicOnlyCheck);

			_showPropMethCheck = new CheckBox();
			_showPropMethCheck.Location = new Point(10, 40);
			_showPropMethCheck.Width = FIELD_WIDTH;
			_showPropMethCheck.Text = "Property/Event accessor methods";
			panel.Controls.Add(_showPropMethCheck);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = "Show";
			l.AutoSize = true;
			Controls.Add(l);


			// Show members panel
			panel = new Panel();
			panel.Dock = DockStyle.Top;
			panel.Height = 50;
			Controls.Add(panel);

			// Add these using fixed locations
			_showFieldsCheck = new CheckBox();
			_showFieldsCheck.Location = new Point(10, 0);
			_showFieldsCheck.Text = "Fields";
			panel.Controls.Add(_showFieldsCheck);

			_showPropertiesCheck = new CheckBox();
			_showPropertiesCheck.Location = new Point(10, 20);
			_showPropertiesCheck.Text = "Properties";
			panel.Controls.Add(_showPropertiesCheck);

			_showMethodsCheck = new CheckBox();
			_showMethodsCheck.Location = new Point(120, 0);
			_showMethodsCheck.Text = "Methods";
			panel.Controls.Add(_showMethodsCheck);

			_showEventsCheck = new CheckBox();
			_showEventsCheck.Location = new Point(120, 20);
			_showEventsCheck.Text = "Events";
			panel.Controls.Add(_showEventsCheck);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.Text = "Show Members";
			l.AutoSize = true;
			Controls.Add(l);

			// Padding
			l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 5;
			Controls.Add(l);
		}

		Size ICustPanel.PreferredSize {
			get {
				return new Size(300, 450);
			}
		}
		
		public void BeforeShow()
		{
			// The state of the check box needs to be refreshed
			// from the current state in the event the dialog
			// was previously cancelled.
			_showPropMethCheck.Checked = ComponentInspectorProperties.ShowPropertyAccessorMethods;

			_showFieldsCheck.Checked = ComponentInspectorProperties.ShowFields;
			_showPropertiesCheck.Checked = (ComponentInspectorProperties.ShowProperties);
			_showMethodsCheck.Checked = ComponentInspectorProperties.ShowMethods;
			_showEventsCheck.Checked = ComponentInspectorProperties.ShowEvents;

			_showBaseClassesCheck.Checked = ComponentInspectorProperties.ShowBaseClasses;
			_showPublicOnlyCheck.Checked = ComponentInspectorProperties.ShowPublicMembersOnly;

			_showMemberCategoriesCheck.Checked = ComponentInspectorProperties.ShowMemberCategories;
			_showObjectAsBaseCheck.Checked = ComponentInspectorProperties.ShowObjectAsBaseClass;
			_showBaseCategoriesCheck.Checked = ComponentInspectorProperties.ShowBaseCategories;
			_showCategoriesCount.Text = Convert.ToString(ComponentInspectorProperties.CategoryThreshold);
			// Compute enable of show cat count
			ShowCatClicked(null, null);

			_showBaseClassNamesCheck.Checked = ComponentInspectorProperties.ShowBaseClassNames;
			_hexDisplayCheck.Checked = ComponentInspectorProperties.DisplayHex;
		}

		public bool AfterShow()
		{
			int showMemCat = Convert.ToInt32(_showCategoriesCount.Text);

			// Set the properties
			ComponentInspectorProperties.ShowPropertyAccessorMethods =_showPropMethCheck.Checked;
			ComponentInspectorProperties.ShowFields = _showFieldsCheck.Checked;
			ComponentInspectorProperties.ShowProperties = _showPropertiesCheck.Checked;
			ComponentInspectorProperties.ShowMethods = _showMethodsCheck.Checked;
			ComponentInspectorProperties.ShowEvents = _showEventsCheck.Checked;

			ComponentInspectorProperties.ShowBaseClasses = _showBaseClassesCheck.Checked;
			ComponentInspectorProperties.ShowPublicMembersOnly = _showPublicOnlyCheck.Checked;

			ComponentInspectorProperties.ShowMemberCategories = _showMemberCategoriesCheck.Checked;
			ComponentInspectorProperties.ShowBaseCategories = _showBaseCategoriesCheck.Checked;
			ComponentInspectorProperties.ShowObjectAsBaseClass = _showObjectAsBaseCheck.Checked;
			ComponentInspectorProperties.CategoryThreshold = showMemCat;

			ComponentInspectorProperties.ShowBaseClassNames = _showBaseClassNamesCheck.Checked;
			ComponentInspectorProperties.DisplayHex = _hexDisplayCheck.Checked;
			return true;
		}

		protected void ShowCatClicked(Object sender, EventArgs e)
		{
			_showCatCountPanel.Enabled = _showMemberCategoriesCheck.Checked || _showBaseCategoriesCheck.Checked;
		}
	}
}
