// <file>
//	 <copyright see="prj:///doc/copyright.txt"/>
//	 <license see="prj:///doc/license.txt"/>
//	 <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//	 <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.ObjBrowser.GuiDesigner;

namespace NoGoop.ObjBrowser
{
	internal class ImagePanel : Panel
	{
		Panel		  _wrapperPanel;
		Label		  _checkLabel;
		CheckBox	  _designCheck;

		Panel		  _designPanel;
		Panel		  _nonDesignPanel;
		Panel		  _desLabelPanel = new Panel();

		internal static ImagePanel CreateImagePanel(int width, bool showDesignModeCheckButton)
		{
			ObjectBrowser objB = ObjectBrowser.ObjBrowser;

			ImagePanel panel = new ImagePanel();
			panel._nonDesignPanel = new Panel();
			panel._nonDesignPanel.Name = "Non-Design Surface";
			panel._nonDesignPanel.Dock = DockStyle.Fill;

			panel._designPanel = new Panel();
			panel._designPanel.Name = "Design Surface";
			panel._designPanel.Dock = DockStyle.Fill;

			panel.WrapImagePanel(width, showDesignModeCheckButton);
			return panel;
		}
		
		internal Panel DesignPanel {
			get {
				return _designPanel;
			}
		}
		
		internal Panel WrapperPanel {
			get {
				return _wrapperPanel;
			}
		}

		void WrapImagePanel(int width, bool showDesignModeCheckButton)
		{
			// Create the label for the check box to control design time
			_desLabelPanel = new Panel();
			_desLabelPanel.Dock = DockStyle.Left;
			_desLabelPanel.Width = width;
			_desLabelPanel.Height = TreeListPanel.BASE_HEADER_HEIGHT;

			if (showDesignModeCheckButton) {
				_checkLabel = new Label();
				_checkLabel.Dock = DockStyle.Left;
				_checkLabel.Text = StringParser.Parse("${res:ComponentInspector.ImagePanel.DesignModeLabel}");
				_checkLabel.AutoSize = true;
				_desLabelPanel.Controls.Add(_checkLabel);

				_designCheck = new CheckBox();
				_designCheck.Checked = true;
				_designCheck.Dock = DockStyle.Left;
			
				// This does not work because the checkbox alignment of the text
				// does not match that of text outside of the checkbox, sigh
				//_designCheck.Text = "Design mode";
				//_designCheck.TextAlign = ContentAlignment.TopRight;
				_designCheck.CheckAlign = ContentAlignment.TopLeft;
				_designCheck.Width = 15;
				_designCheck.CheckedChanged += new EventHandler(DesignChecked);
				_desLabelPanel.Controls.Add(_designCheck);
			}

			Label panelName = new Label();
			panelName.Dock = DockStyle.Right;
			panelName.Text = StringParser.Parse("${res:ComponentInspector.ImagePanel.ControlDesignSurfaceLabel}");
			panelName.AutoSize = true;
			_desLabelPanel.Controls.Add(panelName);

			_wrapperPanel = new Panel();
			_wrapperPanel.Width = width;
			_wrapperPanel.Controls.Add(_designPanel);
			_wrapperPanel.BorderStyle = BorderStyle.Fixed3D;
			new PanelLabel(_wrapperPanel, _desLabelPanel);
		}

		void SwitchWindowTarget(Control con)
		{
			IWindowTarget wt;

			foreach (Control c in con.Controls) {
				if (c.Site is DesignerSite) {
					if (_designCheck.Checked) {
						wt = ((DesignerSite)c.Site).DesignWindowTarget;
						if (wt != null) {
							// The designer gets the windows events
							c.WindowTarget = wt;
						}
					} else {
						wt = ((DesignerSite)c.Site).OrigWindowTarget;
						if (wt != null) {
							// So the control gets the windows events 
							// instead of the designer
							c.WindowTarget = wt;
						}
					}
				}
				SwitchWindowTarget(c);
			}
		}

		void DesignChecked(Object sender, EventArgs args)
		{
			Control selUIService = DesignerHost.Host.SelectionUIService;

			// Swap the design/non design panels by rebuilding the control
			// list for the wrapper panel.
			ObjectBrowser.DesignerHost.DesignMode = _designCheck.Checked;
			_wrapperPanel.Controls.Clear();

			if (_designCheck.Checked) {
				// Move the controls from the non-design panel to the 
				// design panel
				for (int i = 0; i < _nonDesignPanel.Controls.Count; ) {
					Control c = (Control)_nonDesignPanel.Controls[i];
					_nonDesignPanel.Controls.Remove(c);

					// This Control could have been added to the design 
					// surface while it was not in design mode
					if (c.Site == null) {
						IDesigner designer = DesignerHost.Host.GetDesigner(c);
						if (designer != null)
							designer.Initialize(c);
					}

					_designPanel.Controls.Add(c);
				}

				SwitchWindowTarget(_designPanel);
				_wrapperPanel.Controls.Add(_designPanel);
			} else {
				// Move the controls from the design panel to the 
				// non-design panel
				for (int i = 0; i < _designPanel.Controls.Count; ) {
					Control c = (Control)_designPanel.Controls[i];
					if (selUIService.Equals(c)) {
						i++;
						continue;
					}
					_designPanel.Controls.Remove(c);
					try {
						_nonDesignPanel.Controls.Add(c);
					} catch (Exception ex) {
						ErrorDialog.Show(ex,
										 "Error adding control " + c + " to "
										 + "non-design panel",
										 MessageBoxIcon.Warning);
						i++;
					}
				}
				SwitchWindowTarget(_nonDesignPanel);
				_wrapperPanel.Controls.Add(_nonDesignPanel);
			}
			ResetSize(selUIService);

			new PanelLabel(_wrapperPanel, _desLabelPanel);
		}

		internal void ResetSize(Control selUIService)
		{
			if (_designCheck == null || _designCheck.Checked) {
				selUIService.Height = _designPanel.ClientSize.Height;
				selUIService.Width = _designPanel.ClientSize.Width;
			} else {
				selUIService.Height = 0;
				selUIService.Width = 0;
			}
		}

		// This is used to add a control to the design surface
		// when it was added to the object tree, this is not used
		// when the control is dragged directly to the design surface
		internal void AddControl(ObjectInfo objInfo, Control control)
		{
			// For some reason, property grid gets an error when
			// initially associated with the image panel, associated
			// it with another panel first.
			//if (control is PropertyGrid)
			//	ObjectBrowser._testPanel.Controls.Add(control);

			IDesigner designer = null;
			DesignerHost host = DesignerHost.Host;
			if (host.DesignMode) {
				host.AddingControls = true;
				designer = host.GetDesigner(control, objInfo);
				if (designer != null)
					designer.Initialize(control);

				// If no designer, can't add it to design surface
				if (designer != null)
					_designPanel.Controls.Add(control);
				host.AddingControls = false;
			} else {
				_nonDesignPanel.Controls.Add(control);
			}


			// FIXME - Hack
			//if (control is PropertyGrid)
			//	designer.Initialize(control);
		}

		// returns true if the control is found
		bool RemoveControlFromControl(Control parent, Control control)
		{
			if (parent.Controls.Contains(control)) {
				parent.Controls.Remove(control);
				return true;
			}

			foreach (Control c in parent.Controls) {
				if (RemoveControlFromControl(c, control))
					return true;
			}
			return false;
		}

		internal void RemoveControl(Control control)
		{
			RemoveControlFromControl(_designPanel, control);
			RemoveControlFromControl(_nonDesignPanel, control);
		}
	}
}
