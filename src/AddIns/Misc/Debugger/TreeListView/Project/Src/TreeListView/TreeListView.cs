using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.APIs;
using System.Diagnostics;

namespace System.Windows.Forms
{
	/// <summary>
	/// TreeListView control (simulates a TreeView in a ListView)
	/// </summary>
	public class TreeListView : System.Windows.Forms.ListView
	{
		#region Private delegates
		private delegate int IntHandler();
		private delegate TreeListViewItem[] ItemArrayHandler();
		#endregion

		#region Events, Delegates, and internal calls
			#region Events
			/// <summary>
			/// Occurs when the label for an item is edited by the user.
			/// </summary>
			[Description("Occurs when the label for an item is edited by the user.")]
			public new event TreeListViewLabelEditEventHandler AfterLabelEdit;
			/// <summary>
			/// Occurs when the user starts editing the label of an item.
			/// </summary>
			[Description("Occurs when the user starts editing the label of an item."),Browsable(true)]
			public new event TreeListViewBeforeLabelEditEventHandler BeforeLabelEdit;
			/// <summary>
			/// Occurs before the tree node is collapsed.
			/// </summary>
			[Description("Occurs before the tree node is collapsed")]
			public event TreeListViewCancelEventHandler BeforeExpand;
			/// <summary>
			/// Occurs before the tree node is collapsed.
			/// </summary>
			[Description("Occurs before the tree node is collapsed")]
			public event TreeListViewCancelEventHandler BeforeCollapse;
			/// <summary>
			/// Occurs after the tree node is expanded
			/// </summary>
			[Description("Occurs after the tree node is expanded")]
			public event TreeListViewEventHandler AfterExpand;
			/// <summary>
			/// Occurs after the tree node is collapsed
			/// </summary>
			[Description("Occurs after the tree node is collapsed")]
			public event TreeListViewEventHandler AfterCollapse;
			#endregion
			#region On???
			/// <summary>
			/// Raises the AfterLabelEdit event.
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnAfterLabelEdit(TreeListViewLabelEditEventArgs e)
			{
				if(AfterLabelEdit != null) AfterLabelEdit(this, e);
			}
			/// <summary>
			/// Please use OnAfterLabelEdit(TreeListViewLabelEditEventArgs e)
			/// </summary>
			/// <param name="e"></param>
			protected override void OnAfterLabelEdit(LabelEditEventArgs e)
			{
				throw(new Exception("Please use OnAfterLabelEdit(TreeListViewLabelEditEventArgs e)"));
			}
			/// <summary>
			/// Raises the BeforeLabelEdit event.
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnBeforeLabelEdit(TreeListViewBeforeLabelEditEventArgs e)
			{
				if(BeforeLabelEdit != null) BeforeLabelEdit(this, e);
			}
			/// <summary>
			/// Please use OnBeforeLabelEdit(TreeListViewLabelEditEventArgs e)
			/// </summary>
			/// <param name="e"></param>
			protected override void OnBeforeLabelEdit(LabelEditEventArgs e)
			{
				throw(new Exception("Please use OnBeforeLabelEdit(TreeListViewLabelEditEventArgs e)"));
			}
			/// <summary>
			/// Raises the BeforeExpand event.
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnBeforeExpand(TreeListViewCancelEventArgs e)
			{
				if(BeforeExpand != null) BeforeExpand(this, e);
			}
			/// <summary>
			/// Raises the AfterExpand event.
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnAfterExpand(TreeListViewEventArgs e)
			{
				if(AfterExpand != null) AfterExpand(this, e);
			}
			/// <summary>
			/// Raises the BeforeCollapse event.
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnBeforeCollapse(TreeListViewCancelEventArgs e)
			{
				if(BeforeCollapse != null) BeforeCollapse(this, e);
			}
			/// <summary>
			/// Raises the AfterCollapse event.
			/// </summary>
			/// <param name="e"></param>
			protected virtual void OnAfterCollapse(TreeListViewEventArgs e)
			{
				if(AfterCollapse != null) AfterCollapse(this, e);
			}
			#endregion
			#region Internal calls
			internal void RaiseBeforeExpand(TreeListViewCancelEventArgs e)
			{
				OnBeforeExpand(e);
			}
			/// <summary>
			/// Raises the MouseDown event
			/// </summary>
			/// <param name="e">A MouseEventArgs that contains the event data</param>
			protected override void OnMouseDown(MouseEventArgs e)
			{
				if(!_skipMouseDownEvent)
					base.OnMouseDown(e);
			}
			internal void RaiseBeforeCollapse(TreeListViewCancelEventArgs e)
			{
				OnBeforeCollapse(e);
			}
			internal void RaiseAfterExpand(TreeListViewEventArgs e)
			{
				OnAfterExpand(e);
			}
			internal void RaiseAfterCollapse(TreeListViewEventArgs e)
			{
				OnAfterCollapse(e);
			}
			#endregion
		#endregion
		#region Modified properties
			#region Scrollable
			private bool _scrollable = true;
			/// <summary>
			/// Gets or sets a value indicating whether a scroll bar is added to the control when there is not enough room to display all items
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
			[Browsable(true), Description("Gets or sets a value indicating whether a scroll bar is added to the control when there is not enough room to display all items")]
			new public bool Scrollable
			{
				get
				{
					return _scrollable;
				}
				set
				{
					_scrollable = value;
				}
			}
			#endregion
			#region CheckBoxes
			private CheckBoxesTypes _checkboxes = CheckBoxesTypes.None;
			/// <summary>
			/// Gets or sets a value indicating whether a check box appears next to each item in the control
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(typeof(CheckBoxesTypes), "None")]
			[Browsable(true), Description("Gets or sets a value indicating whether a check box appears next to each item in the control")]
			new public CheckBoxesTypes CheckBoxes
			{
				get
				{
					return _checkboxes;
				}
				set
				{
					if(_checkboxes == value) return;
					_checkboxes = value;
					_checkDirection = value == CheckBoxesTypes.Recursive ? CheckDirection.All : CheckDirection.None;
					base.CheckBoxes = value == CheckBoxesTypes.None ? false : true;
					if(Created)
						Invalidate();
				}
			}
			#endregion
			#region FullRowSelect
			/// <summary>
			/// Gets or sets a value indicating whether clicking an item selects all its subitems
			/// </summary>
			[Browsable(true), Description("Gets or sets a value indicating whether clicking an item selects all its subitems"),
			DefaultValue(true)]
			new public bool FullRowSelect
			{
				get
				{
					return base.FullRowSelect;
				}
				set
				{
					base.FullRowSelect = value;
				}
			}
			#endregion
			#region StateImageList
			/// <summary>
			/// Not supported
			/// </summary>
			[Browsable(false)]
			new public ImageList StateImageList
			{
				get{return base.StateImageList;}
				set{base.StateImageList = value;}
			}
			#endregion
			#region LargeImageList
			/// <summary>
			/// Not supported
			/// </summary>
			[Browsable(false)]
			new public ImageList LargeImageList
			{
				get{return base.LargeImageList;}
				set{base.LargeImageList = value;}
			}
			#endregion
			#region SmallImageList
			private ImageList _smallimaglist = null;
			/// <summary>
			/// Gets or sets the ImageList to use when displaying items as small icons in the control (must be filled in)
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(null)]
			[Browsable(true), Description("Gets or sets the ImageList to use when displaying items as small icons in the control")]
			new public ImageList SmallImageList
			{
				get
				{
					return _smallimaglist;
				}
				set
				{
					_smallimaglist = value;
				}
			}
			#endregion
			#region Sorting
			private SortOrder _sorting = SortOrder.Ascending;
			/// <summary>
			/// Get or set the sort order
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
			[Browsable(true), Description("Get or Set the sort order"), DefaultValue(typeof(SortOrder), "Ascending")]
			new public SortOrder Sorting
			{
				get{return(_sorting);}
				set{if(_sorting == value) return;
					_sorting = value;
					Items.SortOrderRecursively = value;}
			}
			#endregion
			#region ExpandMethod
			private TreeListViewExpandMethod _expandmethod = TreeListViewExpandMethod.EntireItemDbleClick;
			/// <summary>
			/// Get or set the expand method
			/// </summary>
			[Browsable(true), DefaultValue(typeof(TreeListViewExpandMethod), "EntireItemDbleClick"),
			Description("Get or Set the expand method")]
			public TreeListViewExpandMethod ExpandMethod
			{
				get{return(_expandmethod);}
				set{_expandmethod = value;}
			}
			#endregion
			#region View
			/// <summary>
			/// View (always Details)
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			[Browsable(false)]
			public new View View
			{
				get{return(base.View);}
				set{base.View = View.Details;}
			}
			#endregion
			#region Items
			/// <summary>
			/// Items of the TreeListView
			/// </summary>
			//		[Browsable(true),
			//		Editor(typeof(TreeListViewItemsEditor), typeof(System.Drawing.Design.UITypeEditor))]
			[Browsable(false)]
			[Description("Items of the TreeListView")]
			new public TreeListViewItemCollection Items
			{
				get{return(_items);}
			}
			#endregion
			#region SelectedItems
			/// <summary>
			/// Get currently selected items
			/// </summary>
			[Browsable(false)]
			new public SelectedTreeListViewItemCollection SelectedItems
			{
				get
				{
					SelectedTreeListViewItemCollection sel = new SelectedTreeListViewItemCollection(this);
					return(sel);
				}
			}
			#endregion
			#region CheckedItems
			/// <summary>
			/// Get currently checked items
			/// </summary>
			[Browsable(false)]
			public new TreeListViewItem[] CheckedItems
			{
				get
				{
					return (TreeListViewItem[]) Invoke(new ItemArrayHandler(GetCheckedItems));
				}
			}
			private TreeListViewItem[] GetCheckedItems()
			{
				if(InvokeRequired)
					throw(new Exception("Invoke required"));
				TreeListViewItemCollection items = new TreeListViewItemCollection();
				foreach(TreeListViewItem item in Items)
					item.GetCheckedItems(ref items);
				return(items.ToArray());
			}
			#endregion
			#region FocusedItem
			/// <summary>
			/// Gets the item in the control that currently has focus.
			/// </summary>
			[Browsable(false)]
			new public TreeListViewItem FocusedItem
			{
				get{return (TreeListViewItem) base.FocusedItem;}
			}
			#endregion
		#endregion
		#region Properties
			#region Private Properties
			internal TreeListViewItem _selectionMark = null;
			internal bool _updating = false;
			internal bool _skipMouseDownEvent = false;
			internal CheckDirection _checkDirection = CheckDirection.None;
			internal int _comctl32Version;
			private DateTime _lastdoubleclick;
			internal EditItemInformations _lastitemclicked;
			private CustomEdit _customedit;
			private TreeListViewItemCollection _items;
			private System.ComponentModel.IContainer components;
			private System.Windows.Forms.ImageList imageList1;
			internal bool FreezeCheckBoxes = false;
			private Point _mousescrollposition = new Point(0, 0);
			private DateTime _dblclicktime = DateTime.Now;
			internal System.Windows.Forms.ImageList plusMinusImageList;
			#endregion

			#region HasMarquee
			private bool _hasMarquee = false;
			/// <summary>
			/// Gets whether the marquee selection tool is curently being used
			/// </summary>
			[Browsable(false)]
			public bool HasMarquee
			{
				get
				{
					return _hasMarquee;
				}
			}
			#endregion
			#region EditedItem
			internal EditItemInformations _editeditem = new EditItemInformations();
			/// <summary>
			/// Gets the informations of the current edited item
			/// </summary>
			[Browsable(false)]
			public EditItemInformations EditedItem
			{
				get
				{
					return _editeditem;
				}
			}
			#endregion
			#region InEdit
			private bool _inedit;
			/// <summary>
			/// Gets whether an item is currently edited
			/// </summary>
			[Browsable(false)]
			public bool InEdit
			{
				get
				{
					return _inedit;
				}
			}
			#endregion
			#region ItemsCount
			/// <summary>
			/// Get the number of items recursively
			/// </summary>
			[Browsable(false)]
			public int ItemsCount
			{
				get
				{
					TreeListViewItem[] items = _items.ToArray();
					int count = items.Length;
					foreach(TreeListViewItem item in items) count += item.ChildrenCount;
					return count;
				}
			}
			#endregion
			#region Comparer
			/// <summary>
			/// Get or set the comparer
			/// </summary>
			[Browsable(false)]
			public ITreeListViewItemComparer Comparer
			{
				get{return(Items.Comparer);}
				set{Items.Comparer = value;}
			}
			#endregion
			#region ShowPlusMinus
			private bool _showplusminus = true;
			/// <summary>
			/// Gets or sets a value indicating whether plus-sign (+) and minus-sign (-) buttons are displayed next to TreeListView that contain child TreeListViews
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
			[Browsable(true), Description("Gets or sets a value indicating whether plus-sign (+) and minus-sign (-) buttons are displayed next to TreeListView that contain child TreeListViews")]
			public bool ShowPlusMinus
			{
				get
				{
					return _showplusminus;
				}
				set
				{
					if(_showplusminus == value) return;
					_showplusminus = value;
					if(Created) Invoke(new MethodInvoker(VisChanged));
				}
			}
			#endregion
			#region PlusMinusLineColor
			private Color _plusMinusLineColor = Color.DarkGray;
			/// <summary>
			/// Gets or Sets the color of the lines if ShowPlusMinus property is enabled
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(typeof(Color), "DarkGray")]
			[Browsable(true), Description("Gets or Sets the color of the lines if ShowPlusMinus property is enabled")]
			public Color PlusMinusLineColor
			{
				get
				{
					return _plusMinusLineColor;
				}
				set
				{
					_plusMinusLineColor = value;
					if(Created) Invalidate();
				}
			}
			#endregion
			#region UseXPHighlightStyle
			private bool _useXPHighLightStyle = true;
			/// <summary>
			/// Gets or Sets whether the control draw XP-Style highlight color
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue(true)]
			[Browsable(true), Description("Gets or Sets whether the control draw XP-Style highlight color")]
			public bool UseXPHighlightStyle
			{
				get
				{
					return _useXPHighLightStyle;
				}
				set
				{
					_useXPHighLightStyle = value;
					if(Created) Invalidate();
				}
			}
			#endregion
			#region PathSeparator
			private string _pathSeparator = "\\";
			/// <summary>
			/// Gets or sets the delimiter string that the TreeListViewItem path uses
			/// </summary>
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), DefaultValue("\\")]
			[Browsable(true), Description("Gets or sets the delimiter string that the TreeListViewItem path uses")]
			public string PathSeparator
			{
				get
				{
					return _pathSeparator;
				}
				set
				{
					_pathSeparator = value;
				}
			}
			#endregion
		#endregion

		#region Constructor
		/// <summary>
		/// Create a new instance of a TreeListView
		/// </summary>
		public TreeListView()
		{
			InitializeComponent();
			if(!IsHandleCreated) CreateHandle();
			_items = new TreeListViewItemCollection(this);
			_items.SortOrder = _sorting;
			_comctl32Version = APIsComctl32.GetMajorVersion();

			int style = APIsUser32.SendMessage(Handle, (int) APIsEnums.ListViewMessages.GETEXTENDEDLISTVIEWSTYLE, 0, 0);
			style |= (int) (APIsEnums.ListViewExtendedStyles.INFOTIP | APIsEnums.ListViewExtendedStyles.LABELTIP);
			APIsUser32.SendMessage(Handle, (int) APIsEnums.ListViewMessages.SETEXTENDEDLISTVIEWSTYLE, 0, style);
		}
		#endregion
		#region WndProc
		/// <summary>
		/// WndProc
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			TreeListViewItem item = null; Rectangle rec;
			switch((APIsEnums.WindowMessages) m.Msg)
			{
				#region NOTIFY
				case APIsEnums.WindowMessages.NOTIFY:
				case (APIsEnums.WindowMessages) APIsEnums.ReflectedMessages.NOTIFY:
					APIsStructs.NMHDR nmhdr = (APIsStructs.NMHDR) m.GetLParam(typeof(APIsStructs.NMHDR));
					APIsStructs.NMHEADER nmheader =(APIsStructs.NMHEADER) m.GetLParam(typeof(APIsStructs.NMHEADER));
					switch((APIsEnums.ListViewNotifications) nmhdr.code)
					{
						#region APIsEnums.ListViewNotifications.MARQUEEBEGIN
						case APIsEnums.ListViewNotifications.MARQUEEBEGIN:
							if((MouseButtons & MouseButtons.Left) != MouseButtons.Left)
								m.Result = (IntPtr)1;
							else
								_hasMarquee = true;
							break;
						#endregion
						#region APIsEnums.ListViewNotifications.ITEMCHANGING
						case APIsEnums.ListViewNotifications.ITEMCHANGING:
							APIsStructs.NMLISTVIEW nmlistview = (APIsStructs.NMLISTVIEW) m.GetLParam(typeof(APIsStructs.NMLISTVIEW));
							if(nmlistview.iItem < 0) break;
							if((item = GetTreeListViewItemFromIndex(nmlistview.iItem)) == null) break;
							bool cancel = false;
							if(nmlistview.Select)
							{
								if(_selectionMark == null) _selectionMark = item;
								else if(!_selectionMark.Visible) _selectionMark = item;
								if(HasMarquee) item.Focused = true;
							}
							else if(nmlistview.UnSelect && HasMarquee)
							{
								if(item.NextVisibleItem != null)
									if(item.NextVisibleItem.Selected)
										item.NextVisibleItem.Focused = true;
								if(item.PrevVisibleItem != null)
									if(item.PrevVisibleItem.Selected)
										item.PrevVisibleItem.Focused = true;
							}
							#region Select after dbl click
							// Disable the selection after a double click (normaly, if the control scrolls after
							// a collapse, the new item under the cursor is automatically selected...)
							if(_dblclicktime.AddMilliseconds(500).CompareTo(DateTime.Now) > 0 &&
								(nmlistview.Select || nmlistview.Focus) &&
								FocusedItem != item)
								cancel = true;
							#endregion
							#region Wrong Level Select
							if(((APIsEnums.ListViewItemStates)nmlistview.uNewState & APIsEnums.ListViewItemStates.SELECTED) == APIsEnums.ListViewItemStates.SELECTED &&
								MultiSelect)
								if(SelectedIndices.Count > 0)
									if(GetTreeListViewItemFromIndex(nmlistview.iItem).Parent != SelectedItems[0].Parent)
										cancel = true;
							#endregion
							#region Check during selection
							// Disable check boxes check when :
							// - the Marquee selection tool is being used
							// - the Ctrl or Shift keys are down
							bool state = (nmlistview.uChanged & (uint)APIsEnums.ListViewItemFlags.STATE) == (uint)APIsEnums.ListViewItemFlags.STATE;
							bool ctrlKeyDown = (ModifierKeys & Keys.Control) == Keys.Control;
							bool shiftKeyDown = (ModifierKeys & Keys.Shift) == Keys.Shift;
							if((nmlistview.Check || nmlistview.UnCheck) &&
								(HasMarquee || ctrlKeyDown || shiftKeyDown))
							{
//									MessageBox.Show(this,
//										"uChanged = " + nmlistview->uChanged.ToString() + "\n\n" + 
//										"uOld = " + nmlistview->uOldState.ToString() + "\n" + 
//										"uNew = " + nmlistview->uChanged.ToString() + "\n\n" +
//										"OldCheck : " + (oldCheck ? "true" : "false") + "\n" + 
//										"NewCheck : " + (newCheck ? "true" : "false"));
								cancel = true;
							}
							#endregion
							if(cancel)
							{
								m.Result = (IntPtr)1;
								return;
							}
							break;
						#endregion

						#region APIsEnums.ListViewNotifications.BEGINLABELEDIT
						case APIsEnums.ListViewNotifications.BEGINLABELEDIT:
							// Cancel label edit if the message is sent just after a double click
							if(_lastdoubleclick.AddMilliseconds(450) > DateTime.Now)
							{
								Message canceledit = Message.Create(Handle, (int) APIsEnums.ListViewMessages.CANCELEDITLABEL, IntPtr.Zero, IntPtr.Zero);
								WndProc(ref canceledit);
								m.Result = (IntPtr) 1;
								return;
							}
							item = _lastitemclicked.Item;
							item.EnsureVisible();
							// Add subitems if needed
							while(item.SubItems.Count-1 < _lastitemclicked.ColumnIndex) item.SubItems.Add("");
							TreeListViewBeforeLabelEditEventArgs beforeed = new TreeListViewBeforeLabelEditEventArgs(
								FocusedItem, _lastitemclicked.ColumnIndex, item.SubItems[_lastitemclicked.ColumnIndex].Text);
							OnBeforeLabelEdit(beforeed);
							if(beforeed.Cancel)
							{
								Message canceledit = Message.Create(Handle, (int) APIsEnums.ListViewMessages.CANCELEDITLABEL, IntPtr.Zero, IntPtr.Zero);
								WndProc(ref canceledit);
								m.Result = (IntPtr) 1;
								return;
							}
							_inedit = true;
							// Get edit handle
							Message mess = Message.Create(Handle, (int)APIsEnums.ListViewMessages.GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero);
							WndProc(ref mess);
							IntPtr edithandle = mess.Result;
							_customedit = new CustomEdit(edithandle, this, beforeed.Editor);
							_editeditem = new EditItemInformations(
								FocusedItem, beforeed.ColumnIndex, FocusedItem.SubItems[beforeed.ColumnIndex].Text);
							m.Result = IntPtr.Zero;
							return;
						#endregion
						#region APIsEnums.ListViewNotifications.ENDLABELEDIT
						case APIsEnums.ListViewNotifications.ENDLABELEDIT:
							if(_customedit != null)
								_customedit.HideEditControl();
							_customedit = null;
							_inedit = false;
							_editeditem = new EditItemInformations();
							m.Result = IntPtr.Zero;
							return;
						#endregion
						
						#region CUSTOMDRAW
						case (APIsEnums.ListViewNotifications) APIsEnums.NotificationMessages.CUSTOMDRAW:
							base.WndProc(ref m);
							CustomDraw(ref m);
							return;
						#endregion

						#region BEGINSCROLL
						case APIsEnums.ListViewNotifications.BEGINSCROLL:
							_updating = true;
							break;
						#endregion
						#region ENDSCROLL
						case APIsEnums.ListViewNotifications.ENDSCROLL:
							_updating = false;
							// Disable display bug with vertical lines (slow...)
//							if(ShowPlusMinus)
//							{
//								DrawPlusMinusItemsLines();
//								DrawPlusMinusItems();
//							}
							break;
						#endregion

						#region APIsEnums.HeaderControlNotifications.BEGINDRAG
						case (APIsEnums.ListViewNotifications) APIsEnums.HeaderControlNotifications.BEGINDRAG:
							nmheader =(APIsStructs.NMHEADER) m.GetLParam(typeof(APIsStructs.NMHEADER));
							if(nmheader.iItem == 0)
							{
								m.Result = (IntPtr)1;
								return;
							}
							break;
						#endregion
						#region APIsEnums.HeaderControlNotifications.ENDDRAG
						case (APIsEnums.ListViewNotifications) APIsEnums.HeaderControlNotifications.ENDDRAG:
							nmheader =(APIsStructs.NMHEADER) m.GetLParam(typeof(APIsStructs.NMHEADER));
							// Get mouse position in header coordinates
							IntPtr headerHandle = (IntPtr) APIsUser32.SendMessage(Handle, (int) APIsEnums.ListViewMessages.GETHEADER, IntPtr.Zero, IntPtr.Zero);
							APIsStructs.POINTAPI pointapi = new APIsStructs.POINTAPI(MousePosition);
							APIsUser32.ScreenToClient(headerHandle, ref pointapi);
							// HeaderItem Rect
							APIsStructs.RECT headerItemRect = new APIsStructs.RECT();
							APIsUser32.SendMessage(headerHandle, (int)APIsEnums.HeaderControlMessages.GETITEMRECT, 0, ref headerItemRect);
							int headerItemWidth = headerItemRect.right - headerItemRect.left;
							// Cancel the drag operation if the first column is moved
							// or destination is the first column
							if(pointapi.x <= headerItemRect.left + headerItemWidth / 2 ||
								nmheader.iItem == 0)
							{
								m.Result = (IntPtr)1;
								return;
							}
							break;
						#endregion
						#region APIsEnums.HeaderControlNotifications.TRACK / ENDTRACK
//						case (APIsEnums.ListViewNotifications)APIsEnums.HeaderControlNotifications.TRACK:
						case (APIsEnums.ListViewNotifications)APIsEnums.HeaderControlNotifications.ENDTRACK:
							Invalidate();
							break;
						#endregion
					}
					break;
				#endregion

				#region LBUTTONDOWN
					// Cancel the click on checkboxes if the item is not "checkable"
					case APIsEnums.WindowMessages.LBUTTONDOWN:
						if(Columns.Count == 0) break;
						// Set the clickeditem and column
						int colclicked = GetColumnAt(MousePosition);
						if(colclicked == -1) colclicked = 0;
						item = GetItemAtFullRow(PointToClient(MousePosition));
						_lastitemclicked = new EditItemInformations(item, colclicked, "");
						if(_selectionMark == null || !_selectionMark.Visible) _selectionMark = item;
						if(((APIsEnums.KeyStatesMasks)(int)m.WParam & APIsEnums.KeyStatesMasks.SHIFT) != APIsEnums.KeyStatesMasks.SHIFT &&
							!(((APIsEnums.KeyStatesMasks)(int)m.WParam & APIsEnums.KeyStatesMasks.CONTROL) == APIsEnums.KeyStatesMasks.CONTROL &&
							item.Parent != _selectionMark.Parent))
							_selectionMark = item;
						// Get where the mouse has clicked
						APIsStructs.LVHITTESTINFO lvhittest = new APIsStructs.LVHITTESTINFO();
						lvhittest.pt = new APIsStructs.POINTAPI(PointToClient(MousePosition));
						APIsUser32.SendMessage(Handle, (Int32) APIsEnums.ListViewMessages.HITTEST, 0, ref lvhittest);
						if(item == null) break;
						// Plus / Minus click
						if(item.GetBounds(TreeListViewItemBoundsPortion.PlusMinus).Contains(PointToClient(MousePosition)) &&
							ShowPlusMinus && item.Items.Count > 0 &&
							Columns[0].Width > (item.Level+1)*SystemInformation.SmallIconSize.Width)
						{
							Focus();
							if(item.IsExpanded) item.Collapse();
							else item.Expand();
							OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, PointToClient(MousePosition).X, PointToClient(MousePosition).Y, 0));
							return;
						}
						// Cancel mouse click if multiselection on a wrong item
						if(SelectedIndices.Count > 0 &&
							(((APIsEnums.KeyStatesMasks)(int)m.WParam & APIsEnums.KeyStatesMasks.SHIFT) == APIsEnums.KeyStatesMasks.SHIFT ||
							((APIsEnums.KeyStatesMasks)(int)m.WParam & APIsEnums.KeyStatesMasks.CONTROL) == APIsEnums.KeyStatesMasks.CONTROL) &&
							MultiSelect)
						{
							if(_selectionMark.Parent == item.Parent &&
								((APIsEnums.KeyStatesMasks)(int)m.WParam & APIsEnums.KeyStatesMasks.SHIFT) == APIsEnums.KeyStatesMasks.SHIFT)
							{
								_updating = true;
								SetSelectedItemsRange(item, _selectionMark);
								// Prevent all item at the wrong level of being selected
								m.WParam = (IntPtr) APIsEnums.KeyStatesMasks.CONTROL;
								base.WndProc(ref m);
								item.Selected = true;
								_updating = false;
								DrawSelectedItemsFocusCues();
								return;
							}
						}
						break;
				#endregion
				#region LBUTTONDBLCLK
					// Disable this notification to remove the auto-check when
					// the user double-click on an item and append the expand / collapse function
				case APIsEnums.WindowMessages.LBUTTONDBLCLK:
					_lastdoubleclick = DateTime.Now;
					if(FocusedItem != null)
					{
						item = FocusedItem;
						bool doExpColl = false;
						switch(ExpandMethod)
						{
							case TreeListViewExpandMethod.IconDbleClick:
								rec = item.GetBounds(ItemBoundsPortion.Icon);
								if(rec.Contains(PointToClient(MousePosition))) doExpColl = true;
								break;
							case TreeListViewExpandMethod.ItemOnlyDbleClick:
								rec = item.GetBounds(ItemBoundsPortion.ItemOnly);
								if(rec.Contains(PointToClient(MousePosition))) doExpColl = true;
								break;
							case TreeListViewExpandMethod.EntireItemDbleClick:
								rec = item.GetBounds(ItemBoundsPortion.Entire);
								if(rec.Contains(PointToClient(MousePosition))) doExpColl = true;
								break;
							default:
								break;
						}
						if(doExpColl)
						{
							_dblclicktime = DateTime.Now;
							Cursor = Cursors.WaitCursor;
							BeginUpdate();
							if(item.IsExpanded) item.Collapse();
							else item.Expand();
							EndUpdate();
							Cursor = Cursors.Default;
						}
					}
					OnDoubleClick(new EventArgs());
					return;
				#endregion
				#region MOUSEMOVE
				case APIsEnums.WindowMessages.MOUSEMOVE:
					if((MouseButtons & MouseButtons.Left) != MouseButtons.Left && HasMarquee)
						_hasMarquee = false;
					break;
				#endregion
				#region UNICHAR, CHAR, KEYDOWN
				case APIsEnums.WindowMessages.UNICHAR:
				case APIsEnums.WindowMessages.CHAR:
					CharPressed((char) m.WParam);
					return;
				case APIsEnums.WindowMessages.KEYDOWN:
					OnKeyDown(new KeyEventArgs((Keys)(int) m.WParam));
					return;
				#endregion
				#region PAINT
				case APIsEnums.WindowMessages.PAINT:
					if(InEdit && EditedItem.Item != null)
					{
						APIsStructs.RECT rect = new APIsStructs.RECT(
							EditedItem.Item.GetBounds(ItemBoundsPortion.Entire));
						APIsUser32.ValidateRect(Handle, ref rect);
					}
					base.WndProc(ref m);
					DrawIntermediateStateItems();
					DrawSelectedItemsFocusCues();
					return;
				#endregion
				#region VSCROLL, HSCROLL, ENSUREVISIBLE
				case APIsEnums.WindowMessages.VSCROLL:
				case APIsEnums.WindowMessages.HSCROLL:
				case (APIsEnums.WindowMessages)APIsEnums.ListViewMessages.ENSUREVISIBLE:
					if(!Scrollable)
					{
						m.Result = (IntPtr)0;
						return;
					}
					break;
				#endregion
			}
			base.WndProc(ref m);
		}
		#region KeyFunction
			#region OnKeyDown
			/// <summary>
			/// Raises the KeyDown event
			/// </summary>
			/// <param name="e"></param>
			protected override void OnKeyDown(KeyEventArgs e)
			{
				Keys key = e.KeyCode;
				if(FocusedItem == null)
				{
					if(base.Items.Count > 0 &&
						(key == Keys.Down || key == Keys.Up || key == Keys.Left || key == Keys.Right))
					{
						base.Items[0].Selected = true;
						base.Items[0].Focused = true;
						base.Items[0].EnsureVisible();
					}
					base.OnKeyDown(e);
					return;
				}
				TreeListViewItem item = FocusedItem;
				switch(key)
				{
					case Keys.Down:
						if(item.NextVisibleItem != null)
						{
							TreeListViewItem nextitem = item.NextVisibleItem;
							if((Control.ModifierKeys & Keys.Shift) == Keys.Shift &&
								MultiSelect)
							{
								if(item.Parent != nextitem.Parent && item.Selected)
								{
									while((nextitem = nextitem.NextVisibleItem) != null)
										if(nextitem.Parent == item.Parent)
											break;
								}
								if(nextitem != null)
									SetSelectedItemsRange(_selectionMark, nextitem);
								else
									nextitem = item.NextVisibleItem;
							}
							else if((Control.ModifierKeys & Keys.Control) != Keys.Control)
							{
								SetSelectedItemsRange(nextitem, nextitem);
								_selectionMark = nextitem;
							}
							nextitem.Focused = true;
							nextitem.EnsureVisible();
						}
						break;
					case Keys.Up:
						if(item.PrevVisibleItem != null)
						{
							TreeListViewItem previtem = item.PrevVisibleItem;
							if((Control.ModifierKeys & Keys.Shift) == Keys.Shift &&
								MultiSelect)
							{
								if(item.Parent != previtem.Parent && item.Selected)
								{
									while((previtem = previtem.PrevVisibleItem) != null)
										if(previtem.Parent == item.Parent)
											break;
								}
								if(previtem != null)
									SetSelectedItemsRange(_selectionMark, previtem);
								else
									previtem = item.PrevVisibleItem;
							}
							else if((Control.ModifierKeys & Keys.Control) != Keys.Control)
							{
								SetSelectedItemsRange(previtem, previtem);
								_selectionMark = previtem;
							}
							previtem.Focused = true;
							previtem.EnsureVisible();
						}
						break;
					case Keys.Enter:
						base.SelectedItems.Clear();
						if(item.IsExpanded) item.Collapse();
						else item.Expand();
						item.Selected = true;
						item.EnsureVisible();
						break;
					case Keys.Left:
						if(item.IsExpanded)
						{
							base.SelectedItems.Clear();
							item.Selected = true;
							item.Collapse();
							item.EnsureVisible();
						}
						else if(item.Parent != null)
						{
							base.SelectedItems.Clear();
							item.Parent.Selected = true;
							item.Parent.Focused = true;
							item.Parent.EnsureVisible();
						}
						break;
					case Keys.Right:
						if(item.Items.Count == 0) break;
						if(!item.IsExpanded)
						{
							base.SelectedItems.Clear();
							item.Selected = true;
							item.Expand();
							item.EnsureVisible();
						}
						else
						{
							base.SelectedItems.Clear();
							item.Items[item.Items.Count-1].Selected = true;
							item.Items[item.Items.Count-1].Focused = true;
							item.Items[item.Items.Count-1].EnsureVisible();
						}
						break;
					case Keys.Space:
						if(base.CheckBoxes) item.Checked = !item.Checked;
						break;
				}
				base.OnKeyDown(e);
			}
			#endregion
			#region CharPressed
			private void CharPressed(char character)
			{
				Debug.Assert(!InvokeRequired);
				string begin = character.ToString().ToUpper();
				if(FocusedItem == null) return;
				TreeListViewItem item = FocusedItem;
				base.SelectedItems.Clear();
				item.Selected = true;
				// Select an item begining with the specified character
				if((begin.CompareTo("A") >= 0 && begin.CompareTo("Z") <= 0) || begin == " ")
				{
					// Get the collection in wich the item is
					TreeListViewItemCollection collection = item.Parent == null ? this.Items : item.Parent.Items;
					bool founded = false;
					// Search in the next items
					for(int i = collection.GetIndexOf(item) + 1 ; i < collection.Count ; i++)
						if(collection[i].Text.ToUpper().StartsWith(begin))
						{
							collection[i].Selected = true;
							collection[i].Focused = true;
							collection[i].EnsureVisible();
							founded = true;
							break;
						}
					// Search in the previous items
					if(!founded)
						for(int i = 0 ; i < collection.GetIndexOf(item) ; i++)
							if(collection[i].Text.ToUpper().StartsWith(begin))
							{
								collection[i].Selected = true;
								collection[i].Focused = true;
								collection[i].EnsureVisible();
								founded = true;
								break;
							}
				}
			}
			#endregion
		#endregion
		#endregion
		#region Draw
			#region CustomDraw
			private void CustomDraw(ref Message m)
			{
				int iRow, iCol; bool bSelected;
				unsafe
				{
					APIsStructs.NMLVCUSTOMDRAW * nmlvcd = (APIsStructs.NMLVCUSTOMDRAW *)m.LParam.ToPointer();
					switch((APIsEnums.CustomDrawDrawStateFlags)nmlvcd->nmcd.dwDrawStage)
					{
						case APIsEnums.CustomDrawDrawStateFlags.PREPAINT:
							m.Result = (IntPtr)APIsEnums.CustomDrawReturnFlags.NOTIFYITEMDRAW;
							break;
						case APIsEnums.CustomDrawDrawStateFlags.ITEMPREPAINT:
							m.Result = (IntPtr)APIsEnums.CustomDrawReturnFlags.NOTIFYSUBITEMDRAW;
							break;
						case APIsEnums.CustomDrawDrawStateFlags.ITEMPREPAINT |
							APIsEnums.CustomDrawDrawStateFlags.SUBITEM:
							iRow = (int)nmlvcd->nmcd.dwItemSpec;
							iCol = (int)nmlvcd->iSubItem;
							bSelected = base.Items[iRow].Selected;// && this.Focused;
							TreeListViewItem item = GetTreeListViewItemFromIndex(iRow);
							if(bSelected && _useXPHighLightStyle)
							{
								Color color = Focused ? ColorUtil.VSNetSelectionColor : ColorUtil.VSNetSelectionUnfocusedColor;
								if(HideSelection && !Focused) color = BackColor;
								if(FullRowSelect || iCol == 0)
									nmlvcd->clrTextBk = (int)ColorUtil.RGB(color.R, color.G, color.B);
								nmlvcd->nmcd.uItemState &= ~(uint)APIsEnums.CustomDrawItemStateFlags.SELECTED;
								if(iCol == 0) item.DrawFocusCues();
							}
							if(iCol == 0)
							{
								item.DrawIntermediateState();
								item.DrawPlusMinusLines();
								item.DrawPlusMinus();
							}
							m.Result = (IntPtr)APIsEnums.CustomDrawReturnFlags.NEWFONT;
							break;
					}
				}
			}
			#endregion
			#region Draw Items Parts
			internal void DrawIntermediateStateItems()
			{
				Debug.Assert(!InvokeRequired);
				if(CheckBoxes != CheckBoxesTypes.Recursive) return;
				if(_updating) return;
				TreeListViewItemCollection items = GetVisibleItems();
				Graphics g = Graphics.FromHwnd(Handle);
				foreach(TreeListViewItem item in items)
					item.DrawIntermediateState(g);
				g.Dispose();
			}
			internal void DrawSelectedItemsFocusCues()
			{
				if(_updating) return;
				if((HideSelection && !Focused) || !_useXPHighLightStyle) return;
				Debug.Assert(!InvokeRequired);
				SelectedTreeListViewItemCollection items = SelectedItems;
				if(FocusedItem != null && Focused)
					FocusedItem.DrawFocusCues();
				foreach(TreeListViewItem temp in items)
					temp.DrawFocusCues();
			}
			internal void DrawPlusMinusItems()
			{
				if(_updating) return;
				Graphics g = Graphics.FromHwnd(Handle);
				TreeListViewItemCollection items = GetVisibleItems();
				foreach(TreeListViewItem item in items)
					item.DrawPlusMinus(g);
				g.Dispose();
			}
			internal void DrawPlusMinusItemsLines()
			{
				if(_updating) return;
				Graphics g = Graphics.FromHwnd(Handle);
				TreeListViewItemCollection items = GetVisibleItems();
				foreach(TreeListViewItem item in items)
					item.DrawPlusMinusLines(g);
				g.Dispose();
			}
			#endregion
		#endregion

		#region Functions
			#region SetSelectedItemsRange
			private void SetSelectedItemsRange(TreeListViewItem item1, TreeListViewItem item2)
			{
				if(InvokeRequired)
					throw(new Exception("Invoke required"));
				if(item1 == null || item2 == null) return;
				if(!item1.Visible || !item2.Visible) return;
				if(item1.Parent != item2.Parent) return;
				TreeListViewItemCollection items = item1.Container;
				int index1 = items.GetIndexOf(item1);
				int index2 = items.GetIndexOf(item2);
				ListViewItem[] selItems = new ListViewItem[base.SelectedItems.Count];
				base.SelectedItems.CopyTo(selItems, 0);
				foreach(ListViewItem selItem in selItems)
				{
					int selItemIndex = items.GetIndexOf((TreeListViewItem)selItem);
					if(selItemIndex < Math.Min(index1, index2) ||
						selItemIndex > Math.Max(index1, index2))
						selItem.Selected = false;
				}
				for(int i = Math.Min(index1, index2); i <= Math.Max(index1, index2); i++)
					if(!items[i].Selected) items[i].Selected = true;
			}
			#endregion
			#region ExpandAll / CollapseAll
			/// <summary>
			/// Expands all the tree nodes
			/// </summary>
			public void ExpandAll()
			{
				if(InvokeRequired)
					throw(new Exception("Invoke required"));
				BeginUpdate();
				foreach(TreeListViewItem item in Items)
					item.ExpandAllInternal();
				EndUpdate();
			}
			/// <summary>
			/// Collapses all the tree nodes
			/// </summary>
			public void CollapseAll()
			{
				if(InvokeRequired)
					throw(new Exception("Invoke required"));
				BeginUpdate();
				foreach(TreeListViewItem item in Items)
					item.CollapseAllInternal();
				EndUpdate();
			}
			#endregion
			#region ExitEdit
			internal void ExitEdit(bool Cancel, string Text)
			{
				if(!InEdit || EditedItem.Item == null) return;
				// Mouse position
				Point pos = EditedItem.Item != null ?
					EditedItem.Item.GetBounds(TreeListViewItemBoundsPortion.Icon).Location :
					new Point(0, 0);
				pos.Offset(1,1);
				EditItemInformations editedItem = EditedItem;

				Message m = Message.Create(Handle, (int) APIsEnums.WindowMessages.LBUTTONDOWN, (IntPtr)1, (IntPtr) ((pos.Y << 16) + pos.X));
				_skipMouseDownEvent = true;
				base.WndProc(ref m);
				_skipMouseDownEvent = false;
				if(!Cancel)
				{
					TreeListViewLabelEditEventArgs e = new TreeListViewLabelEditEventArgs(EditedItem.Item, EditedItem.ColumnIndex, Text);
					OnAfterLabelEdit(e);
					if(!e.Cancel)
						editedItem.Item.SubItems[
							editedItem.ColumnIndex].Text = Text;
				}
				_inedit = false;
				_editeditem = new EditItemInformations(null, 0, "");
			}
			#endregion
			#region GetItemRect
			/// <summary>
			/// Retrieves the specified portion of the bounding rectangle for a specific item within the list view control
			/// </summary>
			/// <param name="index">The zero-based index of the item within the ListView.ListViewItemCollection whose bounding rectangle you want to return</param>
			/// <param name="portion">One of the TreeListViewItemBoundsPortion values that represents a portion of the TreeListViewItem for which to retrieve the bounding rectangle</param>
			/// <returns>A Rectangle that represents the bounding rectangle for the specified portion of the specified TreeListViewItem</returns>
			public Rectangle GetItemRect(int index, TreeListViewItemBoundsPortion portion)
			{
				if(index >= base.Items.Count || index < 0)
					throw(new Exception("Out of range exception"));
				TreeListViewItem item = (TreeListViewItem) base.Items[index];
				return item.GetBounds(portion);
			}
			#endregion
			#region KillFocus
			/// <summary>
			/// Kill the focus of the control
			/// </summary>
			public void KillFocus()
			{
				APIsUser32.SendMessage(
					Handle,
					(int) APIsEnums.WindowMessages.KILLFOCUS,
					IntPtr.Zero,
					IntPtr.Zero);
			}
			#endregion
			#region OnItemCheck
			/// <summary>
			/// Raises the ItemCheck event
			/// </summary>
			/// <param name="e">An ItemCheckEventArgs that contains the event data</param>
			protected override void OnItemCheck(System.Windows.Forms.ItemCheckEventArgs e)
			{
				base.OnItemCheck(e);
				ListView.ListViewItemCollection baseItems = base.Items;
				if(e.Index >= base.Items.Count || e.Index < 0)
					return;
				TreeListViewItem item = (TreeListViewItem) base.Items[e.Index];
				if(item == null) return; 
				if(this._checkDirection == CheckDirection.None) return; 
				CheckDirection oldDirection = _checkDirection; 

				TreeListViewItem parentItem = item.Parent;
				if(parentItem != null && (oldDirection & CheckDirection.Upwards) == CheckDirection.Upwards)
				{
					_checkDirection = CheckDirection.Upwards;
					while(parentItem != null)
					{
						if(e.NewValue == CheckState.Checked)
						{
							if(!parentItem.Checked)
							{
								parentItem.Checked = true;
								break;
							}
							else
							{
								bool allChecked = true;
								foreach(TreeListViewItem childItem in parentItem.Items)
								{
									if(childItem == item) continue;
									if(!childItem.Checked)
									{
										allChecked = false;
										break;
									}
								} 
								if(allChecked) parentItem.Redraw();
							}
						}
						else
						{
							bool allUnChecked = true;
							foreach(TreeListViewItem childItem in parentItem.Items)
							{
								if(childItem == item) continue;
								if(childItem.Checked)
								{
									allUnChecked = false;
									break;
								}
							} 
							if(allUnChecked && parentItem.Checked)
							{
								parentItem.Checked = false;
								break;
							}
						}
						parentItem = parentItem.Parent;
					}
				}

				if((oldDirection & CheckDirection.Downwards) == CheckDirection.Downwards)
				{
					_checkDirection = CheckDirection.Downwards;
					foreach(TreeListViewItem childItem in item.Items)
						childItem.Checked = e.NewValue == CheckState.Checked;
				}
				_checkDirection = oldDirection;
			}
			#endregion
			#region OnColumnClick
			/// <summary>
			/// Raises the ColumnClick event
			/// </summary>
			/// <param name="e">A ColumnClickEventArgs that contains the event data</param>
			protected override void OnColumnClick(System.Windows.Forms.ColumnClickEventArgs e) 
			{ 
				base.OnColumnClick(e);
				Cursor = Cursors.WaitCursor;
				ListViewItem[] selItems = new ListViewItem[base.SelectedItems.Count];
				base.SelectedItems.CopyTo(selItems, 0);

				// Must set ListView.checkDirection to CheckDirection.None. 
				// Forbid recursively checking. 
				CheckDirection oldDirection = _checkDirection;
				_checkDirection = CheckDirection.None;
				BeginUpdate();
				if(Comparer.Column == e.Column)
					Sorting = (Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);
				else
				{
					Comparer.Column = e.Column;
					Items.SortOrderRecursivelyWithoutSort = SortOrder.Ascending;
					try{Items.Sort(true);}
					catch{}
				}

				if(FocusedItem != null) FocusedItem.EnsureVisible();
				foreach(ListViewItem item in selItems)
					if(item.Index > -1) item.Selected = true;
				EndUpdate();
				// Reset ListView.checkDirection
				_checkDirection = oldDirection;
				Cursor = Cursors.Default;
			}
			#endregion
			#region OnVisibleChanged
			/// <summary>
			/// Raises the VisibleChanged event
			/// </summary>
			/// <param name="e"></param>
			protected override void OnVisibleChanged(EventArgs e)
			{
				base.OnVisibleChanged(e);
				if(base.SmallImageList != _smallimaglist)
					base.SmallImageList = _smallimaglist;
				VisChanged();
			}
			internal void VisChanged()
			{
				if(!Visible) return;
				BeginUpdate();
				try
				{
					foreach(TreeListViewItem item in this.Items)
						item.RefreshIndentation(true);
				}
				catch{}
				if(FocusedItem != null) FocusedItem.EnsureVisible();
				EndUpdate();
			}
			#endregion
			#region GetItemAt
			/// <summary>
			/// Gets an item at the specified coordinates
			/// </summary>
			/// <param name="p">Mouse position</param>
			/// <returns></returns>
			public TreeListViewItem GetItemAt(Point p)
			{
				return(GetItemAt(p.X, p.Y));
			}
			/// <summary>
			/// Gets an item at the specified coordinates (fullrow)
			/// </summary>
			/// <param name="p">Mouse position</param>
			/// <returns></returns>
			public TreeListViewItem GetItemAtFullRow(Point p)
			{
				if(FullRowSelect) return(GetItemAt(p));
				TreeListViewItemCollection items = GetVisibleItems();
				foreach(TreeListViewItem item in items)
					if(item.GetBounds(TreeListViewItemBoundsPortion.Entire).Contains(p))
						return item;
				return null;
			}
			/// <summary>
			/// Gets an item at the specified coordinates.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			new public TreeListViewItem GetItemAt(int x, int y)
			{
				return (TreeListViewItem) base.GetItemAt(x, y);
			}
			#endregion
			#region GetTreeListViewItemFromIndex
			/// <summary>
			/// Gets the TreeListViewItem from the ListView index of the item
			/// </summary>
			/// <param name="index">Index of the Item</param>
			/// <returns></returns>
			public TreeListViewItem GetTreeListViewItemFromIndex(int index)
			{
				if(base.Items.Count < index + 1) return(null);
				return((TreeListViewItem) base.Items[index]);
			}
			#endregion
			#region Sort
			/// <summary>
			/// Not supported (use items.Sort)
			/// </summary>
			new public void Sort()
			{
				if(InvokeRequired)
					throw(new Exception("Invoke required"));
				Items.Sort(true);
			}
			#endregion
			#region Dispose
			/// <summary>
			/// Nettoyage des ressources utilisées.
			/// </summary>
			protected override void Dispose( bool disposing )
			{
				if(disposing)
					if( components != null )
						components.Dispose();
				base.Dispose( disposing );
			}
			#endregion
			#region BeginUpdate / EndUpdate
			/// <summary>
			/// Prevents the control from drawing until the EndUpdate method is called
			/// </summary>
			new public void BeginUpdate()
			{
				_updating = true;
				base.BeginUpdate();
			}
			/// <summary>
			/// Resumes drawing of the list view control after drawing is suspended by the BeginUpdate method
			/// </summary>
			new public void EndUpdate()
			{
				_updating = false;
				base.EndUpdate();
			}
			#endregion
		#endregion
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TreeListView));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.plusMinusImageList = new System.Windows.Forms.ImageList(this.components);
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// plusMinusImageList
			// 
			this.plusMinusImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.plusMinusImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.plusMinusImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("plusMinusImageList.ImageStream")));
			this.plusMinusImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// TreeListView
			// 
			this.FullRowSelect = true;
			this.View = System.Windows.Forms.View.Details;

		}
		#endregion

		#region Column Order
		/// <summary>
		/// Get the index of the specified column from its physical position
		/// </summary>
		/// <param name="columnorder"></param>
		/// <returns></returns>
		public int GetColumnIndex(int columnorder)
		{
			if(columnorder < 0 || columnorder > Columns.Count - 1) return(-1);
			return APIsUser32.SendMessage(Handle, (int)APIsEnums.HeaderControlMessages.ORDERTOINDEX, columnorder, 0);
		}
		/// <summary>
		/// Gets the order of a specified column
		/// </summary>
		/// <param name="columnindex"></param>
		/// <returns></returns>
		public int GetColumnOrder(int columnindex)
		{
			if(this.Columns.Count == 0) return(-1);
			if(columnindex < 0 || columnindex > this.Columns.Count - 1) return(-1);
			IntPtr[] colorderarray = new IntPtr[this.Columns.Count];
			APIsUser32.SendMessage(this.Handle, (int) APIsEnums.ListViewMessages.GETCOLUMNORDERARRAY, (IntPtr) this.Columns.Count, ref colorderarray[0]);
			return((int) colorderarray[columnindex]);
		}
		/// <summary>
		/// Gets the columns order
		/// </summary>
		/// <returns>Example {3,1,4,2}</returns>
		public int[] GetColumnsOrder()
		{
			if(this.Columns.Count == 0) return(new int[] {});
			IntPtr[] colorderarray = new IntPtr[this.Columns.Count];
			try
			{
				APIsUser32.SendMessage(this.Handle, (int) APIsEnums.ListViewMessages.GETCOLUMNORDERARRAY, (IntPtr) this.Columns.Count, ref colorderarray[0]);
			}
			catch{}
			int[] colorderarrayint = new int[this.Columns.Count];
			for(int i = 0 ; i < this.Columns.Count ; i ++)
				colorderarrayint[i] = (int) colorderarray[i];
			return(colorderarrayint);
		}
		/// <summary>
		/// Indicates the column order (for example : {0,1,3,2})
		/// </summary>
		/// <param name="colorderarray"></param>
		public void SetColumnsOrder(int[] colorderarray)
		{
			if(this.Columns.Count == 0) return;
			if(colorderarray.Length != this.Columns.Count) return;
			if(colorderarray[0] != 0) return;
			IntPtr[] colorderarrayintptr = new IntPtr[this.Columns.Count];
			for(int i = 0 ; i < this.Columns.Count ; i ++)
				colorderarrayintptr[i] = (IntPtr) colorderarray[i];
			try
			{
				APIsUser32.SendMessage(this.Handle, (int) APIsEnums.ListViewMessages.SETCOLUMNORDERARRAY, (IntPtr) this.Columns.Count, ref colorderarrayintptr[0]);
			}
			catch{}
			Refresh();
		}

		private void _scroll()
		{
			while(MouseButtons == MouseButtons.Middle)
			{
				int dx = MousePosition.Y - _mousescrollposition.Y;
				int dy = MousePosition.Y - _mousescrollposition.Y;
				Scroll(
					dx,
					dy);
				Threading.Thread.Sleep(100);
			}
			Cursor = Cursors.Default;
		}
		
		/// <summary>
		/// Scrolls the control
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void Scroll(int x, int y)
		{
			APIsUser32.SendMessage(Handle, (int) APIsEnums.ListViewMessages.SCROLL, x, y);
		}
		/// <summary>
		/// Indicates the column order (for example : "3142")
		/// </summary>
		/// <param name="colorder"></param>
		public void SetColumnsOrder(string colorder)
		{
			if(colorder == null) return;
			int[] colorderarray = new int[colorder.Length];
			for(int i = 0 ; i < colorder.Length ; i++)
				colorderarray[i] = int.Parse(new String(colorder[i], 1));
			SetColumnsOrder(colorderarray);
		}
		#endregion

		#region Item Region
		/// <summary>
		/// Gets the items that are visible in the TreeListView
		/// </summary>
		/// <returns>A collection of items</returns>
		public TreeListViewItemCollection GetVisibleItems()
		{
			TreeListViewItemCollection visibleItems = new TreeListViewItemCollection();
			if(base.Items.Count == 0) return visibleItems;
			int firstItemIndex = TopItem.Index;
			int itemsPerPageCount =
				APIsUser32.SendMessage(Handle, (int) APIsEnums.ListViewMessages.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
			int lastVisibleItemIndex = firstItemIndex + itemsPerPageCount > base.Items.Count ?
				base.Items.Count : firstItemIndex + itemsPerPageCount;
			for(int i = firstItemIndex; i < lastVisibleItemIndex; i++)
				visibleItems.Add((TreeListViewItem) base.Items[i]);
			return visibleItems;
		}
		/// <summary>
		/// Gets the column at the specified position
		/// </summary>
		/// <param name="p">Point in client coordinates</param>
		/// <returns>The nul zero based index of the column (-1 if failed)</returns>
		public int GetColumnAt(Point p)
		{
			APIsStructs.LVHITTESTINFO hittest = new APIsStructs.LVHITTESTINFO();
			hittest.pt = new APIsStructs.POINTAPI(PointToClient(MousePosition));
			APIsUser32.SendMessage(
				Handle,
				(Int32) APIsEnums.ListViewMessages.SUBITEMHITTEST,
				0,
				ref hittest);
			return(hittest.iSubItem);
		}
		/// <summary>
		/// Get SubItem rectangle
		/// </summary>
		/// <param name="item"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		public Rectangle GetSubItemRect(TreeListViewItem item, int column)
		{
			ListViewItem lvitem = (ListViewItem) item;
			return GetSubItemRect(lvitem.Index, column);
		}
		/// <summary>
		/// Get SubItem rectangle
		/// </summary>
		/// <param name="row"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		public Rectangle GetSubItemRect(int row, int col)
		{
			APIsStructs.RECT rc = new APIsStructs.RECT();
			rc.top = col;
			rc.left = (int)APIsEnums.ListViewSubItemPortion.BOUNDS;
			APIsUser32.SendMessage(Handle, (int)APIsEnums.ListViewMessages.GETSUBITEMRECT,  row, ref rc);
			
			if ( col == 0 )
			{
				// The LVM_GETSUBITEMRECT message does not give us the rectangle for the first subitem
				// since it is not considered a subitem
				// obtain the rectangle for the header control and calculate from there
				Rectangle headerRect = GetHeaderItemRect(col);
				return new Rectangle((int)rc.left, (int)rc.top, (int)headerRect.Width, (int)(rc.bottom-rc.top));
			}
			
			return new Rectangle((int)rc.left, (int)rc.top, (int)(rc.right-rc.left), (int)(rc.bottom-rc.top));
		}
		/// <summary>
		/// Get HeaderItem text
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public string GetHeaderItemText(int index)
		{
			APIsStructs.HDITEM hdi = new APIsStructs.HDITEM();
			hdi.mask = APIsEnums.HeaderItemFlags.TEXT;
			hdi.cchTextMax =  255;
			hdi.pszText = Marshal.AllocHGlobal(255);
			APIsUser32.SendMessage(Handle, APIsEnums.HeaderControlMessages.GETITEMW, index, ref hdi);
			string text = Marshal.PtrToStringAuto(hdi.pszText);
			return text;
		}
		/// <summary>
		/// Get HeaderItem rect
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		protected Rectangle GetHeaderItemRect(int index)
		{
			APIsStructs.RECT rc = new APIsStructs.RECT();
			IntPtr header = APIsUser32.GetDlgItem(Handle, 0);
			APIsUser32.SendMessage(header, (int)APIsEnums.HeaderControlMessages.GETITEMRECT, index, ref rc);
			return new Rectangle((int)rc.left, (int)rc.top, (int)(rc.right-rc.left), (int)(rc.bottom-rc.top));
		}
		/// <summary>
		/// Get row rect
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public Rectangle GetRowRect(int row)
		{
			APIsStructs.RECT rc = new APIsStructs.RECT();
			rc.top = 0;
			rc.left = (int)APIsEnums.ListViewSubItemPortion.BOUNDS;
			APIsUser32.SendMessage(Handle, (int)APIsEnums.ListViewMessages.GETSUBITEMRECT,  row, ref rc);
			return new Rectangle((int)rc.left, (int)rc.top, (int)(rc.right-rc.left), (int)(rc.bottom-rc.top));
		}
		#endregion
	}
	#region TreeListViewItemCheckDirection Enum
	/// <summary>
	/// Check boxes direction in the TreeListView
	/// </summary>
	[Flags]
	[Serializable]
	internal enum CheckDirection
	{
		/// <summary>
		/// Simply check the item
		/// </summary>
		None = 0,
		/// <summary>
		/// Set the indeterminate state to the parent items
		/// </summary>
		Upwards = 1,
		/// <summary>
		/// Check children items recursively
		/// </summary>
		Downwards = 2,
		/// <summary>
		/// Upwards + Downwards
		/// </summary>
		All = 3,
	}
	#endregion 
	#region EditItemInformations
	/// <summary>
	/// Class that contains all informations on an edited item
	/// </summary>
	public struct EditItemInformations
	{
		#region Properties
		internal DateTime CreationTime;
		private string _label;
		/// <summary>
		/// Gets the label of the subitem
		/// </summary>
		public string Label
		{
			get{return _label;}
		}
		private TreeListViewItem _item;
		/// <summary>
		/// Gets the item being edited
		/// </summary>
		public TreeListViewItem Item
		{
			get{return _item;}
		}
		private int _colindex;
		/// <summary>
		/// Gets the number of the subitem
		/// </summary>
		public int ColumnIndex
		{
			get{return _colindex;}
		}
		#endregion
		#region Constructor
		/// <summary>
		/// Creates a new instance of EditItemInformations
		/// </summary>
		/// <param name="item"></param>
		/// <param name="column"></param>
		/// <param name="label"></param>
		public EditItemInformations(TreeListViewItem item, int column, string label)
		{
			_item = item; _colindex = column; _label = label; CreationTime = DateTime.Now;
		}
		#endregion
	}
	#endregion

	#region Event Handlers
	/// <summary>
	/// TreeListViewBeforeLabelEditEventHandler delegate
	/// </summary>
	public delegate void TreeListViewBeforeLabelEditEventHandler(object sender, TreeListViewBeforeLabelEditEventArgs e);
	/// <summary>
	/// TreeListViewItemLabelEditHandler delegate
	/// </summary>
	public delegate void TreeListViewLabelEditEventHandler(object sender, TreeListViewLabelEditEventArgs e);
	/// <summary>
	/// TreeListViewCancelEventHandler delegate
	/// </summary>
	public delegate void TreeListViewCancelEventHandler(object sender, TreeListViewCancelEventArgs e);
	/// <summary>
	/// TreeListViewEventHandler delegate
	/// </summary>
	public delegate void TreeListViewEventHandler(object sender, TreeListViewEventArgs e);
	#endregion
	#region TreeListViewLabelEditEventArgs & TreeListViewBeforeLabelEditEventArgs
	/// <summary>
	/// Arguments of a TreeListViewLabelEdit event.
	/// </summary>
	[Serializable]
	public class TreeListViewLabelEditEventArgs : CancelEventArgs
	{
		#region Properties
		private string _label;
		/// <summary>
		/// Gets the label of the subitem
		/// </summary>
		public string Label
		{
			get{return _label;}
		}
		private TreeListViewItem _item;
		/// <summary>
		/// Gets the item being edited
		/// </summary>
		public TreeListViewItem Item
		{
			get{return _item;}
		}
		internal int _colindex;
		/// <summary>
		/// Gets the number of the subitem
		/// </summary>
		public int ColumnIndex
		{
			get{return _colindex;}
		}
		#endregion
		#region Constructor
		/// <summary>
		/// Creates a new instance of TreeListViewLabelEditEventArgs
		/// </summary>
		/// <param name="item"></param>
		/// <param name="column"></param>
		/// <param name="label"></param>
		public TreeListViewLabelEditEventArgs(TreeListViewItem item, int column, string label) : base()
		{
			_item = item; _colindex = column; _label = label;
		}
		#endregion
	}
	/// <summary>
	/// Arguments of a TreeListViewBeforeLabelEdit event.
	/// </summary>
	[Serializable]
	public class TreeListViewBeforeLabelEditEventArgs : TreeListViewLabelEditEventArgs
	{
		#region Properties
		/// <summary>
		/// Gets or sets the index of the subitem
		/// </summary>
		new public int ColumnIndex
		{
			get{return _colindex;}
			set{_colindex = value;}
		}
		private Control _editor;
		/// <summary>
		/// Gets or sets the editor (a TextBox will be displayed if null)
		/// </summary>
		public Control Editor
		{
			get{return _editor;}
			set{_editor = value;}
		}
		#endregion
		#region Constructor
		/// <summary>
		/// Creates a new instance of TreeListViewBeforeLabelEditEventArgs
		/// </summary>
		/// <param name="item"></param>
		/// <param name="column"></param>
		/// <param name="label"></param>
		public TreeListViewBeforeLabelEditEventArgs(TreeListViewItem item, int column, string label) : base(item, column, label)
		{}
		#endregion
	}
	#endregion
	#region TreeListViewEventArgs
	/// <summary>
	/// Arguments of a TreeListViewEvent
	/// </summary>
	[Serializable]
	public class TreeListViewEventArgs : EventArgs
	{
		private TreeListViewItem _item;
		/// <summary>
		/// Item that will be expanded
		/// </summary>
		public TreeListViewItem Item{get{return(_item);}}
		private TreeListViewAction _action;
		/// <summary>
		/// Action returned by the event
		/// </summary>
		public TreeListViewAction Action{get{return(_action);}}
		/// <summary>
		/// Create a new instance of TreeListViewEvent arguments
		/// </summary>
		/// <param name="item"></param>
		/// <param name="action"></param>
		public TreeListViewEventArgs(TreeListViewItem item, TreeListViewAction action)
		{
			_item = item;
			_action = action;
		}
	}
	/// <summary>
	/// Arguments of a TreeListViewCancelEventArgs
	/// </summary>
	[Serializable]
	public class TreeListViewCancelEventArgs : TreeListViewEventArgs
	{
		private bool _cancel = false;
		/// <summary>
		/// True -> the operation is canceled
		/// </summary>
		public bool Cancel
		{
			get{return(_cancel);}
			set{_cancel = value;}
		}
		/// <summary>
		/// Create a new instance of TreeListViewCancelEvent arguments
		/// </summary>
		/// <param name="item"></param>
		/// <param name="action"></param>
		public TreeListViewCancelEventArgs(TreeListViewItem item, TreeListViewAction action) :
			base(item, action)
		{}
	}
	#endregion

	#region TreeListViewAction
	/// <summary>
	/// TreeListView actions
	/// </summary>
	[Serializable]
	public enum TreeListViewAction
	{
		/// <summary>
		/// By Keyboard
		/// </summary>
		ByKeyboard,
		/// <summary>
		/// ByMouse
		/// </summary>
		ByMouse,
		/// <summary>
		/// Collapse
		/// </summary>
		Collapse,
		/// <summary>
		/// Expand
		/// </summary>
		Expand,
		/// <summary>
		/// Unknown
		/// </summary>
		Unknown
	}
	#endregion
	#region TreeListViewExpandMethod
	/// <summary>
	/// Expand / Collapse method
	/// </summary>
	[Serializable]
	public enum TreeListViewExpandMethod
	{
		/// <summary>
		/// Expand when double clicking on the icon
		/// </summary>
		IconDbleClick,
		/// <summary>
		/// Expand when double clicking on the entire item
		/// </summary>
		EntireItemDbleClick,
		/// <summary>
		/// Expand when double clicking on the item only
		/// </summary>
		ItemOnlyDbleClick,
		/// <summary>
		/// None
		/// </summary>
		None
	}
	#endregion

	#region CheckBoxexTypes
	/// <summary>
	/// Check boxes types for TreeListView control
	/// </summary>
	[Serializable]
	public enum CheckBoxesTypes
	{
		/// <summary>
		/// No CheckBoxes
		/// </summary>
		None,
		/// <summary>
		/// Simple CheckBoxes
		/// </summary>
		Simple,
		/// <summary>
		/// Check children recursively and set indeterminate state for the parents
		/// </summary>
		Recursive,
	}
	#endregion
}
