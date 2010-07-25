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

using NoGoop.Controls;
using NoGoop.ObjBrowser.LinkHelpers;
using NoGoop.Util;

namespace NoGoop.ObjBrowser.Panels
{


	// Used to track the detail elements and sort them when its
	// time to render
	internal class DetailElement : IComparable
	{
		internal int                _order;
		internal bool               _internal;
		internal String             _name;
		internal String             _value;
		internal bool               _isLink;
		internal ILinkTarget        _linkTarget;
		internal Object             _linkModifier;
		internal Object             _helpLinkMod;

		public int CompareTo(Object obj)
		{
			if (obj != null && 
				obj is DetailElement)
			{
				DetailElement de = (DetailElement)obj;
				if (_order < de._order)
					return -1;
				if (_order > de._order)
					return 1;
				return 0;
			}
			throw new ArgumentException();
		}        

		public override bool Equals(Object obj)
		{
			if (obj != null && 
				obj is DetailElement)
			{
				DetailElement de = (DetailElement)obj;
				if (de._name.Equals(_name) &&
					de._order.Equals(_order))
					return true;
			}
			return false;
		}        

		public override int GetHashCode()
		{
			return _order.GetHashCode() + _name.GetHashCode();
		}        

		public override String ToString()
		{
			return _name + " " + _value;
		}

	}

	internal class DetailPanel : Panel
	{

		protected const int             LABEL_WIDTH_PAD = 5;
		protected const int             SPACING_HEIGHT = 5;

		internal bool                   _needsReorder;
		protected static DetailPanel    _detailPanel;
		protected ArrayList             _elements;
		protected int                   _maxLabelWidth;


		internal static Panel Panel
		{
			get
				{
					return _detailPanel;
				}
		}


		internal DetailPanel() : base()
		{
			_detailPanel = this;
			_elements = new ArrayList();
			AutoScroll = true;
			BorderStyle = BorderStyle.Fixed3D;
		}


		internal static void Clear()
		{
			_detailPanel.SuspendLayout();
			_detailPanel.Controls.Clear();
			_detailPanel._needsReorder = true;
			_detailPanel._elements.Clear();
			_detailPanel._maxLabelWidth = 0;
			StatusPanel.Clear();
			_detailPanel.ResumeLayout();
		}

		internal static void Add(String title,
								 bool isInternal,
								 int order,
								 String value)
		{
			AddInternal(title, isInternal, order, value);
		}


		internal static void AddLink(String title,
									 bool isInternal,
									 int order,
									 ILinkTarget linkTarget,
									 Object linkModifier)
		{
			AddLink(title, isInternal, order, linkTarget, 
					linkModifier, null);
		}

		// linkTarget is the class that knows how to display
		// the link, and is called to execute the link
		// linkModifier is the data passed to the linkTarget
		// helpLinkMod is a linkModifier for the HelpLinkHelper
		// to display help associated with the link
		internal static void AddLink(String title,
									 bool isInternal,
									 int order,
									 ILinkTarget linkTarget,
									 Object linkModifier,
									 Object helpLinkMod)
		{
			DetailElement de = 
				AddInternal(title, isInternal, order, null);
			de._isLink = true;
			de._value = linkTarget.GetLinkName(linkModifier);
			de._linkTarget = linkTarget;
			de._linkModifier = linkModifier;
			de._helpLinkMod = helpLinkMod;
		}


		protected static DetailElement AddInternal(String title,
												   bool isInternal,
												   int order,
												   String value)
		{
			DetailElement de = new DetailElement();
			de._name = title;
			de._value = value;
			de._order = order;
			de._internal = isInternal;

			if (ShouldShow(de))
			{
				_detailPanel._maxLabelWidth = 
					Utils.SetMaxWidth(_detailPanel, title, 
									  _detailPanel._maxLabelWidth);
			}

			_detailPanel._elements.Add(de);
			_detailPanel.Invalidate();
			return de;
		}


