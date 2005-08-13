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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/ClassDef/*'/>
	public abstract class DockPaneStripBase : Control
	{
		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Construct[@name="(DockPane)"]/*'/>
		protected internal DockPaneStripBase(DockPane pane)
		{
			m_dockPane = pane;

			SetStyle(ControlStyles.Selectable, false);
		}

		private DockPane m_dockPane;
		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="DockPane"]/*'/>
		protected DockPane DockPane
		{
			get	{	return m_dockPane;	}
		}

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="Appearance"]/*'/>
		protected DockPane.AppearanceStyle Appearance
		{
			get	{	return DockPane.Appearance;	}
		}

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="OutlineSize"]/*'/>
		protected int OutlineSize
		{
			get	{	return MeasureOutline.Width;	}
		}

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Property[@name="Tabs"]/*'/>
		protected DockPaneTabCollection Tabs
		{
			get	{	return DockPane.Tabs;	}
		}

		internal void RefreshChanges()
		{
			OnRefreshChanges();
		}

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="OnRefreshChanges()"]/*'/>
		protected virtual void OnRefreshChanges()
		{
		}

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="MeasureHeight()"]/*'/>
		protected internal abstract int MeasureHeight();

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="EnsureTabVisible(DockContent)"]/*'/>
		protected internal abstract void EnsureTabVisible(DockContent content);

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetHitTest"]/*'/>
		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetHitTest()"]/*'/>
		protected int GetHitTest()
		{
			return GetHitTest(PointToClient(Control.MousePosition));
		}

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetHitTest(Point)"]/*'/>
		protected internal abstract int GetHitTest(Point point);

		/// <include file='CodeDoc/DockPaneStripBase.xml' path='//CodeDoc/Class[@name="DockPaneStripBase"]/Method[@name="GetOutlineXorPath(int)"]/*'/>
		protected internal abstract GraphicsPath GetOutlineXorPath(int index);

		/// <exclude/>
		protected override Size DefaultSize
		{
			get	{	return Size.Empty;	}
		}

		/// <exclude/>
		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)Win32.Msgs.WM_MOUSEACTIVATE)
			{
				DockPane.Activate();
				base.WndProc(ref m);
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDOWN)
			{
				int index = GetHitTest();
				if (index != -1)
				{
					DockContent content = Tabs[index].Content;
					if (DockPane.ActiveContent != content)
					{
						DockPane.ActiveContent = content;
						Update();
					}
					if (DockPane.DockPanel.AllowRedocking && DockPane.AllowRedocking && DockPane.ActiveContent.AllowRedocking)
						DockPane.DockPanel.DragHandler.BeginDragContent(DockPane, DockPane.ClientRectangle); //DockPane.DisplayingRectangle);
				}
				else
					base.WndProc(ref m);
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_MBUTTONUP)
			{
				base.WndProc(ref m);
				int index = GetHitTest();
				if (index != -1)
				{
					DockPane.CloseContent(Tabs[index].Content);
				}
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_RBUTTONDOWN)
			{
				int index = GetHitTest();
				if (index != -1)
				{
					DockContent content = Tabs[index].Content;
					if (DockPane.ActiveContent != content)
						DockPane.ActiveContent = content;
				}
				base.WndProc(ref m);
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_RBUTTONUP)
			{
				int index = GetHitTest();
				if (index != -1)
				{
					DockContent content = Tabs[index].Content;
					if (content.TabPageContextMenu != null) 
						content.TabPageContextMenu.Show(this, this.PointToClient(Control.MousePosition));
				}
				base.WndProc(ref m);
				return;
			}
			else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONDBLCLK)
			{
				base.WndProc(ref m);

				int index = GetHitTest();
				if (DockPane.DockPanel.AllowRedocking && index != -1)
				{
					DockContent content = Tabs[index].Content;
					try	{	content.IsFloat = !content.IsFloat;	}	
					catch	{	}
				}

				return;
			}

			base.WndProc(ref m);
			return;
		}
	}
}
