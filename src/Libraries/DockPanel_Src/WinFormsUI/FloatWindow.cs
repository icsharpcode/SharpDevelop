// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI
{
	/// <summary>
	/// Descripiton of class FloatWindow
	/// </summary>
	public class FloatWindow : Form, IDockListContainer
	{
		private DockList m_dockList;
		internal static Size DefaultWindowSize = new Size(300, 300);
		internal const int WM_CHECKDISPOSE = (int)(Win32.Msgs.WM_USER + 1);

		public FloatWindow(DockPanel dockPanel, DockPane pane)
		{
			InternalConstruct(dockPanel, pane, false, Rectangle.Empty);
		}

		public FloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
		{
			InternalConstruct(dockPanel, pane, true, bounds);
		}

		private void InternalConstruct(DockPanel dockPanel, DockPane pane, bool boundsSpecified, Rectangle bounds)
		{
			if (dockPanel == null)
				throw(new ArgumentNullException(ResourceHelper.GetString("FloatWindow.Constructor.NullDockPanel")));

			m_dockList = new DockList(this);

			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			ShowInTaskbar = false;
			
			SuspendLayout();
			if (boundsSpecified)
			{
				Bounds = bounds;
				StartPosition = FormStartPosition.Manual;
			}
			else
				StartPosition = FormStartPosition.WindowsDefaultLocation;

			m_dummyControl = new DummyControl();
			m_dummyControl.Bounds = Rectangle.Empty;
			Controls.Add(m_dummyControl);

			m_dockPanel = dockPanel;
			Owner = DockPanel.FindForm();
			DockPanel.AddFloatWindow(this);
			if (pane != null)
				pane.FloatWindow = this;

			ResumeLayout();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (DockPanel != null)
					DockPanel.RemoveFloatWindow(this);
				m_dockPanel = null;
			}
			base.Dispose(disposing);
		}

		private bool m_allowRedocking = true;
		public bool AllowRedocking
		{
			get	{	return m_allowRedocking;	}
			set	{	m_allowRedocking = value;	}
		}

		protected override Size DefaultSize
		{
			get	{	return FloatWindow.DefaultWindowSize;	}
		}

		public DockList DockList
		{
			get	{	return m_dockList;	}
		}

		public DisplayingDockList DisplayingList
		{
			get	{	return DockList.DisplayingList;	}
		}

		private DockPanel m_dockPanel;
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		public DockState DockState
		{
			get	{	return DockState.Float;	}
		}
	
		public bool IsFloat
		{
			get	{	return DockState == DockState.Float;	}
		}

		private Control m_dummyControl;
		internal Control DummyControl
		{
			get	{	return m_dummyControl;	}
		}

		public bool IsDockStateValid(DockState dockState)
		{
			foreach (DockPane pane in DockList)
				foreach (DockContent content in pane.Contents)
					if (!DockHelper.IsDockStateValid(dockState, content.DockableAreas))
						return false;

			return true;
		}

		protected override void OnActivated(EventArgs e)
		{
			DockPanel.FloatWindows.BringWindowToFront(this);
			base.OnActivated (e);
		}

		protected override void OnLayout(LayoutEventArgs levent)
		{
			DisplayingList.Refresh();
			Visible = (DisplayingList.Count > 0);
			SetText();

			base.OnLayout(levent);
		}

		internal void SetText()
		{
			DockPane theOnlyPane = (DisplayingList.Count == 1) ? DisplayingList[0] : null;

			if (theOnlyPane == null)
				Text = string.Empty;
			else if (theOnlyPane.ActiveContent == null)
				Text = string.Empty;
			else
				Text = theOnlyPane.ActiveContent.Text;
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)Win32.Msgs.WM_NCLBUTTONDOWN)
			{
				if (IsDisposed)
					return;

				uint result = User32.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
				if (result == 2 && DockPanel.AllowRedocking && this.AllowRedocking)	// HITTEST_CAPTION
				{
					Activate();
					m_dockPanel.DragHandler.BeginDragFloatWindow(this);
				}
				else
					base.WndProc(ref m);

				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_CLOSE)
			{
				if (DockList.Count == 0)
				{
					base.WndProc(ref m);
					return;
				}
				
				for (int i=DockList.Count - 1; i>=0; i--)
				{
					if (DockList[i].DockState != DockState.Float)
						continue;

					DockContentCollection contents = DockList[i].Contents;
					for (int j=contents.Count - 1; j>=0; j--)
					{
						DockContent content = contents[j];
						if (content.HideOnClose)
							content.Hide();
						else
							content.Close();
					}
				}
				
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_NCLBUTTONDBLCLK)
			{
				uint result = User32.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
				if (result != 2)	// HITTEST_CAPTION
				{
					base.WndProc(ref m);
					return;
				}
				
				// Restore to panel
				DockContent activeContent = DockPanel.ActiveContent;
				foreach(DockPane pane in DockList)
				{
					if (pane.DockState != DockState.Float)
						continue;
					pane.RestoreToPanel();
				}
				if (activeContent != null)
					activeContent.Activate();

				return;
			}
			else if (m.Msg == WM_CHECKDISPOSE)
			{
				if (DockList.Count == 0)
					Dispose();

				return;
			}

			base.WndProc(ref m);
		}

		public virtual Rectangle DisplayingRectangle
		{
			get	{	return ClientRectangle;	}
		}
	}
}
