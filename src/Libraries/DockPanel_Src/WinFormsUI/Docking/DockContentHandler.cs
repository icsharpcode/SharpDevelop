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
using System.ComponentModel;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Delegate[@name="GetPersistStringDelegate"]/*'/>
	public delegate string GetPersistStringDelegate();

	/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/ClassDef/*'/>
	public class DockContentHandler : IDisposable
	{
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Constructor[@name="Overloads"]/*'/>
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Constructor[@name="(Form)"]/*'/>
		public DockContentHandler(Form form) : this(form, null)
		{
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Constructor[@name="(Form, GetPersistStringDelegate)"]/*'/>
		public DockContentHandler(Form form, GetPersistStringDelegate getPersistStringDelegate)
		{
			if (!(form is IDockContent))
				throw new ArgumentException();

			m_form = form;
			m_getPersistStringDelegate = getPersistStringDelegate;

			m_events = new EventHandlerList();
			Form.Disposed +=new EventHandler(Form_Disposed);
			Form.TextChanged += new EventHandler(Form_TextChanged);
		}

		/// <exclude />
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <exclude />
		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				lock(this)
				{
					DockPanel = null;
					if (AutoHideTab != null)
						AutoHideTab.Dispose();
					if (DockPaneTab != null)
						DockPaneTab.Dispose();

					Form.Disposed -= new EventHandler(Form_Disposed);
					Form.TextChanged -= new EventHandler(Form_TextChanged);
					Events.Dispose();
				}
			}
		}

		private Form m_form;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Form"]/*'/>
		public Form Form
		{
			get	{	return m_form;	}
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Form"]/*'/>
		public IDockContent Content
		{
			get	{	return Form as IDockContent;	}
		}

		private EventHandlerList m_events;
		private EventHandlerList Events
		{
			get	{	return m_events;	}
		}

		private bool m_allowRedocking = true;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="AllowRedocking"]/*'/>
		public bool AllowRedocking
		{
			get	{	return m_allowRedocking;	}
			set	{	m_allowRedocking = value;	}
		}

		private double m_autoHidePortion = 0.25;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="AutoHidePortion"]/*'/>
		public double AutoHidePortion
		{
			get	{	return m_autoHidePortion;	}
			set
			{
				if (value <= 0 || value > 1)
					throw(new ArgumentOutOfRangeException(ResourceHelper.GetString("IDockContent.AutoHidePortion.OutOfRange")));

				if (m_autoHidePortion == value)
					return;

				m_autoHidePortion = value;

				if (DockPanel == null)
					return;

				if (DockPanel.ActiveAutoHideContent == this.Form as IDockContent)
					DockPanel.PerformLayout();
			}
		}

		private bool m_closeButton = true;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="CloseButton"]/*'/>
		public bool CloseButton
		{
			get	{	return m_closeButton;	}
			set
			{
				if (m_closeButton == value)
					return;

				m_closeButton = value;
				if (Pane != null)
					if (Pane.ActiveContent == this)
						Pane.RefreshChanges();
			}
		}
		
		private DockState DefaultDockState
		{
			get
			{
				if (ShowHint != DockState.Unknown && ShowHint != DockState.Hidden)
					return ShowHint;

				if ((DockableAreas & DockAreas.Document) != 0)
					return DockState.Document;
				if ((DockableAreas & DockAreas.DockRight) != 0)
					return DockState.DockRight;
				if ((DockableAreas & DockAreas.DockLeft) != 0)
					return DockState.DockLeft;
				if ((DockableAreas & DockAreas.DockBottom) != 0)
					return DockState.DockBottom;
				if ((DockableAreas & DockAreas.DockTop) != 0)
					return DockState.DockTop;

				return DockState.Unknown;
			}
		}

		private DockState DefaultShowState
		{
			get
			{
				if (ShowHint != DockState.Unknown)
					return ShowHint;

				if ((DockableAreas & DockAreas.Document) != 0)
					return DockState.Document;
				if ((DockableAreas & DockAreas.DockRight) != 0)
					return DockState.DockRight;
				if ((DockableAreas & DockAreas.DockLeft) != 0)
					return DockState.DockLeft;
				if ((DockableAreas & DockAreas.DockBottom) != 0)
					return DockState.DockBottom;
				if ((DockableAreas & DockAreas.DockTop) != 0)
					return DockState.DockTop;
				if ((DockableAreas & DockAreas.Float) != 0)
					return DockState.Float;

				return DockState.Unknown;
			}
		}

		private DockAreas m_allowedAreas = DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.DockBottom | DockAreas.Document | DockAreas.Float;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="DockableAreas"]/*'/>
		public DockAreas DockableAreas
		{
			get	{	return m_allowedAreas;	}
			set
			{
				if (m_allowedAreas == value)
					return;

				if (!DockHelper.IsDockStateValid(DockState, value))
					throw(new InvalidOperationException(ResourceHelper.GetString("IDockContent.DockableAreas.InvalidValue")));

				m_allowedAreas = value;

				if (!DockHelper.IsDockStateValid(ShowHint, m_allowedAreas))
					ShowHint = DockState.Unknown;
			}
		}

		private DockState m_dockState = DockState.Unknown;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="DockState"]/*'/>
		public DockState DockState
		{
			get	{	return m_dockState;	}
			set
			{
				if (m_dockState == value)
					return;

				if (value == DockState.Hidden)
					IsHidden = true;
				else
					SetDockState(false, value, Pane);
			}
		}

		private DockPanel m_dockPanel = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="DockPanel"]/*'/>
		public DockPanel DockPanel
		{
			get { return m_dockPanel; }
			set
			{
				if (m_dockPanel == value)
					return;

				Pane = null;

				if (m_dockPanel != null)
					m_dockPanel.RemoveContent(Content);

				if (m_dockPaneTab != null)
				{
					m_dockPaneTab.Dispose();
					m_dockPaneTab = null;
				}

				if (m_autoHideTab != null)
				{
					m_autoHideTab.Dispose();
					m_autoHideTab = null;
				}

				m_dockPanel = value;

				if (m_dockPanel != null)
				{
					m_dockPanel.AddContent(Content);
					Form.TopLevel = false;
					Form.FormBorderStyle = FormBorderStyle.None;
					Form.ShowInTaskbar = false;
					User32.SetWindowPos(Form.Handle, IntPtr.Zero, 0, 0, 0, 0,
						Win32.FlagsSetWindowPos.SWP_NOACTIVATE |
						Win32.FlagsSetWindowPos.SWP_NOMOVE |
						Win32.FlagsSetWindowPos.SWP_NOSIZE |
						Win32.FlagsSetWindowPos.SWP_NOZORDER |
						Win32.FlagsSetWindowPos.SWP_NOOWNERZORDER |
						Win32.FlagsSetWindowPos.SWP_FRAMECHANGED);
					m_dockPaneTab = DockPanel.DockPaneTabFactory.CreateDockPaneTab(Content);
					m_autoHideTab = DockPanel.AutoHideTabFactory.CreateAutoHideTab(Content);
				}
			}
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Icon"]/*'/>
		public Icon Icon
		{
			get	{	return Form.Icon;	}
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="Pane"]/*'/>
		public DockPane Pane
		{
			get {	return IsFloat ? FloatPane : PanelPane; }
			set
			{
				if (Pane == value)
					return;

				DockPane oldPane = Pane;

				SuspendSetDockState();
				FloatPane = (value == null ? null : (value.IsFloat ? value : FloatPane));
				PanelPane = (value == null ? null : (value.IsFloat ? PanelPane : value));
				ResumeSetDockState(IsHidden, value != null ? value.DockState : DockState.Unknown, oldPane);
			}
		}

		private bool m_isHidden = true;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="IsHidden"]/*'/>
		public bool IsHidden
		{
			get	{	return m_isHidden;	}
			set
			{
				if (m_isHidden == value)
					return;

				SetDockState(value, VisibleState, Pane);
			}
		}

		private string m_tabText = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="TabText"]/*'/>
		public string TabText
		{
			get	{	return m_tabText==null ? Form.Text : m_tabText;	}
			set
			{
				if (m_tabText == value)
					return;

				m_tabText = value;
				if (Pane != null)
					Pane.RefreshChanges();
			}
		}

		private DockState m_visibleState = DockState.Unknown;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="VisibleState"]/*'/>
		public DockState VisibleState
		{
			get	{	return m_visibleState;	}
			set
			{
				if (m_visibleState == value)
					return;

				SetDockState(IsHidden, value, Pane);
			}
		}

		private bool m_isFloat = false;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="IsFloat"]/*'/>
		public bool IsFloat
		{
			get	{	return m_isFloat;	}
			set
			{
				DockState visibleState;

				if (m_isFloat == value)
					return;

				DockPane oldPane = Pane;

				if (value)
				{
					if (!IsDockStateValid(DockState.Float))
						throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.IsFloat.InvalidValue"));
					visibleState = DockState.Float;
				}
				else
					visibleState = (PanelPane != null) ? PanelPane.DockState : DefaultDockState;

				if (visibleState == DockState.Unknown)
					throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.IsFloat.InvalidValue"));

				SetDockState(IsHidden, visibleState, oldPane);
			}
		}

		private DockPane m_panelPane = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="PanelPane"]/*'/>
		public DockPane PanelPane
		{
			get	{	return m_panelPane;	}
			set
			{
				if (m_panelPane == value)
					return;

				if (value != null)
				{
					if (value.IsFloat || value.DockPanel != DockPanel)
						throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.DockPane.InvalidValue"));
				}

				DockPane oldPane = Pane;

				if (m_panelPane != null)
					m_panelPane.RemoveContent(Content);

				m_panelPane = value;
				if (m_panelPane != null)
				{
					m_panelPane.AddContent(Content);
					SetDockState(IsHidden, IsFloat ? DockState.Float : m_panelPane.DockState, oldPane);
				}
				else
					SetDockState(IsHidden, DockState.Unknown, oldPane);
			}
		}

		private DockPane m_floatPane = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="FloatPane"]/*'/>
		public DockPane FloatPane
		{
			get	{	return m_floatPane;	}
			set
			{
				if (m_floatPane == value)
					return;

				if (value != null)
				{
					if (!value.IsFloat || value.DockPanel != DockPanel)
						throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.FloatPane.InvalidValue"));
				}

				DockPane oldPane = Pane;

				if (m_floatPane != null)
					m_floatPane.RemoveContent(Content);

				m_floatPane = value;
				if (m_floatPane != null)
				{
					m_floatPane.AddContent(Content);
					SetDockState(IsHidden, IsFloat ? DockState.Float : VisibleState, oldPane);
				}
				else
					SetDockState(IsHidden, DockState.Unknown, oldPane);
			}
		}

		private int m_countSetDockState = 0;
		private void SuspendSetDockState()
		{
			m_countSetDockState ++;
		}

		private void ResumeSetDockState()
		{
			m_countSetDockState --;
			if (m_countSetDockState < 0)
				m_countSetDockState = 0;
		}

		internal bool IsSuspendSetDockState
		{
			get	{	return m_countSetDockState != 0;	}
		}

		private void ResumeSetDockState(bool isHidden, DockState visibleState, DockPane oldPane)
		{
			ResumeSetDockState();
			SetDockState(isHidden, visibleState, oldPane);
		}

		internal void SetDockState(bool isHidden, DockState visibleState, DockPane oldPane)
		{
			if (IsSuspendSetDockState)
				return;

			if (DockPanel == null && visibleState != DockState.Unknown)
				throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.SetDockState.NullPanel"));

			if (visibleState == DockState.Hidden || (visibleState != DockState.Unknown && !IsDockStateValid(visibleState)))
				throw new InvalidOperationException(ResourceHelper.GetString("IDockContent.SetDockState.InvalidState"));

			SuspendSetDockState();

			DockState oldDockState = DockState;

			if (m_isHidden != isHidden || oldDockState == DockState.Unknown)
			{
				m_isHidden = isHidden;
			}
			m_visibleState = visibleState;
			m_dockState = isHidden ? DockState.Hidden : visibleState;

			if (visibleState == DockState.Unknown)
				Pane = null;
			else
			{
				m_isFloat = (m_visibleState == DockState.Float);

				if (Pane == null)
					Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, visibleState, true);
				else if (Pane.DockState != visibleState)
				{
					if (Pane.Contents.Count == 1)
						Pane.SetDockState(visibleState);
					else
						Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, visibleState, true);
				}
			}

			SetPane(Pane);
			SetVisible();

			if (oldPane != null && !oldPane.IsDisposed && oldDockState == oldPane.DockState)
				RefreshDockPane(oldPane);

			if (Pane != null && DockState == Pane.DockState)
			{
				if ((Pane != oldPane) ||
					(Pane == oldPane && oldDockState != oldPane.DockState))
					RefreshDockPane(Pane);
			}

			if (oldDockState != DockState)
				OnDockStateChanged(EventArgs.Empty);

			ResumeSetDockState();
		}

		private void RefreshDockPane(DockPane pane)
		{
			pane.RefreshChanges();
			pane.ValidateActiveContent();
		}

		internal string PersistString
		{
			get	{	return GetPersistStringDelegate == null ? Form.GetType().ToString() : GetPersistStringDelegate();	}
		}

		private GetPersistStringDelegate m_getPersistStringDelegate = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="GetPersistStringDelegate"]/*'/>
		public GetPersistStringDelegate GetPersistStringDelegate
		{
			get	{	return m_getPersistStringDelegate;	}
			set	{	m_getPersistStringDelegate = value;	}
		}


		private bool m_hideOnClose = false;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="HideOnClose"]/*'/>
		public bool HideOnClose
		{
			get	{	return m_hideOnClose;	}
			set	{	m_hideOnClose = value;	}
		}

		private DockState m_showHint = DockState.Unknown;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="ShowHint"]/*'/>
		public DockState ShowHint
		{
			get	{	return m_showHint;	}
			set
			{	
				if (!DockHelper.IsDockStateValid(value, DockableAreas))
					throw (new InvalidOperationException(ResourceHelper.GetString("IDockContent.ShowHint.InvalidValue")));

				if (m_showHint == value)
					return;

				m_showHint = value;
			}
		}

		private bool m_isActivated = false;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="IsActivated"]/*'/>
		public bool IsActivated
		{
			get	{	return m_isActivated;	}
		}
		internal void SetIsActivated(bool value)
		{
			if (m_isActivated == value)
				return;

			m_isActivated = value;
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="IsDockStateValid(DockState)"]/*'/>
		public bool IsDockStateValid(DockState dockState)
		{
			if (DockPanel != null && dockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.SystemMdi)
				return false;
			else
				return DockHelper.IsDockStateValid(dockState, DockableAreas);
		}

		private ContextMenu m_tabPageContextMenu = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="TabPageContextMenu"]/*'/>
		public ContextMenu TabPageContextMenu
		{
			get	{	return m_tabPageContextMenu;	}
			set	{	m_tabPageContextMenu = value;	}
		}

		private string m_toolTipText = null;
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Property[@name="ToolTipText"]/*'/>
		public string ToolTipText
		{
			get	{	return m_toolTipText;	}
			set {	m_toolTipText = value;	}
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Activate()"]/*'/>
		public void Activate()
		{
			if (DockPanel == null)
				Form.Activate();
			else if (Pane == null)
				Show(DockPanel);
			else
			{
				IsHidden = false;
				Pane.ActiveContent = Content;
				if (DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
					Form.Activate();
				else if (!Form.ContainsFocus)
				{
					if (Contains(ActiveWindowHandle))
						User32.SetFocus(ActiveWindowHandle);

					if (!Form.ContainsFocus)
					{
						if (!Form.SelectNextControl(Form.ActiveControl, true, true, true, true))
							// Since DockContent Form is not selectalbe, use Win32 SetFocus instead
							User32.SetFocus(Form.Handle);
					}
				}
			}
		}

		private bool Contains(IntPtr hWnd)
		{
			Control control = Control.FromChildHandle(hWnd);
			for (Control parent=control; parent!=null; parent=parent.Parent)
				if (parent == Form)
					return true;

			return false;
		}

		private IntPtr m_activeWindowHandle = IntPtr.Zero;
		internal IntPtr ActiveWindowHandle
		{
			get	{	return m_activeWindowHandle;	}
			set	{	m_activeWindowHandle = value;	}
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Hide()"]/*'/>
		public void Hide()
		{
			IsHidden = true;
		}

		internal void SetPane(DockPane pane)
		{
			if (pane != null && pane.DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
			{
				if (Form.Parent is DockPane)
					SetParent(null);
				if (Form.MdiParent != DockPanel.ParentForm)
				{
					FlagClipWindow = true;
					Form.MdiParent = DockPanel.ParentForm;
				}
			}
			else
			{
				FlagClipWindow = true;
				if (Form.MdiParent != null)
					Form.MdiParent = null;
				if (Form.TopLevel)
					Form.TopLevel = false;
				SetParent(pane);
			}
		}

		internal void SetVisible()
		{
			bool visible;

			if (IsHidden)
				visible = false;
			else if (Pane != null && Pane.DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyles.DockingMdi)
				visible = true;
			else if (Pane != null && Pane.ActiveContent == Form as IDockContent)
				visible = true;
			else if (Pane != null && Pane.ActiveContent != Form as IDockContent)
				visible = false;
			else
				visible = Form.Visible;

			Form.Visible = visible;
		}

		private void SetParent(Control value)
		{
			if (Form.Parent == value)
				return;

			if (Environment.Version.Major > 1)
			{
				Form.Parent = value;
				return;
			}

			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// Workaround for .Net Framework bug: removing control from Form may cause form
			// unclosable. Set focus to another dummy control.
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Control oldParent = Form.Parent;

			Form form = null;
			if (Form.Parent is DockPane)
				form = ((DockPane)Form.Parent).FindForm();
			if (Form.ContainsFocus)
			{
				if (form is FloatWindow)
				{
					((FloatWindow)form).DummyControl.Focus();
					form.ActiveControl = ((FloatWindow)form).DummyControl;
				}
				else if (DockPanel != null)
				{
					DockPanel.DummyControl.Focus();
					if (form != null)
						form.ActiveControl = DockPanel.DummyControl;
				}
			}
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

			Form.Parent = value;
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show"]/*'/>
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show()"]/*'/>
		public void Show()
		{
			if (DockPanel == null)
				Form.Show();
			else
				Show(DockPanel);
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPanel)"]/*'/>
		public void Show(DockPanel dockPanel)
		{
			if (dockPanel == null)
				throw(new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullDockPanel")));

			if (DockState == DockState.Unknown)
				Show(dockPanel, DefaultShowState);
			else			
				Activate();
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPanel, DockState)"]/*'/>
		public void Show(DockPanel dockPanel, DockState dockState)
		{
			if (dockPanel == null)
				throw(new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullDockPanel")));

			if (dockState == DockState.Unknown || dockState == DockState.Hidden)
				throw(new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidDockState")));

			DockPanel = dockPanel;

			if (dockState == DockState.Float && FloatPane == null)
				Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, DockState.Float, true);
			else if (PanelPane == null)
			{
				DockPane paneExisting = null;
				foreach (DockPane pane in DockPanel.Panes)
					if (pane.DockState == dockState)
					{
						paneExisting = pane;
						break;
					}

				if (paneExisting == null)
					Pane = DockPanel.DockPaneFactory.CreateDockPane(Content, dockState, true);
				else
					Pane = paneExisting;
			}

			DockState = dockState;
			Activate();
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPanel, Rectangle)"]/*'/>
		public void Show(DockPanel dockPanel, Rectangle floatWindowBounds)
		{
			if (dockPanel == null)
				throw(new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullDockPanel")));

			DockPanel = dockPanel;
			if (FloatPane == null)
			{
				IsHidden = true;	// to reduce the screen flicker
				FloatPane = DockPanel.DockPaneFactory.CreateDockPane(Content, DockState.Float, false);
				FloatPane.FloatWindow.StartPosition = FormStartPosition.Manual;
			}

			FloatPane.FloatWindow.Bounds = floatWindowBounds;
			
			Show(dockPanel, DockState.Float);
			Activate();
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPane, IDockContent)"]/*'/>
		public void Show(DockPane pane, IDockContent beforeContent)
		{
			if (pane == null)
				throw(new ArgumentNullException(ResourceHelper.GetString("IDockContent.Show.NullPane")));

			if (beforeContent != null && pane.Contents.IndexOf(beforeContent) == -1)
				throw(new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidBeforeContent")));

			DockPanel = pane.DockPanel;
			Pane = pane;
			pane.SetContentIndex(Content, pane.Contents.IndexOf(beforeContent));
			Show();
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Show(DockPane, DockAlignment, double)"]/*'/>
		public void Show(DockPane prevPane, DockAlignment alignment, double proportion)
		{
			if (prevPane == null)
				throw(new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidPrevPane")));

			if (DockHelper.IsDockStateAutoHide(prevPane.DockState))
				throw(new ArgumentException(ResourceHelper.GetString("IDockContent.Show.InvalidPrevPane")));

			DockPanel = prevPane.DockPanel;
			DockPanel.DockPaneFactory.CreateDockPane(Content, prevPane, alignment, proportion, true);
			Show();
		}

		internal void SetBounds(Rectangle bounds)
		{
			Form.Bounds = bounds;
		}

		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="Close()"]/*'/>
		public void Close()
		{
			DockPanel dockPanel = DockPanel;
			if (dockPanel != null)
				dockPanel.SuspendLayout(true);
			Form.Close();
			if (dockPanel != null)
				dockPanel.ResumeLayout(true, true);
		}

		private DockPaneTab m_dockPaneTab = null;
		internal DockPaneTab DockPaneTab
		{
			get	{	return m_dockPaneTab;	}
		}

		private AutoHideTab m_autoHideTab = null;
		internal AutoHideTab AutoHideTab
		{
			get	{	return m_autoHideTab;	}
		}

		#region Events
		private static readonly object DockStateChangedEvent = new object();
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Event[@name="DockStateChanged"]/*'/>
		public event EventHandler DockStateChanged
		{
			add	{	Events.AddHandler(DockStateChangedEvent, value);	}
			remove	{	Events.RemoveHandler(DockStateChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockContentHandler.xml' path='//CodeDoc/Class[@name="DockContentHandler"]/Method[@name="OnDockStateChanged(EventArgs)"]/*'/>
		protected virtual void OnDockStateChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[DockStateChangedEvent];
			if (handler != null)
				handler(this, e);
		}
		#endregion

		private void Form_Disposed(object sender, EventArgs e)
		{
			Dispose();
		}

		private void Form_TextChanged(object sender, EventArgs e)
		{
			if (DockHelper.IsDockStateAutoHide(DockState))
				DockPanel.RefreshAutoHideStrip();
			else if (Pane != null)
			{
				if (Pane.FloatWindow != null)
					Pane.FloatWindow.SetText();
				Pane.RefreshChanges();
			}
		}

		private bool m_flagClipWindow = false;
		internal bool FlagClipWindow
		{
			get	{	return m_flagClipWindow;	}
			set
			{
				if (m_flagClipWindow == value)
					return;

				m_flagClipWindow = value;
				if (m_flagClipWindow)
					Form.Region = new Region(Rectangle.Empty);
				else
					Form.Region = null;
			}
		}

        #if FRAMEWORK_VER_2x
        private ContextMenuStrip m_tabPageContextMenuStrip = null;
        public ContextMenuStrip TabPageContextMenuStrip
        {
            get { return m_tabPageContextMenuStrip; }
            set { m_tabPageContextMenuStrip = value; }
        }
        #endif
	}
}