		internal Control AddDetailControl(DetailElement de)
		{
			Panel detailLine = new Panel();
			detailLine.Dock = DockStyle.Top;
			detailLine.Layout += new LayoutEventHandler(DetailLayoutHandler);
			//detailLine.Width = _detailPanel.Width;

			RichTextBox tb = null;
			Control valueControl = null;

			if (de._isLink)
			{
				if (de._helpLinkMod != null)
				{
					LinkLabel ll = new LinkLabel();
					ll.Dock = DockStyle.Left;
					ll.AutoSize = true;
					ll.Text = "Show Documentation";
					ll.Links.Add(0, ll.Text.Length, de);
					ll.LinkClicked += 
						new LinkLabelLinkClickedEventHandler(HelpLinkClicked);
					detailLine.Controls.Add(ll);

					// Padding
					Label l = new Label();
					l.Dock = DockStyle.Left;
					l.Width = 40;
					detailLine.Controls.Add(ll);
				}

				LinkLabel linkLabel = new LinkLabel();
				valueControl = linkLabel;
				linkLabel.AutoSize = true;
				linkLabel.Text = de._value;
				linkLabel.Links.Add(0, linkLabel.Text.Length, de);
				linkLabel.LinkClicked += 
					new LinkLabelLinkClickedEventHandler(LinkClicked);

				// We assume the label is going to be one line and since
				// we don't have the resize handler for the text box, just
				// wire it into the standard height
				detailLine.Height = linkLabel.Height;
			}
			else
			{
				tb = new RichTextBox();
				valueControl = tb;
				// The value
				tb.TabStop = false;
				tb.Multiline = true;
				tb.WordWrap = true;
				tb.ReadOnly = true;
				tb.Text = (String)de._value;
				tb.DetectUrls = false;
				tb.BorderStyle = BorderStyle.None;
				tb.BackColor = BackColor; 
				tb.Layout += new LayoutEventHandler(TextLayoutHandler);
			}

			detailLine.Controls.Add(valueControl);

			// Label for each line
			if (de._name != null)
			{
				// For the help link, we can't fill
				if (de._isLink && de._helpLinkMod != null)
					valueControl.Dock = DockStyle.Left;
				else
					valueControl.Dock = DockStyle.Fill;

				Label l;

				// spacing
				l = new Label();
				l.Dock = DockStyle.Left;
				l.Width = SPACING_HEIGHT;
				detailLine.Controls.Add(l);

				// The actual label
				l = new Label();
				l.Dock = DockStyle.Left;
				// FIXME Because the last character is slightly cut off
				l.Text = de._name + "                              ";
				//l.Text = de._name;
				l.Width = _maxLabelWidth + LABEL_WIDTH_PAD + 20;
				l.TextAlign = ContentAlignment.TopRight;
				l.Font = new Font(l.Font, FontStyle.Bold);
				detailLine.Controls.Add(l);
			}
			else
			{
				// No text label - Makes the value bold
				valueControl.Font = new Font(valueControl.Font, FontStyle.Bold);
			}

			if (tb != null)
				TextLayoutHandler(tb, new LayoutEventArgs(tb, null));

			return detailLine;
		}


		protected void TextLayoutHandler(object sender, 
										 LayoutEventArgs e)
		{

			RichTextBox tb = (RichTextBox)sender;
			// Get number of actual lines the text box needs 
			// given the current width
			int lines = (1 + tb.GetLineFromCharIndex(tb.TextLength));
			tb.Height = tb.PreferredHeight * lines;
			tb.Parent.Height = tb.Height;
		}


		protected void DetailLayoutHandler(object sender, 
										   LayoutEventArgs e)
		{

			// This code all exists to handle only the case of
			// a detailLine panel with only one control, we need
			// to force the width of that control to be the right
			// thing.  Seems complicated to do this, in a future
			// .NET version maybe this can be removed.
			Panel p = (Panel)sender;
			if (p.Parent != null)
			{
				p.Width = p.Parent.Width;
				// This is the case where there is no label text, 
				// for some reason we must force the width.
				if (p.Controls.Count == 1)
					((Control)p.Controls[0]).Width = p.Parent.Width;
			}
		}

		protected void AddSpace(ArrayList c)
		{
			Label l = new Label();
			l.Dock = DockStyle.Top;
			l.Height = SPACING_HEIGHT;
			c.Add(l);
		}


		protected static bool ShouldShow(DetailElement de)
		{
			if (!de._internal ||
				(LocalPrefs.Get(LocalPrefs.SHOW_INTERNAL_DETAILS) != null))
				return true;
			return false;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (_needsReorder)
			{
				ArrayList c = new ArrayList();
				_elements.Sort();

				AddSpace(c);
				foreach (DetailElement de in _elements)
				{
					if (ShouldShow(de))
					{
						Control detailLine = AddDetailControl(de);
						c.Add(detailLine);
					}
				}

				SuspendLayout();
				Utils.AddControls(this, c);
				ResumeLayout();

				// Need to force another layout becuase the layout
				// handlers of the detail panels changed their height but
				// that was not taken into consideration in the height
				// calculation for autoscroll, so need to do it again
				// to get the scroll right.
				PerformLayout();
				_needsReorder = false;
			}

			base.OnPaint(e);
		}

		protected void ShowLink(ILinkTarget target,
								Object linkModifier)
		{
			// Links can sometimes take a while
			Cursor save = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				target.ShowTarget(linkModifier);
			}
			catch (Exception ex)
			{
				ErrorDialog.Show(ex,
							"(bug, please report) Unexpected exception "
							+ "showing link",
							"Unexpected Exception Showing Link",
							MessageBoxIcon.Error);
			}
			Cursor.Current = save;
		}

		protected void LinkClicked(object sender, 
								   LinkLabelLinkClickedEventArgs e)
		{
			DetailElement de = (DetailElement)e.Link.LinkData;
			ShowLink(de._linkTarget, de._linkModifier);
		}

		protected void HelpLinkClicked(object sender, 
								   LinkLabelLinkClickedEventArgs e)
		{
			DetailElement de = (DetailElement)e.Link.LinkData;
			ShowLink(HelpLinkHelper.HLHelper, de._helpLinkMod);
		}
	}
}
