using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices.APIs;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
	internal class CustomEdit : NativeWindow, IWin32Window
	{
		#region Properties
		private TreeListViewItemEditControlHandle _editorhandle;
		private EditItemInformations _informations;
		private Control _editor;
		new public IntPtr Handle
		{
			get{return base.Handle;}
		}
		private TreeListView _treelistview;
		#endregion
		#region Constructor & Destructor
		private CustomEdit(){}
		public CustomEdit(IntPtr handle, TreeListView treelistview, Control editor)
		{
			_treelistview = treelistview;
			_informations = _treelistview.EditedItem;
			if(editor == null) _editor = new TextBox();
			else _editor = editor;
			_editor.Hide();
			if(!_treelistview.Controls.Contains(_editor))
				_treelistview.Controls.Add(_editor);
			_editorhandle = new TreeListViewItemEditControlHandle(_treelistview, _editor, this);
			AssignHandle(handle);
		}
		#endregion

		#region Functions
		public void ShowEditControl()
		{
			if(_treelistview.FocusedItem == null) return;
			ListViewItem item = (ListViewItem) _treelistview.EditedItem.Item;
			Rectangle rec = _treelistview.EditedItem.ColumnIndex > 0 ?
				_treelistview.GetSubItemRect(item.Index, _treelistview.EditedItem.ColumnIndex) :
				_treelistview.GetItemRect(item.Index, ItemBoundsPortion.Label);
			_editor.Size = rec.Size;
			_editor.Location = rec.Location;
			_editor.Top--;
			_editor.Show();
			_editor.Text = item.SubItems[_treelistview.EditedItem.ColumnIndex].Text;
			_editor.Focus();
		}
		public void HideEditControl()
		{
			_editor.Hide();
			ReleaseHandle();
			_editorhandle.ReleaseHandle();
		}
		#endregion

		#region WndProc
		public void SendMessage(ref Message m)
		{
			WndProc(ref m);
		}
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case (int)APIsEnums.WindowMessages.SHOWWINDOW:
					bool show = m.WParam != IntPtr.Zero;
					if(show) ShowEditControl();
					else HideEditControl();
					return;
			}
		}
		#endregion
	}
	internal class TreeListViewItemEditControlHandle : NativeWindow, IWin32Window
	{
		#region Properties
		private CustomEdit _customedit;
		private Control _control;
		private TreeListView _treelistview;
		new public IntPtr Handle
		{
			get{return base.Handle;}
		}
		#endregion
		#region Constructor / Destructor
		public TreeListViewItemEditControlHandle(TreeListView treelistview, Control control, CustomEdit customedit)
		{
			_control = control;
			_treelistview = treelistview;
			_customedit = customedit;
			if(!control.Created) control.CreateControl();
			AssignHandle(control.Handle);
		}
		#endregion

		#region End edit
		private void EndEdit(bool Cancel)
		{
			if(!_treelistview.InEdit) return;
			_treelistview.ExitEdit(Cancel, _control.Text);
		}
		#endregion
		#region OnKillFocus
		private bool OnKillFocus(Message m)
		{
			// If the control is a combobox don't end edit if the handle is a handle
			// of one of the sub controls of the combobox
			if(!(_control is ComboBox)) return true;
			APIsStructs.PCOMBOBOXINFO info = new APIsStructs.PCOMBOBOXINFO();
			info.cbSize = (uint)Marshal.SizeOf(typeof(APIsStructs.PCOMBOBOXINFO));
			if(!APIsUser32.GetComboBoxInfo(_control.Handle, ref info)) return true;
			if(m.WParam == info.hwndCombo || m.WParam == info.hwndItem || m.WParam == info.hwndList)
			{
				ReleaseHandle();
				AssignHandle(m.WParam);
				return false;
			}
			return true;
		}
		#endregion
		#region Wndproc
		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				case (int) APIsEnums.WindowMessages.KEYDOWN:
					Keys key = (Keys)(int) m.WParam;
					if(key != Keys.Return && key != Keys.Escape) break;
					bool Cancel = key != Keys.Enter;
					EndEdit(Cancel);
					return;
				case (int) APIsEnums.WindowMessages.KILLFOCUS:
					if(OnKillFocus(m))
					{
						EndEdit(!(_control is ComboBox && _treelistview.EditedItem.Label != _control.Text));
						return;
					}
					break;
			}
			base.WndProc(ref m);
		}
		private int HighOrder(IntPtr Param)
		{
			int intparam = Param.ToInt32();
			return (intparam >> 16) & 0x0000ffff;
		}
		private int LowOrder(IntPtr Param)
		{
			int intparam = Param.ToInt32();
			return intparam & 0x0000ffff;
		}
		#endregion
	}
}
