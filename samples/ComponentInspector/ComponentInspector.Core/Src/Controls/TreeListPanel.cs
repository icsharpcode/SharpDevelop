// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ICSharpCode.Core;
using NoGoop.Win32;

namespace NoGoop.Controls
{
	public class TreeListPanel : Panel
	{
		internal const int BASE_HEADER_HEIGHT = 18;
		internal const int COLUMN_HEADER_HEIGHT = BASE_HEADER_HEIGHT - 2;
		protected const int INITIAL_COL_WIDTH = 200;
		protected TreeListView      _treeView;
		protected Panel             _topPanel;
		internal Label              _headerPanel;
		internal ArrayList          _columnHeaderPanels;
		internal ArrayList          _columnHeaderSplitters;
		protected int               _scrollPos;
		protected static Pen     _greyPen;
		
		static TreeListPanel() 
		{
			SolidBrush brush = new SolidBrush(Color.LightGray);
			_greyPen = new Pen(brush, 1);
		}
		
		public TreeListPanel()
		{
			_columnHeaderPanels = new ArrayList();
			_columnHeaderSplitters = new ArrayList();
		}
		
		internal TreeListView TreeListView {
			get {
				return _treeView;
			}
			set {
				_treeView = value;
			}
		}
		
		// Called after this control is set up with its properties
		internal void Setup()
		{
			//AutoScroll = true;
			_topPanel = new Panel();
			//_topPanel.BorderStyle = BorderStyle.Fixed3D;
			for (int i = _treeView.Columns.Count - 1; i >= 0; i--) {
				ColumnHeader ch = (ColumnHeader)_treeView.Columns[i];
				Label chPanel = new Label();
				chPanel.Height = COLUMN_HEADER_HEIGHT;
				chPanel.Width = ch.Width;
				chPanel.Text = ch.Text;
				chPanel.BorderStyle = BorderStyle.Fixed3D;
				//chPanel.TextAlign = ch.TextAlign;
				if (i == 0)
					chPanel.Dock = DockStyle.Fill;
				else
					chPanel.Dock = DockStyle.Left;
				_columnHeaderPanels.Add(chPanel);
				_topPanel.Controls.Add(chPanel);
				Splitter splitter = new Splitter();
				splitter.Dock = DockStyle.Left;
				splitter.Height = COLUMN_HEADER_HEIGHT;
				splitter.Width = 3;
				splitter.SplitterMoved +=
					new SplitterEventHandler(SplitterMoved);
				splitter.BorderStyle = BorderStyle.Fixed3D;
				splitter.ForeColor = Color.Black;
				_columnHeaderSplitters.Add(splitter);
				_topPanel.Controls.Add(splitter);
			}
			_headerPanel = new Label();
			_headerPanel.Dock = DockStyle.Left;
			_headerPanel.Height = COLUMN_HEADER_HEIGHT;
			_headerPanel.Width = INITIAL_COL_WIDTH;
			_headerPanel.BorderStyle = BorderStyle.Fixed3D;
			_headerPanel.Text = StringParser.Parse("${res:ComponentInspector.TreeListPanel.HeaderPanel}");
			_topPanel.Controls.Add(_headerPanel);
			SetupSizes();
			Controls.Add(_treeView);
			Controls.Add(_topPanel);
			CreateControl();
		}
		
		protected void SetupSizes()
		{
			SetupTopPanelSizes();
			_treeView.Location = new Point(0, COLUMN_HEADER_HEIGHT);
			_treeView.Width = ClientSize.Width;
			_treeView.Height = ClientSize.Height - COLUMN_HEADER_HEIGHT;
		}
		
		protected void SetupTopPanelSizes()
		{
			Rectangle bounds = _topPanel.Bounds;
			bounds.X = -_scrollPos;
			_topPanel.Location = new Point(0, 0);
			_topPanel.Height = COLUMN_HEADER_HEIGHT;
			_topPanel.Bounds = bounds;
			_topPanel.Width = ClientSize.Width + _scrollPos;
		}
		
		protected override void OnLayout(LayoutEventArgs e)
		{
			SuspendLayout();
			SetupSizes();
			ResumeLayout();
			base.OnResize(e);
		}
		
		protected void SplitterMoved(object sender, SplitterEventArgs e)
		{
			_treeView.Invalidate();
		}
		
		protected const int PIXEL_PADDING = 2;
		
		internal void HandleColumnsDrawing(TreeListNode tlNode, NMCUSTOMDRAW cd)
		{
			RECT newRect = cd.rc;
			// Make text align correctly - seems to be off a pixel
			newRect.top += 1;
			using (Graphics g = Graphics.FromHdc(cd.hdc)) {
				// Do each of the columns defined for the tree
				for (int i = 0; i < _treeView.Columns.Count; i++) {
					newRect.left = cd.rc.left + ((Splitter)_columnHeaderSplitters[i]).Bounds.X - _scrollPos;
					Object colData = tlNode.ColumnData[i];
					if (i == 0) {
						// Clear out the data for the column (first time)
						g.FillRectangle(SystemBrushes.Window, newRect.left - PIXEL_PADDING, cd.rc.top, cd.rc.right, cd.rc.bottom);
					}
					// Vertical line to separate the columns
					g.DrawLine(_greyPen, newRect.left, cd.rc.top, newRect.left, cd.rc.bottom);
					// Write the actual text
					if (colData != null) {
						// Start the text one pixel after the line
						newRect.left += PIXEL_PADDING;
						g.DrawString(colData.ToString(), _treeView.Font, SystemBrushes.ControlText, newRect.left, newRect.top);
					}
				}
				// Draw the horizontal grid lines
				g.DrawLine(_greyPen, cd.rc.left, cd.rc.top, cd.rc.right, cd.rc.top);
				g.DrawLine(_greyPen, cd.rc.left, cd.rc.bottom, cd.rc.right, cd.rc.bottom);
			}
		}
		
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg) { 
				case Windows.WM_NOTIFY: {
					NMHDR nmhdr = (NMHDR)m.GetLParam(typeof(NMHDR));
					switch (nmhdr.code) {
						case Windows.NM_CUSTOMDRAW: {
							NMCUSTOMDRAW cd = (NMCUSTOMDRAW)m.GetLParam(typeof(NMCUSTOMDRAW));
							switch (cd.dwDrawStage) {
								case Windows.CDDS_PREPAINT:
									// Tell windows we want to see the POSTPAINT event
									m.Result = (IntPtr)Windows.CDRF_NOTIFYITEMDRAW;
									_scrollPos = Windows.GetScrollPos(cd.nmcd.hwndFrom, 
																	 Windows.SB_HORZ);
									SetupTopPanelSizes();
									return;
								case Windows.CDDS_ITEMPREPAINT:
									// Tell windows we want to see the POSTPAINT event
									m.Result = (IntPtr)Windows.CDRF_NOTIFYPOSTPAINT;
									return;
								case Windows.CDDS_ITEMPOSTPAINT:
									TreeNode node = 
										_treeView.GetNodeFromHandle(cd.dwItemSpec);
									if (node is TreeListNode)
										HandleColumnsDrawing((TreeListNode)node, cd);
									break;
								default:
									break;
							} 
							break;
						}
					}
					break; 
				}
			}
			base.WndProc(ref m);
		}
	}
}
