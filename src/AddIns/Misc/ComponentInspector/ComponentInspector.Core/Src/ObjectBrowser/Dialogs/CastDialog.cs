// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.Dialogs
{
	internal class CastDialog : Dialog
	{
		protected TextBox               _textBox;
		protected CheckBox              _rememberCheck;

		protected bool                  _removed;
		protected CastInfo              _castInfo;
		protected MemberInfo            _memberInfo;
		protected Object                _currentObj;

		internal bool Removed {
			get {
				return _removed;
			}
		}

		internal CastInfo CastInfo {
			get {
				return _castInfo;
			}
		}

		// castInfo is an existing CastInfo if any, currentObj is
		// the current value to be cast.  We will try the cast with
		// this value to make sure its possible before accepting the
		// new cast.
		internal CastDialog(CastInfo castInfo,
							MemberInfo member,
							Object currentObj) : base(!INCLUDE_BUTTONS)
		{
			_memberInfo = member;
			_castInfo = castInfo;
			_currentObj = currentObj;

			Activated += new EventHandler(ActivateHandler);

			if (_memberInfo != null)
				Text = "Cast for " + _memberInfo.ToString();
			else
				Text = "Cast";
			Width = 400;
			Height = 170;

			// Panel for dialog contents
			Panel panel = new Panel();
			panel.Dock = DockStyle.Top;
			Controls.Add(panel);

			if (_memberInfo != null) {
				_rememberCheck = new CheckBox();
				_rememberCheck.Dock = DockStyle.Top;
				_rememberCheck.Text = "Remember cast across sessions?";
				if (_castInfo != null)
					_rememberCheck.Checked = _castInfo.Perm;
				else
					_rememberCheck.Checked = true;
				panel.Controls.Add(_rememberCheck);
			}

			// Spacing
			Label l = new Label();
			l.Dock = DockStyle.Top;
			panel.Controls.Add(l);

			// Panel for textbox
			Panel textPanel = new Panel();
			textPanel.Dock = DockStyle.Top;
			panel.Controls.Add(textPanel);

			_textBox = new TextBox();
			_textBox.Dock = DockStyle.Fill;
			if (_castInfo != null)
				_textBox.Text = _castInfo.CastType.FullName;
			textPanel.Controls.Add(_textBox);

			l = new Label();
			l.Dock = DockStyle.Left;
			l.Text = "Cast to ";
			l.AutoSize = true;
			textPanel.Controls.Add(l);

			textPanel.Height = _textBox.Height;

			Panel bp = new Panel();
			bp.Dock = DockStyle.Bottom;

			l = new Label();
			l.Dock = DockStyle.Fill;
			bp.Controls.Add(l);

			Button ok = Utils.MakeButton("OK");
			ok.Dock = DockStyle.Right;
			ok.DialogResult = DialogResult.OK;
			AcceptButton = ok;
			bp.Controls.Add(ok);

			if (_castInfo != null)
			{
				Button b = Utils.MakeButton("Remove");
				b.Dock = DockStyle.Right;
				b.DialogResult = DialogResult.OK;
				b.Click += new EventHandler(RemoveClick);
				bp.Controls.Add(b);
			}

			Button cancel = Utils.MakeButton("Cancel");
			cancel.Dock = DockStyle.Right;
			cancel.DialogResult = DialogResult.Cancel;
			bp.Controls.Add(cancel);

			bp.Height = Utils.BUTTON_HEIGHT;

			Controls.Add(bp);

		}

		// When this dialog is activated make sure we point to
		// the textbox
		protected void ActivateHandler(object sender, 
									   EventArgs e)
		{
			_textBox.Focus();
		}

		protected void RemoveClick(Object sender, EventArgs e)
		{
			CastInfo.RemoveCast(_castInfo);
			_removed = true;
		}

		internal bool DoShowDialog()
		{
			while (true) {
				if (ShowDialog() != DialogResult.OK)
					return false;

				if (_removed) {
					_castInfo = null;
					return true;
				}

				Type castType = null;
				try {
					castType = ReflectionHelper.GetType(_textBox.Text);
				} catch (Exception ex) {
					ErrorDialog.Show(ex,
									 "Error determining type of cast",
									 MessageBoxIcon.Error);
					continue;
				}

				if (castType == null) {
					ErrorDialog.Show("Cannot find type " + _textBox.Text
									 + " in any of the assemblies currently "
									 + "open.",
									 "Cannot find type " + _textBox.Text,
									 MessageBoxIcon.Error);
					continue;
				}

				// This throws if there is a problem with casting the
				// current object
				try {
					if (_memberInfo != null) {
						_castInfo = CastInfo.AddCast(_memberInfo,
													 castType, 
													 _rememberCheck.Checked,
													 _currentObj);
					} else {
						_castInfo = CastInfo.AddCast(null,
													 castType, 
													 false,
													 _currentObj);
					}
				} catch (Exception ex) {
					ErrorDialog.Show(ex,
									 "Cast type incompatible with "
									 + "object's type",
									 MessageBoxIcon.Error);
					continue;
				}
				return true;
			}
		}
	}
}
