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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WeifenLuo.WinFormsUI
{
	/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/ClassDef/*'/>
	public class DockPane : UserControl
	{
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Enum[@name="AppearanceStyle"]/EnumDef/*'/>
		public enum AppearanceStyle
		{
			/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Enum[@name="AppearanceStyle"]/Member[@name="ToolWindow"]/*'/>
			ToolWindow,
			/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Enum[@name="AppearanceStyle"]/Member[@name="Document"]/*'/>
			Document
		}

		private enum HitTestArea
		{
			Caption,
			TabStrip,
			Content,
			None
		}

		private struct HitTestResult
		{
			public HitTestArea HitArea;
			public int Index;

			public HitTestResult(HitTestArea hitTestArea)
			{
				HitArea = hitTestArea;
				Index = -1;
			}

			public HitTestResult(HitTestArea hitTestArea, int index)
			{
				HitArea = hitTestArea;
				Index = index;
			}
		}

		private DockPaneCaptionBase m_captionControl;
		private DockPaneCaptionBase CaptionControl
		{
			get	{	return m_captionControl;	}
		}

		private DockPaneStripBase m_tabStripControl;
		private DockPaneStripBase TabStripControl
		{
			get	{	return m_tabStripControl;	}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="Overloads"]/*'/>
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(DockContent, DockState, bool)"]/*'/>
		public DockPane(DockContent content, DockState visibleState, bool show)
		{
			InternalConstruct(content, visibleState, false, Rectangle.Empty, null, DockAlignment.Right, 0.5, show);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(DockContent, FloatWindow, bool)"]/*'/>
		public DockPane(DockContent content, FloatWindow floatWindow, bool show)
		{
			InternalConstruct(content, DockState.Float, false, Rectangle.Empty, floatWindow.DockList.GetDefaultPrevPane(this), DockAlignment.Right, 0.5, show);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(DockContent, DockPane, DockAlignment, double, bool)"]/*'/>
		public DockPane(DockContent content, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
		{
			if (prevPane == null)
				throw(new ArgumentNullException());
			InternalConstruct(content, prevPane.DockState, false, Rectangle.Empty, prevPane, alignment, proportion, show);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Constructor[@name="(DockContent, Rectangle, bool)"]/*'/>
		public DockPane(DockContent content, Rectangle floatWindowBounds, bool show)
		{
			InternalConstruct(content, DockState.Float, true, floatWindowBounds, null, DockAlignment.Right, 0.5, show);
		}

		private void InternalConstruct(DockContent content, DockState dockState, bool flagBounds, Rectangle floatWindowBounds, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
		{
			if (dockState == DockState.Hidden || dockState == DockState.Unknown)
				throw new ArgumentException(ResourceHelper.GetString("DockPane.DockState.InvalidState"));

			if (content == null)
				throw new ArgumentNullException(ResourceHelper.GetString("DockPane.Constructor.NullContent"));

			if (content.DockPanel == null)
				throw new ArgumentException(ResourceHelper.GetString("DockPane.Constructor.NullDockPanel"));

			SuspendLayout();
			SetStyle(ControlStyles.ResizeRedraw, true);
			SetStyle(ControlStyles.UserPaint, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			SetStyle(ControlStyles.Selectable, true);

			m_isFloat = (dockState == DockState.Float);

			m_contents = new DockContentCollection();
			m_displayingContents = new DockContentCollection(this);
			m_tabs = new DockPaneTabCollection(this);
			m_dockPanel = content.DockPanel;
			m_dockPanel.AddPane(this);

			m_splitter = new DockPaneSplitter(this);

			m_nestedDockingStatus = new NestedDockingStatus(this);

			m_autoHidePane = DockPanel.AutoHidePaneFactory.CreateAutoHidePane(this);
			m_captionControl = DockPanel.DockPaneCaptionFactory.CreateDockPaneCaption(this);
			m_tabStripControl = DockPanel.DockPaneStripFactory.CreateDockPaneStrip(this);
			Controls.AddRange(new Control[] {	m_captionControl, m_tabStripControl	});
			
			if (flagBounds)
				FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this, floatWindowBounds);
			else if (prevPane != null)
				AddToDockList(prevPane.DockListContainer, prevPane, alignment, proportion);

			SetDockState(dockState);
			if (show)
				content.Pane = this;
			else if (this.IsFloat)
				content.FloatPane = this;
			else
				content.PanelPane = this;

			ResumeLayout();
		}

		private void Close_Click(object sender, EventArgs e)
		{
			CloseActiveContent();
			if (!DockHelper.IsDockStateAutoHide(DockState) && ActiveContent != null)
				ActiveContent.Activate();
		}

		/// <exclude/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				m_dockState = DockState.Unknown;

				if (DockListContainer != null)
					DockListContainer.DockList.Remove(this);

				if (DockPanel != null)
				{
					DockPanel.RemovePane(this);
					m_dockPanel = null;
				}

				Splitter.Dispose();
				AutoHidePane.Dispose();
			}
			base.Dispose(disposing);
		}

		private Stack<DockContent> m_visitedTabs = new Stack<DockContent>();
		private DockContent m_activeContent = null;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="ActiveContent"]/*'/>
		public virtual DockContent ActiveContent
		{
			get	{	return m_activeContent;	}
			set
			{
				if (ActiveContent == value)
					return;

				if (value != null)
				{
					if (!DisplayingContents.Contains(value))
						throw(new InvalidOperationException(ResourceHelper.GetString("DockPane.ActiveContent.InvalidValue")));
				}
				else
				{
					if (DisplayingContents.Count != 0)
						throw(new InvalidOperationException(ResourceHelper.GetString("DockPane.ActiveContent.InvalidValue")));
				}

				DockContent oldValue = m_activeContent;

				if (DockPanel.ActiveAutoHideContent == oldValue)
					DockPanel.ActiveAutoHideContent = null;

				m_activeContent = value;
				
				if (m_activeContent != null && m_activeContent.DockState == DockState.Document)
				{
					if (!(m_visitedTabs.Count > 0 && m_visitedTabs.Peek() == m_activeContent))
					{
						m_visitedTabs.Push(m_activeContent);
					}
				}
				
				if (FloatWindow != null)
					FloatWindow.SetText();

				RefreshChanges();
				PerformLayout();
				DockPanel.RefreshActiveWindow();

				if (m_activeContent != null)
					TabStripControl.EnsureTabVisible(m_activeContent);
			}
		}
		
		private bool m_allowRedocking = true;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="AllowRedocking"]/*'/>
		public virtual bool AllowRedocking
		{
			get	{	return m_allowRedocking;	}
			set	{	m_allowRedocking = value;	}
		}

		private DockPaneTabCollection m_tabs;
		internal DockPaneTabCollection Tabs
		{
			get	{	return m_tabs;	}
		}

		private Rectangle CaptionRectangle
		{
			get
			{
				if (!HasCaption)
					return Rectangle.Empty;

				Rectangle rectWindow = DisplayingRectangle;
				int x, y, width;
				x = rectWindow.X;
				y = rectWindow.Y;
				width = rectWindow.Width;
				int height = CaptionControl.MeasureHeight();

				return new Rectangle(x, y, width, height);
			}
		}

		private Rectangle ContentRectangle
		{
			get
			{
				Rectangle rectWindow = DisplayingRectangle;
				Rectangle rectCaption = CaptionRectangle;
				Rectangle rectTabStrip = TabStripRectangle;

				int x = rectWindow.X;
				int y = rectWindow.Y + (rectCaption.IsEmpty ? 0 : rectCaption.Height) +
					(DockState == DockState.Document ? rectTabStrip.Height : 0);
				int width = rectWindow.Width;
				int height = rectWindow.Height - rectCaption.Height - rectTabStrip.Height;

				return new Rectangle(x, y, width, height);
			}
		}

		private Rectangle TabStripRectangle
		{
			get
			{
				if (Appearance == AppearanceStyle.ToolWindow)
					return TabStripRectangle_ToolWindow;
				else
					return TabStripRectangle_Document;
			}
		}

		private Rectangle TabStripRectangle_ToolWindow
		{
			get
			{
				if (DisplayingContents.Count <= 1 || IsAutoHide)
					return Rectangle.Empty;

				Rectangle rectWindow = DisplayingRectangle;

				int width = rectWindow.Width;
				int height = TabStripControl.MeasureHeight();
				int x = rectWindow.X;
				int y = rectWindow.Bottom - height;
				Rectangle rectCaption = CaptionRectangle;
				if (rectCaption.Contains(x, y))
					y = rectCaption.Y + rectCaption.Height;

				return new Rectangle(x, y, width, height);
			}
		}

		private Rectangle TabStripRectangle_Document
		{
			get
			{
				if (DisplayingContents.Count == 0)
					return Rectangle.Empty;

				if (DisplayingContents.Count == 1 && DockPanel.SdiDocument)
					return Rectangle.Empty;

				Rectangle rectWindow = DisplayingRectangle;
				int x = rectWindow.X;
				int y = rectWindow.Y;
				int width = rectWindow.Width;
				int height = TabStripControl.MeasureHeight();

				return new Rectangle(x, y, width, height);
			}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="CaptionText"]/*'/>
		public virtual string CaptionText
		{
			get	{	return ActiveContent == null ? string.Empty : ActiveContent.Text;	}
		}

		private DockContentCollection m_contents;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="Contents"]/*'/>
		public DockContentCollection Contents
		{
			get	{	return m_contents;	}
		}

		private DockContentCollection m_displayingContents;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DisplayingContents"]/*'/>
		public DockContentCollection DisplayingContents
		{
			get	{	return m_displayingContents;	}
		}

		/// <exclude/>
		protected override Size DefaultSize
		{
			// set the default size to empty to reduce screen flickers
			get	{	return Size.Empty;	}
		}

		private DockPanel m_dockPanel;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockPanel"]/*'/>
		public DockPanel DockPanel
		{
			get	{	return m_dockPanel;	}
		}

		private bool HasCaption
		{
			get
			{	
				if (DockState == DockState.Document ||
					DockState == DockState.Hidden ||
					DockState == DockState.Unknown ||
					(DockState == DockState.Float && FloatWindow.DisplayingList.Count <= 1))
					return false;
				else
					return true;
			}
		}

		private bool m_isActivated = false;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsActivated"]/*'/>
		public bool IsActivated
		{
			get	{	return m_isActivated;	}
		}
		internal void SetIsActivated(bool value)
		{
			if (m_isActivated == value)
				return;

			m_isActivated = value;
			if (DockState != DockState.Document)
				RefreshChanges();
			OnIsActivatedChanged(EventArgs.Empty);
		}

		private bool m_isActiveDocumentPane = false;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsActiveDocumentPane"]/*'/>
		public bool IsActiveDocumentPane
		{
			get	{	return m_isActiveDocumentPane;	}
		}
		internal void SetIsActiveDocumentPane(bool value)
		{
			if (m_isActiveDocumentPane == value)
				return;

			m_isActiveDocumentPane = value;
			if (DockState == DockState.Document)
				RefreshChanges();
			OnIsActiveDocumentPaneChanged(EventArgs.Empty);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="IsDockStateValid(DockState)"]/*'/>
		public bool IsDockStateValid(DockState dockState)
		{
			foreach (DockContent content in Contents)
				if (!content.IsDockStateValid(dockState))
					return false;

			return true;
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsActiveDocumentPane"]/*'/>
		public bool IsAutoHide
		{
			get	{	return DockHelper.IsDockStateAutoHide(DockState);	}
		}

		private DockPaneSplitter m_splitter;
		internal DockPaneSplitter Splitter
		{
			get	{	return m_splitter;	}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="Appearance"]/*'/>
		public AppearanceStyle Appearance
		{
			get	{	return (DockState == DockState.Document) ? AppearanceStyle.Document : AppearanceStyle.ToolWindow;	}
		}

		private Rectangle DisplayingRectangle
		{
			get	{	return ClientRectangle;	}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="Activate()"]/*'/>
		public void Activate()
		{
			if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel.ActiveAutoHideContent != ActiveContent)
				DockPanel.ActiveAutoHideContent = ActiveContent;
				
			if (!IsActivated)
				Focus();
		}

		internal void AddContent(DockContent content)
		{
			if (Contents.Contains(content))
				return;

			Contents.Add(content);
		}

		internal void Close()
		{
			Dispose();
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="CloseActiveContent()"]/*'/>
		public void CloseActiveContent()
		{
			CloseContent(ActiveContent);
		}

		internal void CloseContent(DockContent content)
		{
			if (content == null)
				return;

			if (!content.CloseButton)
				return;

			if (content.HideOnClose)
				content.Hide();
			else
			{
				//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				// Workaround for .Net Framework bug: removing control from Form may cause form
				// unclosable.
				//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
				Form form = FindForm();
				if (ContainsFocus)
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
				
				content.Close();
			}
		}

		private HitTestResult GetHitTest()
		{
			return GetHitTest(Control.MousePosition);
		}

		private HitTestResult GetHitTest(Point ptMouse)
		{
			HitTestResult hitTestResult = new HitTestResult(HitTestArea.None, -1);

			Point ptMouseClient = PointToClient(ptMouse);

			Rectangle rectCaption = CaptionRectangle;
			if (rectCaption.Contains(ptMouseClient))
				return new HitTestResult(HitTestArea.Caption, -1);

			Rectangle rectContent = ContentRectangle;
			if (rectContent.Contains(ptMouseClient))
				return new HitTestResult(HitTestArea.Content, -1);

			Rectangle rectTabStrip = TabStripRectangle;
			if (rectTabStrip.Contains(ptMouseClient))
				return new HitTestResult(HitTestArea.TabStrip, TabStripControl.GetHitTest(TabStripControl.PointToClient(ptMouse)));

			return new HitTestResult(HitTestArea.None, -1);
		}

		private Region GetTestDropOutline(DockStyle dockStyle, int contentIndex)
		{
			int dragSize = MeasureOutline.Width;

			if (dockStyle != DockStyle.Fill)
			{
				Rectangle rect = DisplayingRectangle;
				if (dockStyle == DockStyle.Right)
					rect.X += rect.Width / 2;
				if (dockStyle == DockStyle.Bottom)
					rect.Y += rect.Height / 2;
				if (dockStyle == DockStyle.Left || dockStyle == DockStyle.Right)
					rect.Width -= rect.Width / 2;
				if (dockStyle == DockStyle.Top || dockStyle == DockStyle.Bottom)
					rect.Height -= rect.Height / 2;
				rect.Location = PointToScreen(rect.Location);

				return DrawHelper.CreateDragOutline(rect, dragSize);
			}
			else if (contentIndex == -1)
			{
				Rectangle rect = DisplayingRectangle;
				rect.Location = PointToScreen(rect.Location);
				return DrawHelper.CreateDragOutline(rect, dragSize);
			}
			else
			{
				Rectangle rect = DisplayingRectangle;
				rect.Location = PointToScreen(rect.Location);
				Region region = DrawHelper.CreateDragOutline(rect, dragSize);
				Rectangle rectTabStrip = TabStripRectangle;
				rectTabStrip.Location = PointToScreen(rectTabStrip.Location);
				region.Union(rectTabStrip);
				region.Xor(TabStripControl.GetOutlineXorPath(contentIndex));

				return region;
			}
		}

		private bool m_isHidden = true;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsHidden"]/*'/>
		public bool IsHidden
		{
			get	{	return m_isHidden;	}
		}
		private void SetIsHidden(bool value)
		{
			if (m_isHidden == value)
				return;

			m_isHidden = value;
			if (DockHelper.IsDockStateAutoHide(DockState))
			{
				DockPanel.RefreshAutoHideStrip();
				DockPanel.PerformLayout();
			}
			else if (DockListContainer != null)
				((Control)DockListContainer).PerformLayout();
		}

		/// <exclude/>
		protected override void OnLayout(LayoutEventArgs e)
		{
			SetIsHidden(DisplayingContents.Count == 0);
			if (!IsHidden)
			{
				CaptionControl.Bounds = CaptionRectangle;
				TabStripControl.Bounds = TabStripRectangle;

				Rectangle rectContent = ContentRectangle;

				// Need to set the visible content first, otherwise keyboard focus will be lost
				if (ActiveContent != null)
				{
					ActiveContent.Bounds = rectContent;
					ActiveContent.BringToFront();
					ActiveContent.Visible = true;
				}

				// Hide all inactive contents
				foreach (DockContent content in Contents)
				{
					if (content.Pane == this && content != ActiveContent)
					{
						content.Visible = false;
						content.Bounds = Rectangle.Empty;
					}
				}
			}

			base.OnLayout(e);
		}

		internal void RefreshChanges()
		{
			CaptionControl.RefreshChanges();
			TabStripControl.RefreshChanges();
			if (DockState == DockState.Float)
				FloatWindow.RefreshChanges();
			if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel != null)
			{
				DockPanel.RefreshAutoHideStrip();
				DockPanel.PerformLayout();
			}

			PerformLayout();
		}

		internal void RemoveContent(DockContent content)
		{
			if (!Contents.Contains(content))
				return;
			
			Contents.Remove(content);
			if (content.Parent == this)
				content.SetParent(null);
			if (Contents.Count == 0)
				Dispose();
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="SetContentIndex(DockContent, int)"]/*'/>
		public void SetContentIndex(DockContent content, int index)
		{
			int oldIndex = Contents.IndexOf(content);
			if (oldIndex == -1)
				throw(new ArgumentException(ResourceHelper.GetString("DockPane.SetContentIndex.InvalidContent")));

			if (index < 0 || index > Contents.Count - 1)
				if (index != -1)
					throw(new ArgumentOutOfRangeException(ResourceHelper.GetString("DockPane.SetContentIndex.InvalidIndex")));
				
			if (oldIndex == index)
				return;
			if (oldIndex == Contents.Count - 1 && index == -1)
				return;

			Contents.Remove(content);
			if (index == -1)
				Contents.Add(content);
			else if (oldIndex < index)
				Contents.AddAt(content, index - 1);
			else
				Contents.AddAt(content, index);

			RefreshChanges();
		}

		private void SetParent()
		{
			if (DockState == DockState.Unknown || DockState == DockState.Hidden)
			{
				SetParent(DockPanel.DummyControl);
				Splitter.Parent = DockPanel.DummyControl;
			}
			else if (DockState == DockState.Float)
			{
				SetParent(FloatWindow);
				Splitter.Parent = FloatWindow;
			}
			else if (DockHelper.IsDockStateAutoHide(DockState))
			{
				SetParent(DockPanel.AutoHideWindow);
				Splitter.Parent = DockPanel.DummyControl;
			}
			else
			{
				SetParent(DockPanel.DockWindows[DockState]);
				Splitter.Parent = Parent;
			}
		}

		private void SetParent(Control value)
		{
			if (Parent == value)
				return;

			Control oldParent = Parent;

			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			// Workaround for .Net Framework bug: removing control from Form may cause form
			// unclosable. Set focus to another dummy control.
			//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
			Form form = FindForm();
			if (ContainsFocus)
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

			Parent = value;
			m_splitter.Parent = value;
		}

		/// <exclude/>
		public new void Show()
		{
			Activate();
		}

		internal void TestDrop(DragHandler dragHandler, Point pt)
		{
			if (DockState == DockState.Document)
				DockPanel.TestDrop(dragHandler, pt);

			if (dragHandler.DropTarget.DropTo != null)
				return;

			if (DockHelper.IsDockStateAutoHide(DockState))
				return;

			if (!dragHandler.IsDockStateValid(DockState))
				return;

			if (dragHandler.DragSource == DragSource.FloatWindow &&
				FloatWindow == dragHandler.DragControl)
				return;

			if (dragHandler.DragSource == DragSource.Pane &&
				dragHandler.DragControl == this)
				return;

			if (dragHandler.DragSource == DragSource.Content && 
				dragHandler.DragControl == this &&
				DockState == DockState.Document &&
				DisplayingContents.Count == 1)
				return;

			Point ptClient = PointToClient(pt);
			Rectangle rectPane = DisplayingRectangle;
			int dragSize = MeasurePane.DragSize;
			if (ptClient.Y - rectPane.Top >= 0 && ptClient.Y - rectPane.Top < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Top);
			else if (rectPane.Bottom - ptClient.Y >= 0 && rectPane.Bottom - ptClient.Y < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Bottom);
			else if (rectPane.Right - ptClient.X >= 0 && rectPane.Right - ptClient.X < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Right);
			else if (ptClient.X - rectPane.Left >= 0 && ptClient.X - rectPane.Left < dragSize)
				dragHandler.DropTarget.SetDropTarget(this, DockStyle.Left);
			else
			{
				if (rectPane.Height <= TabStripRectangle.Height)
					return;

				HitTestResult hitTestResult = GetHitTest(pt);
				if (hitTestResult.HitArea == HitTestArea.Caption)
					dragHandler.DropTarget.SetDropTarget(this, -1);
				else if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
					dragHandler.DropTarget.SetDropTarget(this, hitTestResult.Index);
				else if (DockState == DockState.Float && !HasCaption &&
					((ptClient.Y - rectPane.Top >= dragSize && ptClient.Y - rectPane.Top < 2 * dragSize) ||
					(rectPane.Bottom - ptClient.Y >= dragSize && rectPane.Bottom - ptClient.Y < 2 * dragSize) ||
					(rectPane.Right - ptClient.X >= dragSize && rectPane.Right - ptClient.X < 2 * dragSize) ||
					(ptClient.X - rectPane.Left >= dragSize && ptClient.X - rectPane.Left < 2 * dragSize)))
					dragHandler.DropTarget.SetDropTarget(this, -1);
				else
					return;
			}

			if (dragHandler.DropTarget.SameAsOldValue)
				return;

			dragHandler.DragOutline = GetTestDropOutline(dragHandler.DropTarget.Dock, dragHandler.DropTarget.ContentIndex);
		}

		internal void ValidateActiveContent()
		{
			if (ActiveContent == null)
			{
				if (DisplayingContents.Count != 0)
					ActiveContent = DisplayingContents[0];
				return;
			}

			if (DisplayingContents.IndexOf(ActiveContent) >= 0)
				return;

			DockContent prevVisible = null;
			
			if (ActiveContent.DockState == DockState.Unknown)
			{
				while(m_visitedTabs.Count > 0)
				{
					prevVisible = m_visitedTabs.Pop();
					
					if (prevVisible != null && !prevVisible.IsDisposed && prevVisible != ActiveContent)
						break;
				}
			}
			
			if (prevVisible != null)
			{
				ActiveContent = prevVisible;
			}
			else
			{
				for (int i=Contents.IndexOf(ActiveContent)-1; i>=0; i--)
					if (Contents[i].DockState == DockState)
					{
						prevVisible = Contents[i];
						break;
					}
				
				DockContent nextVisible = null;
				for (int i=Contents.IndexOf(ActiveContent)+1; i<Contents.Count; i++)
					if (Contents[i].DockState == DockState)
					{
						nextVisible = Contents[i];
						break;
					}
			
	 			if (prevVisible != null)
	 				ActiveContent = prevVisible;
				else if (nextVisible != null)
					ActiveContent = nextVisible;
	 			else
	 				ActiveContent = null;
			}
		}

		private static readonly object DockStateChangedEvent = new object();
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Event[@name="DockStateChanged"]/*'/>
		public event EventHandler DockStateChanged
		{
			add	{	Events.AddHandler(DockStateChangedEvent, value);	}
			remove	{	Events.RemoveHandler(DockStateChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="OnDockStateChanged(EventArgs)"]/*'/>
		protected virtual void OnDockStateChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[DockStateChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object IsActivatedChangedEvent = new object();
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Event[@name="IsActivatedChanged"]/*'/>
		public event EventHandler IsActivatedChanged
		{
			add	{	Events.AddHandler(IsActivatedChangedEvent, value);	}
			remove	{	Events.RemoveHandler(IsActivatedChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="OnIsActivatedChanged(EventArgs)"]/*'/>
		protected virtual void OnIsActivatedChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[IsActivatedChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		private static readonly object IsActiveDocumentPaneChangedEvent = new object();
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Event[@name="IsActiveDocumentPaneChanged"]/*'/>
		public event EventHandler IsActiveDocumentPaneChanged
		{
			add	{	Events.AddHandler(IsActiveDocumentPaneChangedEvent, value);	}
			remove	{	Events.RemoveHandler(IsActiveDocumentPaneChangedEvent, value);	}
		}
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="OnIsActiveDocumentPaneChanged(EventArgs)"]/*'/>
		protected virtual void OnIsActiveDocumentPaneChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)Events[IsActiveDocumentPaneChangedEvent];
			if (handler != null)
				handler(this, e);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockWindow"]/*'/>
		public DockWindow DockWindow
		{
			get	{	return (m_nestedDockingStatus.DockList == null) ? null : m_nestedDockingStatus.DockList.Container as DockWindow;	}
			set
			{
				DockWindow oldValue = DockWindow;
				if (oldValue == value)
					return;

				AddToDockList(value);
			}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="FloatWindow"]/*'/>
		public FloatWindow FloatWindow
		{
			get	{	return (m_nestedDockingStatus.DockList == null) ? null : m_nestedDockingStatus.DockList.Container as FloatWindow;	}
			set
			{
				FloatWindow oldValue = FloatWindow;
				if (oldValue == value)
					return;

				AddToDockList(value);
			}
		}

		private NestedDockingStatus m_nestedDockingStatus;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="NestedDockingStatus"]/*'/>
		public NestedDockingStatus NestedDockingStatus
		{
			get	{	return m_nestedDockingStatus;	}
		}
	
		private bool m_isFloat;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="IsFloat"]/*'/>
		public bool IsFloat
		{
			get	{	return m_isFloat;	}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockListContainer"]/*'/>
		public IDockListContainer DockListContainer
		{
			get
			{
				if (NestedDockingStatus.DockList == null)
					return null;
				else
					return NestedDockingStatus.DockList.Container;
			}
		}

		private DockState m_dockState = DockState.Unknown;
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Property[@name="DockState"]/*'/>
		public DockState DockState
		{
			get	{	return m_dockState;	}
			set
			{
				SetDockState(value);
			}
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="SetDockState(DockState)"]/*'/>
		public DockPane SetDockState(DockState value)
		{
			if (value == DockState.Unknown || value == DockState.Hidden)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.SetDockState.InvalidState"));

			if ((value == DockState.Float) == this.IsFloat)
			{
				InternalSetDockState(value);
				return this;
			}

			if (DisplayingContents.Count == 0)
				return null;

			DockContent firstContent = null;
			for (int i=0; i<DisplayingContents.Count; i++)
			{
				DockContent content = DisplayingContents[i];
				if (content.IsDockStateValid(value))
				{
					firstContent = content;
					break;
				}
			}
			if (firstContent == null)
				return null;

			firstContent.DockState = value;
			DockPane pane = firstContent.Pane;
			for (int i=0; i<DisplayingContents.Count; i++)
			{
				DockContent content = DisplayingContents[i];
				if (content.IsDockStateValid(value))
					content.Pane = pane;
			}
			return pane;
		}

		private void InternalSetDockState(DockState value)
		{
			if (m_dockState == value)
				return;

			DockState oldDockState = m_dockState;
			IDockListContainer oldContainer = DockListContainer;

			m_dockState = value;

			SuspendRefreshStateChange();
			if (!IsFloat)
				DockWindow = DockPanel.DockWindows[DockState];
			else if (FloatWindow == null)
				FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this);

			ResumeRefreshStateChange(oldContainer, oldDockState);
		}

		private int m_countRefreshStateChange = 0;
		private void SuspendRefreshStateChange()
		{
			m_countRefreshStateChange ++;
		}

		private void ResumeRefreshStateChange()
		{
			m_countRefreshStateChange --;
			if (m_countRefreshStateChange < 0)
				m_countRefreshStateChange = 0;
		}

		private void ResumeRefreshStateChange(IDockListContainer oldContainer, DockState oldDockState)
		{
			ResumeRefreshStateChange();
			RefreshStateChange(oldContainer, oldDockState);
		}

		private void RefreshStateChange(IDockListContainer oldContainer, DockState oldDockState)
		{
			if (m_countRefreshStateChange != 0)
				return;

			SuspendRefreshStateChange();

			SetParent();

			if (ActiveContent != null)
				ActiveContent.SetDockState(ActiveContent.IsHidden, DockState, ActiveContent.Pane);
			foreach (DockContent content in Contents)
			{
				if (content.Pane == this)
					content.SetDockState(content.IsHidden, DockState, content.Pane);
			}

			if (oldContainer != null)
			{
				if (oldContainer.DockState == oldDockState && !oldContainer.IsDisposed)
					((Control)oldContainer).PerformLayout();
			}
			if (DockHelper.IsDockStateAutoHide(oldDockState))
				DockPanel.AutoHideWindow.RefreshActiveContent();

			if (DockListContainer.DockState == DockState)
				((Control)DockListContainer).PerformLayout();
			if (DockHelper.IsDockStateAutoHide(DockState))
				DockPanel.AutoHideWindow.RefreshActiveContent();

			if (DockHelper.IsDockStateAutoHide(oldDockState) ||
				DockHelper.IsDockStateAutoHide(DockState))
			{
				DockPanel.RefreshAutoHideStrip();
				DockPanel.PerformLayout();
			}

			DockPanel.RefreshActiveWindow();
			if (oldDockState != DockState)
				OnDockStateChanged(EventArgs.Empty);

			ResumeRefreshStateChange();
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="AddToDockList"]/*'/>
		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="AddToDockList(IDockListContainer)"]/*'/>
		public DockPane AddToDockList(IDockListContainer container)
		{
			if (container == null)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullContainer"));

			DockAlignment alignment;
			if (container.DockState == DockState.DockLeft || container.DockState == DockState.DockRight)
				alignment = DockAlignment.Bottom;
			else
				alignment = DockAlignment.Right;

			return AddToDockList(container, container.DockList.GetDefaultPrevPane(this), alignment, 0.5);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="AddToDockList(IDockListContainer, DockPane, DockAlignment, double)"]/*'/>
		public DockPane AddToDockList(IDockListContainer container, DockPane prevPane, DockAlignment alignment, double proportion)
		{
			if (container == null)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullContainer"));

			if (container.IsFloat == this.IsFloat)
			{
				InternalAddToDockList(container, prevPane, alignment, proportion);
				return this;
			}

			DockContent firstContent = GetFirstContent(container.DockState);
			if (firstContent == null)
				return null;

			DockPane pane;
			DockPanel.DummyContent.DockPanel = DockPanel;
			if (container.IsFloat)
				pane = DockPanel.DockPaneFactory.CreateDockPane(DockPanel.DummyContent, (FloatWindow)container, true);
			else
				pane = DockPanel.DockPaneFactory.CreateDockPane(DockPanel.DummyContent, container.DockState, true);

			pane.AddToDockList(container, prevPane, alignment, proportion);
			SetVisibleContentsToPane(pane);
			DockPanel.DummyContent.DockPanel = null;

			return pane;
		}

		private void SetVisibleContentsToPane(DockPane pane)
		{
			SetVisibleContentsToPane(pane, ActiveContent);
		}

		private void SetVisibleContentsToPane(DockPane pane, DockContent activeContent)
		{
			for (int i=0; i<DisplayingContents.Count; i++)
			{
				DockContent content = DisplayingContents[i];
				if (content.IsDockStateValid(pane.DockState))
				{
					content.Pane = pane;
					i--;
				}
			}

			if (activeContent != null && pane.DisplayingContents.Contains(activeContent))
				pane.ActiveContent = activeContent;
		}

		private void InternalAddToDockList(IDockListContainer container, DockPane prevPane, DockAlignment alignment, double proportion)
		{
			if ((container.DockState == DockState.Float) != IsFloat)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.InvalidContainer"));

			int count = container.DockList.Count;
			if (container.DockList.Contains(this))
				count --;
			if (prevPane == null && count > 0)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NullPrevPane"));

			if (prevPane != null && !container.DockList.Contains(prevPane))
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.NoPrevPane"));

			if (prevPane == this)
				throw new InvalidOperationException(ResourceHelper.GetString("DockPane.AddToDockList.SelfPrevPane"));

			IDockListContainer oldContainer = DockListContainer;
			DockState oldDockState = DockState;
			container.DockList.Add(this);
			NestedDockingStatus.SetStatus(container.DockList, prevPane, alignment, proportion);

			if (DockHelper.IsDockWindowState(DockState))
				m_dockState = container.DockState;

			RefreshStateChange(oldContainer, oldDockState);
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="SetNestedDockingProportion(double)"]/*'/>
		public void SetNestedDockingProportion(double proportion)
		{
			NestedDockingStatus.SetStatus(NestedDockingStatus.DockList, NestedDockingStatus.PrevPane, NestedDockingStatus.Alignment, proportion);
			if (DockListContainer != null)
				((Control)DockListContainer).PerformLayout();
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="Float()"]/*'/>
		public DockPane Float()
		{
			DockContent activeContent = ActiveContent;

			DockPane floatPane = GetFloatPaneFromContents();
			if (floatPane == null)
			{
				DockContent firstContent = GetFirstContent(DockState.Float);
				if (firstContent == null)
					return null;
				floatPane = DockPanel.DockPaneFactory.CreateDockPane(firstContent,DockState.Float, true);
			}
			SetVisibleContentsToPane(floatPane, activeContent);
			floatPane.Activate();

			return floatPane;
		}

		private DockPane GetFloatPaneFromContents()
		{
			DockPane floatPane = null;
			for (int i=0; i<DisplayingContents.Count; i++)
			{
				DockContent content = DisplayingContents[i];
				if (!content.IsDockStateValid(DockState.Float))
					continue;

				if (floatPane != null && content.FloatPane != floatPane)
					return null;
				else
					floatPane = content.FloatPane;
			}

			return floatPane;
		}

		private DockContent GetFirstContent(DockState dockState)
		{
			for (int i=0; i<DisplayingContents.Count; i++)
			{
				DockContent content = DisplayingContents[i];
				if (content.IsDockStateValid(content.DockState))
					return content;
			}
			return null;
		}

		/// <include file='CodeDoc\DockPane.xml' path='//CodeDoc/Class[@name="DockPane"]/Method[@name="RestoreToPanel()"]/*'/>
		public void RestoreToPanel()
		{
			DockContent activeContent = DockPanel.ActiveContent;

			for (int i=DisplayingContents.Count-1; i>=0; i--)
			{
				DockContent content = DisplayingContents[i];
				try	{	content.IsFloat = false;	}	catch	{	}
			}

			if (activeContent != null)
				activeContent.Activate();
		}

		private AutoHidePane m_autoHidePane;
		internal AutoHidePane AutoHidePane
		{
			get	{	return m_autoHidePane;	}
		}
	}
}
