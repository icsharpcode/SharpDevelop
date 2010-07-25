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

using ICSharpCode.Core;

namespace NoGoop.ObjBrowser.Panels
{
	internal class CustTypeHandlerPanel : Panel, ICustPanel
	{
		protected CheckedListBox    _typeHandlerCheck;

		internal CustTypeHandlerPanel()
		{
			Label l;

			Text = StringParser.Parse("${res:ComponentInspector.InspectorMenu.TypeHandlerOptionsPanel.Title}");

			ICollection typeHandlers = TypeHandlerManager.Instance.GetTypeHandlers();

			_typeHandlerCheck = new CheckedListBox();
			_typeHandlerCheck.CheckOnClick = true;
			_typeHandlerCheck.Dock = DockStyle.Fill;
			_typeHandlerCheck.BackColor = BackColor;
			_typeHandlerCheck.BorderStyle = BorderStyle.None;
			_typeHandlerCheck.ThreeDCheckBoxes = true;
			foreach (TypeHandlerManager.TypeHandlerInfo th in typeHandlers)
				_typeHandlerCheck.Items.Add(th);
			Controls.Add(_typeHandlerCheck);

			l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = 50;
			l.Text = StringParser.Parse("${res:ComponentInspector.TypeHandlerOptionsPanel.InformationLabel}");
			Controls.Add(l);

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
			ICollection typeHandlers = TypeHandlerManager.Instance.GetTypeHandlers();

			int i = 0;
			foreach (TypeHandlerManager.TypeHandlerInfo th in typeHandlers) {
				_typeHandlerCheck.SetItemChecked(i++, th.Enabled);
			}
		}

		public bool AfterShow()
		{
			int i = 0;
			foreach (TypeHandlerManager.TypeHandlerInfo th in _typeHandlerCheck.Items) {
				th.Enabled = _typeHandlerCheck.GetItemChecked(i++);
				ComponentInspectorProperties.EnableTypeHandler(th.Name, th.Enabled);
			}
			return true;
		}
	}
}
