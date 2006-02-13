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
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;

namespace WeifenLuo.WinFormsUI
{
	//  This class comes from Jacob Slusser's MdiClientController class.
	//  It has been modified to the coding standards.
	[ToolboxBitmap(typeof(MdiClientController), "Resources.MdiClientController.bmp")]
	internal class MdiClientController : NativeWindow, IComponent, IDisposable
	{
		private bool m_autoScroll = true;
		private Color m_backColor = SystemColors.AppWorkspace;
		private BorderStyle m_borderStyle = BorderStyle.Fixed3D;
		private MdiClient m_mdiClient = null;
		private Image m_image = null;
		private ContentAlignment m_imageAlign = ContentAlignment.MiddleCenter;
		private Form m_parentForm = null;
		private ISite m_site = null;
		private bool m_stretchImage = false;

		public MdiClientController() : this(null)
		{
		}

		public MdiClientController(Form parentForm)
		{
			ParentForm = parentForm;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				lock(this)
				{
					if(Site != null && Site.Container != null)
						Site.Container.Remove(this);

					if(Disposed != null)
						Disposed(this, EventArgs.Empty);
				}
			}
		}
 
		[DefaultValue(true), Category("Layout")]
		[LocalizedDescription("MdiClientController.AutoScroll.Description")]
		public bool AutoScroll
		{
			get { return m_autoScroll; }
			set
			{
				// By default the MdiClient control scrolls. It can appear though that
				// there are no scrollbars by turning them off when the non-client
				// area is calculated. I decided to expose this method following
				// the .NET vernacular of an AutoScroll property.
				m_autoScroll = value;
				if(MdiClient != null)
					UpdateStyles();
			}
		}

		[DefaultValue(typeof(Color), "AppWorkspace")]
		[Category("Appearance")]
		[LocalizedDescription("MdiClientController.BackColor.Description")]
		public Color BackColor
		{
			// Use the BackColor property of the MdiClient control. This is one of
			// the few properties in the MdiClient class that actually works.

			get
			{
				if(MdiClient != null)
					return MdiClient.BackColor;

				return m_backColor;
			}
			set
			{
				m_backColor = value;
				if(MdiClient != null)
					MdiClient.BackColor = value;
			}
		}

		[DefaultValue(BorderStyle.Fixed3D)]
		[Category("Appearance")]
		[LocalizedDescription("MdiClientController.BorderStyle.Description")]
		public BorderStyle BorderStyle
		{
			get { return m_borderStyle; }
			set
			{
				// Error-check the enum.
				if(!Enum.IsDefined(typeof(BorderStyle), value))
					throw new InvalidEnumArgumentException();

				m_borderStyle = value;

				if(MdiClient == null)
					return;

				// This property can actually be visible in design-mode,
				// but to keep it consistent with the others,
				// prevent this from being show at design-time.
				if(Site != null && Site.DesignMode)
					return;

				// There is no BorderStyle property exposed by the MdiClient class,
				// but this can be controlled by Win32 functions. A Win32 ExStyle
				// of WS_EX_CLIENTEDGE is equivalent to a Fixed3D border and a
				// Style of WS_BORDER is equivalent to a FixedSingle border.
				
				// This code is inspired Jason Dori's article:
				// "Adding designable borders to user controls".
				// http://www.codeproject.com/cs/miscctrl/CsAddingBorders.asp

				// Get styles using Win32 calls
				int style = User32.GetWindowLong(MdiClient.Handle, (int)Win32.GetWindowLongIndex.GWL_STYLE);
				int exStyle = User32.GetWindowLong(MdiClient.Handle, (int)Win32.GetWindowLongIndex.GWL_EXSTYLE);

				// Add or remove style flags as necessary.
				switch(m_borderStyle)
				{
					case BorderStyle.Fixed3D:
						exStyle |= (int)Win32.WindowExStyles.WS_EX_CLIENTEDGE;
						style &= ~((int)Win32.WindowStyles.WS_BORDER);
						break;

					case BorderStyle.FixedSingle:
						exStyle &= ~((int)Win32.WindowExStyles.WS_EX_CLIENTEDGE);
						style |= (int)Win32.WindowStyles.WS_BORDER;
						break;

					case BorderStyle.None:
						style &= ~((int)Win32.WindowStyles.WS_BORDER);
						exStyle &= ~((int)Win32.WindowExStyles.WS_EX_CLIENTEDGE);
						break;
				}
					
				// Set the styles using Win32 calls
				User32.SetWindowLong(MdiClient.Handle, (int)Win32.GetWindowLongIndex.GWL_STYLE, style);
				User32.SetWindowLong(MdiClient.Handle, (int)Win32.GetWindowLongIndex.GWL_EXSTYLE, exStyle);

				// Cause an update of the non-client area.
				UpdateStyles();
			}
		}

		[Browsable(false)]
		public MdiClient MdiClient
		{
			get { return m_mdiClient; }
		}

		[Browsable(false)]
		public new IntPtr Handle
		{
			// Hide this property during design-time.
			get { return base.Handle; }
		}

		[DefaultValue(null)]
		[Category("Appearance")]
		[LocalizedDescription("MdiClientController.Image.Description")]
		public Image Image
		{
			get { return  m_image; }
			set
			{
				m_image = value;
				if(MdiClient != null)
					MdiClient.Invalidate();
			}
		}

		[DefaultValue(ContentAlignment.MiddleCenter)]
		[Category("Appearance")]
		[LocalizedDescription("MdiClientController.ImageAlign.Description")]
		public ContentAlignment ImageAlign
		{
			get { return m_imageAlign; }
			set
			{
				// Error-check the enum.
				if(!Enum.IsDefined(typeof(ContentAlignment), value))
					throw new InvalidEnumArgumentException();

				m_imageAlign = value;
				if(MdiClient != null)
					MdiClient.Invalidate();
			}
		}

		[Browsable(false)]
		public Form ParentForm
		{
			get	{	return m_parentForm;	}
			set
			{
				// If the ParentForm has previously been set,
				// unwire events connected to the old parent.
				if(m_parentForm != null)
				{
					m_parentForm.HandleCreated -= new EventHandler(ParentFormHandleCreated);
					m_parentForm.MdiChildActivate -= new EventHandler(ParentFormMdiChildActivate);
				}

				m_parentForm = value;

				if(m_parentForm == null)
					return;

				// If the parent form has not been created yet,
				// wait to initialize the MDI client until it is.
				if(m_parentForm.IsHandleCreated)
				{
					InitializeMdiClient();
					RefreshProperties();
				}
				else
					m_parentForm.HandleCreated += new EventHandler(ParentFormHandleCreated);

				m_parentForm.MdiChildActivate += new EventHandler(ParentFormMdiChildActivate);
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISite Site
		{
			get { return m_site; }
			set
			{
				m_site = value;

				if(m_site == null)
					return;

				// If the component is dropped onto a form during design-time,
				// set the ParentForm property.
				IDesignerHost host = (value.GetService(typeof(IDesignerHost)) as IDesignerHost);
				if(host != null)
				{
					Form parent = host.RootComponent as Form;
					if(parent != null)
						ParentForm = parent;
				}
			}
		}

		[Category("Appearance"), DefaultValue(false)]
		[LocalizedDescription("MdiClientController.StretchImage.Description")]
		public bool StretchImage
		{
			get { return m_stretchImage; }
			set
			{
				m_stretchImage = value;
				if(MdiClient != null)
					MdiClient.Invalidate();
			}
		}

		public void RenewMdiClient()
		{
			// Reinitialize the MdiClient and its properties.
			InitializeMdiClient();
			RefreshProperties();
		}

		[Browsable(false)]
		public event EventHandler Disposed;

		[Browsable(false)]
		public event EventHandler HandleAssigned;

		[Browsable(false)]
		public event EventHandler MdiChildActivate;

		[Browsable(false)]
		public event LayoutEventHandler Layout;

		protected virtual void OnHandleAssigned(EventArgs e)
		{
			// Raise the HandleAssigned event.
			if(HandleAssigned != null)
				HandleAssigned(this, e);
		}

		protected virtual void OnMdiChildActivate(EventArgs e)
		{
			// Raise the MdiChildActivate event
			if (MdiChildActivate != null)
				MdiChildActivate(this, e);
		}

		protected virtual void OnLayout(LayoutEventArgs e)
		{
			// Raise the Layout event
			if (Layout != null)
				Layout(this, e);
		}

		[Category("Appearance")]
		[LocalizedDescription("MdiClientController.Paint.Description")]
		public event PaintEventHandler Paint;

		protected virtual void OnPaint(PaintEventArgs e)
		{
			// Raise the Paint event.
			if(Paint != null)
				Paint(this, e);
		}

		protected override void WndProc(ref Message m)
		{
			switch(m.Msg)
			{
				//Do all painting in WM_PAINT to reduce flicker.
				case (int)Win32.Msgs.WM_ERASEBKGND:
					return;

				case (int)Win32.Msgs.WM_PAINT:

					// This code is influenced by Steve McMahon's article:
					// "Painting in the MDI Client Area".
					// http://vbaccelerator.com/article.asp?id=4306

					// Use Win32 to get a Graphics object.
					Win32.PAINTSTRUCT paintStruct = new Win32.PAINTSTRUCT();
					IntPtr screenHdc = User32.BeginPaint(m.HWnd, ref paintStruct);

					using(Graphics screenGraphics = Graphics.FromHdc(screenHdc)) 
					{
						// Get the area to be updated.
						Rectangle clipRect = paintStruct.rcPaint;

						// Double-buffer by painting everything to an image and
						// then drawing the image.
						int width = (MdiClient.ClientRectangle.Width > 0 ? MdiClient.ClientRectangle.Width : 0);
						int height = (MdiClient.ClientRectangle.Height > 0 ? MdiClient.ClientRectangle.Height : 0);
						if (width > 0 && height > 0)
						{
							using(Image i = new Bitmap(width, height))
							{
								using(Graphics g = Graphics.FromImage(i))
								{
									// This code comes from J Young's article:
									// "Generating missing Paint event for TreeView and ListView".
									// http://www.codeproject.com/cs/miscctrl/genmissingpaintevent.asp

									// Draw base graphics and raise the base Paint event.
									IntPtr hdc = g.GetHdc();
									Message printClientMessage = Message.Create(m.HWnd, (int)Win32.Msgs.WM_PRINTCLIENT, hdc, IntPtr.Zero);  
									DefWndProc(ref printClientMessage);
									g.ReleaseHdc(hdc);

									// Draw the image here.
									if(Image != null)
										DrawImage(g, clipRect);

									// Call our OnPaint here to draw graphics over the
									// original and raise our Paint event.
									OnPaint(new PaintEventArgs(g, clipRect));
								}

								// Now draw all the graphics at once.
								screenGraphics.DrawImage(i, MdiClient.ClientRectangle);
							}
						}
					}

					User32.EndPaint(m.HWnd, ref paintStruct);
					return;
	
				case (int)Win32.Msgs.WM_SIZE:
					// Repaint on every resize.
					MdiClient.Invalidate();
					break;
					

				case (int)Win32.Msgs.WM_NCCALCSIZE:
					// If AutoScroll is set to false, hide the scrollbars when the control
					// calculates its non-client area.
					if(!AutoScroll)
						User32.ShowScrollBar(m.HWnd, (int)Win32.ScrollBars.SB_BOTH, 0 /*false*/);
					break;
			}
		
			base.WndProc(ref m);
		}

		private void ParentFormHandleCreated(object sender, EventArgs e)
		{
			// The form has been created, unwire the event, and initialize the MdiClient.
			this.m_parentForm.HandleCreated -= new EventHandler(ParentFormHandleCreated);
			InitializeMdiClient();
			RefreshProperties();
		}

		private void ParentFormMdiChildActivate(object sender, EventArgs e)
		{
			OnMdiChildActivate(e);
		}

		private void MdiClientLayout(object sender, LayoutEventArgs e)
		{
			OnLayout(e);
		}

		private void MdiClientHandleDestroyed(object sender, EventArgs e)
		{
			// If the MdiClient handle has been released, drop the reference and
			// release the handle.
			if(m_mdiClient != null)
			{
				m_mdiClient.HandleDestroyed -= new EventHandler(MdiClientHandleDestroyed);
				m_mdiClient = null;
			}

			ReleaseHandle();
		}

		private void InitializeMdiClient()
		{
			// If the mdiClient has previously been set, unwire events connected
			// to the old MDI.
			if(MdiClient != null)
			{
				MdiClient.HandleDestroyed -= new EventHandler(MdiClientHandleDestroyed);
				MdiClient.Layout -= new LayoutEventHandler(MdiClientLayout);
			}

			if(ParentForm == null)
				return;

			// Get the MdiClient from the parent form.
			foreach (Control control in ParentForm.Controls)
			{
				// If the form is an MDI container, it will contain an MdiClient control
				// just as it would any other control.

				m_mdiClient = control as MdiClient;
				if(m_mdiClient == null)
					continue;

				// Assign the MdiClient Handle to the NativeWindow.
				ReleaseHandle();
				AssignHandle(MdiClient.Handle);

				// Raise the HandleAssigned event.
				OnHandleAssigned(EventArgs.Empty);

				// Monitor the MdiClient for when its handle is destroyed.
				MdiClient.HandleDestroyed += new EventHandler(MdiClientHandleDestroyed);
				MdiClient.Layout += new LayoutEventHandler(MdiClientLayout);

				break;
			}
		}

		private void RefreshProperties()
		{
			// Refresh all the properties
			BackColor = m_backColor;
			BorderStyle = m_borderStyle;
			AutoScroll = m_autoScroll;
			Image = m_image;
			ImageAlign = m_imageAlign;
			StretchImage = m_stretchImage;
		}

		private void DrawImage(Graphics g, Rectangle clipRect)
		{
			// Paint the background image.
			if(StretchImage)
				g.DrawImage(this.Image, MdiClient.ClientRectangle);
			else
			{
				// Calculate the location of the image. (Note: this logic could be calculated during sizing
				// instead of during painting to improve performance.)
				Point pt = Point.Empty;
				switch(ImageAlign)
				{
					case ContentAlignment.TopLeft:
						pt = new Point(0, 0);
						break;
					case ContentAlignment.TopCenter:
						pt = new Point((MdiClient.ClientRectangle.Width / 2) - (Image.Width / 2), 0);
						break;
					case ContentAlignment.TopRight:
						pt = new Point(MdiClient.ClientRectangle.Width - Image.Width, 0);
						break;
					case ContentAlignment.MiddleLeft:
						pt = new Point(0, (MdiClient.ClientRectangle.Height / 2) - (Image.Height / 2));
						break;
					case ContentAlignment.MiddleCenter:
						pt = new Point((MdiClient.ClientRectangle.Width / 2) - (Image.Width / 2),
							(MdiClient.ClientRectangle.Height / 2) - (Image.Height / 2));
						break;
					case ContentAlignment.MiddleRight:
						pt = new Point(MdiClient.ClientRectangle.Width - Image.Width,
							(MdiClient.ClientRectangle.Height / 2) - (Image.Height / 2));
						break;
					case ContentAlignment.BottomLeft:
						pt = new Point(0, MdiClient.ClientRectangle.Height - Image.Height);
						break;
					case ContentAlignment.BottomCenter:
						pt = new Point((MdiClient.ClientRectangle.Width / 2) - (Image.Width / 2),
							MdiClient.ClientRectangle.Height - Image.Height);
						break;
					case ContentAlignment.BottomRight:
						pt = new Point(MdiClient.ClientRectangle.Width - Image.Width,
							MdiClient.ClientRectangle.Height - Image.Height);
						break;
				}

				// Paint the image with the calculated coordinates and image size.
				g.DrawImage(Image, new Rectangle(pt, Image.Size));
			}
		}

		private void UpdateStyles()
		{
			// To show style changes, the non-client area must be repainted. Using the
			// control's Invalidate method does not affect the non-client area.
			// Instead use a Win32 call to signal the style has changed.
			User32.SetWindowPos(MdiClient.Handle, IntPtr.Zero, 0, 0, 0, 0,
				Win32.FlagsSetWindowPos.SWP_NOACTIVATE |
				Win32.FlagsSetWindowPos.SWP_NOMOVE |
				Win32.FlagsSetWindowPos.SWP_NOSIZE |
				Win32.FlagsSetWindowPos.SWP_NOZORDER |
				Win32.FlagsSetWindowPos.SWP_NOOWNERZORDER |
				Win32.FlagsSetWindowPos.SWP_FRAMECHANGED);
		}
	}
}
