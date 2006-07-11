using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices; 
using System.Drawing.Drawing2D;

namespace SharpReport.Designer
{
	/// <summary>
	/// Draw a Focus and DragRectangle
	/// http://www.codeproject.com/csharp/SharpFormEditorDemo.asp
	/// <seealso cref="http://www.codeproject.com/csharp/SharpFormEditorDemo.asp">
	/// </seealso>
	/// </summary>
	public class RectTracker
	{
		#region Win32API
		protected struct POINT
		{
			public Int32 x;
			public Int32 y;
		};
		
		protected struct MSG
		{
			public Int32 hwnd;
			public Int32 message;
			public Int32 wParam;
			public Int32 lParam;
			public Int32 time;
			public POINT pt;
		};

		[DllImport("user32.dll", SetLastError=true )]
		protected static extern Int32 GetMessage (ref MSG lpMsg,Int32 hwnd,Int32 wMsgFilterMin,Int32 wMsgFilterMax);

		[DllImport("user32.dll", SetLastError=true )]
		protected static extern Int32 DispatchMessage (ref MSG lpMsg);

		[DllImport("user32.dll", SetLastError=true )]
		private static extern Int32 TranslateMessage (ref MSG lpMsg);

		private const int CX_BORDER=1;
		private const int CY_BORDER=1;
		protected const int WM_MOUSEFIRST    = 0x0200;
		protected const int WM_MOUSEMOVE     = 0x0200;
		protected const int WM_LBUTTONDOWN   = 0x0201;
		protected const int WM_LBUTTONUP     = 0x0202;
		protected const int WM_LBUTTONDBLCLK = 0x0203;
		protected const int WM_RBUTTONDOWN   = 0x0204;
		protected const int WM_RBUTTONUP     = 0x0205;
		protected const int WM_RBUTTONDBLCLK = 0x0206;
		protected const int WM_MBUTTONDOWN   = 0x0207;
		protected const int WM_MBUTTONUP     = 0x0208;
		protected const int WM_MBUTTONDBLCLK = 0x0209;
		protected const int WM_KEYDOWN = 0x100; 
		protected const int WM_KEYUP = 0x101; 
		private static Cursor[]	Cursors =new Cursor[10];
		private static HatchBrush HatchBrush = null;
		private static Pen BlackDottedPen = null;
		private static int HandleSize = 4;
		private static Pen DotedPen=null;
		
		#endregion

		//
		// Style Flags
		public enum StyleFlags
		{
			solidLine = 1, dottedLine = 2, hatchedBorder = 4,
			resizeInside = 8, resizeOutside = 16, hatchInside = 32,
		};

		// Hit-Test codes
		public enum TrackerHit
		{
			hitNothing = -1,
			hitTopLeft = 0, hitTopRight = 1, hitBottomRight = 2, hitBottomLeft = 3,
			hitTop = 4, hitRight = 5, hitBottom = 6, hitLeft = 7, hitMiddle = 8
		};
		public enum backMode
		{
			TRANSPARENT=		 1,
			OPAQUE=				 2
		};
		public	enum rectPos
		{
			Left	=0,
			Right,
			Top,
			Bottom
		};
		struct HANDLEINFO
		{
			public HANDLEINFO(rectPos X,rectPos Y,int CX,int CY,int HX,int HY,int IX,int IY)
			{
				nOffsetX=X;
				nOffsetY=Y;
				nCenterX=CX;
				nCenterY=CY;
				nHandleX=HX;
				nHandleY=HY;
				nInvertX=IX;
				nInvertY=IY;
			}
			public rectPos nOffsetX;	    // offset within RECT for X coordinate
			public rectPos nOffsetY;		// offset within RECT for Y coordinate
			public int nCenterX;			// adjust X by Width()/2 * this number
			public int nCenterY;			// adjust Y by Height()/2 * this number
			public int nHandleX;			// adjust X by handle size * this number
			public int nHandleY;			// adjust Y by handle size * this number
			public int nInvertX;			// handle converts to this when X inverted
			public int nInvertY;			// handle converts to this when Y inverted
		};
		struct RECTINFO
		{
			public RECTINFO(rectPos offset,int nsign)
			{
				nOffsetAcross=offset;
				nSignAcross=nsign;
			}
			public rectPos nOffsetAcross; // offset of opposite point (ie. left->right)
			public int nSignAcross;        // sign relative to that point (ie. add/subtract)
		}

		static HANDLEINFO[] HandleInfo=new HANDLEINFO[]{
														   // corner handles (top-left, top-right, bottom-right, bottom-left
														   new HANDLEINFO(rectPos.Left, rectPos.Top,0, 0,  0,  0, 1, 3 ),
														   new HANDLEINFO(rectPos.Right,rectPos.Top,0, 0, -1,  0, 0, 2),
														   new HANDLEINFO(rectPos.Right,rectPos.Bottom,0, 0, -1, -1, 3, 1),
														   new HANDLEINFO(rectPos.Left, rectPos.Bottom, 0, 0,  0, -1, 2, 0 ),
														   // side handles (top, right, bottom, left)
														   new HANDLEINFO(rectPos.Left, rectPos.Top,1, 0,  0,  0, 4, 6),
														   new HANDLEINFO(rectPos.Right,rectPos.Top,0, 1, -1,  0, 7, 5),
														   new HANDLEINFO(rectPos.Left, rectPos.Bottom, 1, 0,  0, -1, 6, 4 ),
														   new HANDLEINFO(rectPos.Left, rectPos.Top,0, 1,  0,  0, 5, 7)
													   };
		static RECTINFO[] RectInfo=new RECTINFO[]{
													 new RECTINFO(rectPos.Right, +1),
													 new RECTINFO(rectPos.Bottom, +1),
													 new RECTINFO(rectPos.Left,-1),
													 new RECTINFO(rectPos.Top, -1 )
												 };
		// Attributes
		public StyleFlags m_nStyle;      // current state
		public Rectangle m_rect;       // current position (always in pixels)
		public Size m_sizeMin;    // minimum X and Y size during track operation
		public int m_nHandleSize=0;  // size of resize handles (default from WIN.INI)
		protected bool m_bAllowInvert=false;    // flag passed to Track or TrackRubberBand
		protected Rectangle m_rectLast;
		protected Size m_sizeLast;
		protected bool m_bErase=false;          // TRUE if DrawTrackerRect is called for erasing
		protected bool m_bFinalErase=false;     // TRUE if DragTrackerRect called for final erase
		protected static bool bInitialized=false;

		public RectTracker()
		{
			Construct();
			m_nHandleSize = 6;
			m_nStyle = StyleFlags.resizeOutside;
		}
		public RectTracker(Rectangle rect, StyleFlags nStyle)
		{
			Construct();
			m_rect=rect;
			m_nStyle = nStyle;
		}
		protected void Construct()
		{	
			if(false==bInitialized)
			{
				// initialize the cursor array
				Cursors[0] = System.Windows.Forms.Cursors.SizeNWSE;
				Cursors[1] = System.Windows.Forms.Cursors.SizeNESW;
				Cursors[2] = Cursors[0];
				Cursors[3] = Cursors[1];
				Cursors[4] = System.Windows.Forms.Cursors.SizeNS;
				Cursors[5] = System.Windows.Forms.Cursors.SizeWE;
				Cursors[6] = Cursors[4];
				Cursors[7] = Cursors[5];
				Cursors[8] = System.Windows.Forms.Cursors.SizeAll;
				Cursors[9] = System.Windows.Forms.Cursors.PanSW;
				bInitialized = true;
				BlackDottedPen=new Pen(System.Drawing.Color.Black,1);
				HatchBrush=new HatchBrush(HatchStyle.Percent20,Color.Black,Color.FromArgb(0));
				DotedPen=new Pen(Color.Black,1);
				DotedPen.DashStyle=DashStyle.Dot;
			}
			m_nStyle = 0;
			m_nHandleSize = HandleSize;
			m_sizeMin.Height = m_sizeMin.Width = m_nHandleSize*2;

			m_rectLast=new Rectangle(0,0,0,0);
			m_sizeLast.Width = m_sizeLast.Height = 0;
			m_bErase = false;
			m_bFinalErase =  false;
		}


		// Operations
		public virtual void Draw(Graphics gs)
		{
			System.Drawing.Drawing2D.GraphicsState OldState=gs.Save();
			
//			IntPtr hdc = new IntPtr();
			// get normalized rectangle
            Rectangle rect = m_rect;
            NormalizeRect(ref rect);

			// draw lines
			if ((m_nStyle & (StyleFlags.dottedLine|StyleFlags.solidLine)) != 0)
			{
				if((m_nStyle&StyleFlags.dottedLine)!=0)
					BlackDottedPen.DashStyle=DashStyle.Dot;
				else
					BlackDottedPen.DashStyle=DashStyle.Solid;
				rect.Inflate(new Size(+1, +1));   // borders are one pixel outside
				gs.DrawRectangle(BlackDottedPen,rect);
			}

			
			// hatch inside
			if ((m_nStyle & StyleFlags.hatchInside) != 0)
			{
				gs.FillRectangle(HatchBrush,rect.Left+1, rect.Top+1, rect.Width-1, rect.Height-1);
			}

			// draw hatched border
			if (true)//(m_nStyle & StyleFlags.hatchedBorder) != 0)
			{
				Rectangle rectTrue=new Rectangle(0,0,0,0);
				GetTrueRect(ref rectTrue);
				gs.FillRectangle(HatchBrush,rectTrue.Left, rectTrue.Top, rectTrue.Width,rect.Top-rectTrue.Top);
				gs.FillRectangle(HatchBrush,rectTrue.Left, rect.Bottom,rectTrue.Width,rectTrue.Bottom-rect.Bottom);
				gs.FillRectangle(HatchBrush,rectTrue.Left, rect.Top, rect.Left-rectTrue.Left,rect.Height);
				gs.FillRectangle(HatchBrush,rect.Right, rect.Top, rectTrue.Right-rect.Right,rect.Height);
			}

			// draw resize handles
			if ((m_nStyle & (StyleFlags.resizeInside|StyleFlags.resizeOutside)) != 0)
			{
				uint mask = GetHandleMask();
				for (int i = 0; i < 8; ++i)
				{
					if ((mask&(1<<i))!=0)
					{
						GetHandleRect(i, ref rect);
						SolidBrush brush=new SolidBrush(Color.Black);
						Pen pen = new Pen(brush,1);
						SolidBrush brushWhite = new SolidBrush(Color.White);
						gs.FillRectangle(brushWhite,rect);
						gs.DrawRectangle(pen,rect);
					}
				}
			}
			gs.Restore(OldState);
		}
		public void GetTrueRect(ref Rectangle TrueRect)
		{
            // get normalized rectangle
            Rectangle rect = m_rect;
            NormalizeRect(ref rect);
			int nInflateBy = 0;
			if ((m_nStyle & (StyleFlags.resizeOutside|StyleFlags.hatchedBorder)) != 0)
				nInflateBy += GetHandleSize(new Rectangle(0,0,0,0)) - 1;
			if ((m_nStyle & (StyleFlags.solidLine|StyleFlags.dottedLine)) != 0)
				++nInflateBy;
			rect.Inflate(new Size(nInflateBy, nInflateBy));
			TrueRect = rect;
		}

		public virtual bool SetCursor(Control frm, uint nHitTest,Point MousePoint) 
		{
			// trackers should only be in client area
			frm.PointToClient(MousePoint);
			if (!frm.ClientRectangle.Contains(MousePoint))
				return false;

			// convert cursor position to client co-ordinates
			// do hittest and normalize hit
			int nHandle = (int)HitTestHandles(MousePoint);
			System.Console.WriteLine("RectTracker SetCursor {0}",nHandle);
			if (nHandle < 0)
				return false;

			// need to normalize the hittest such that we get proper cursors
			nHandle = NormalizeHit(nHandle);

			// handle special case of hitting area between handles
			//  (logically the same -- handled as a move -- but different cursor)
			if (nHandle == (int)TrackerHit.hitMiddle && !m_rect.Contains(MousePoint))
			{
				// only for trackers with hatchedBorder (ie. in-place resizing)
				if ((m_nStyle & StyleFlags.hatchedBorder)!=0)
					nHandle = 9;
			}

			Debug.Assert(nHandle < 10);
			frm.Cursor=Cursors[nHandle];
			return true;
		}
		public virtual bool Track(Control frm, Point point, bool bAllowInvert,Form frmClipTo)
		{
			// perform hit testing on the handles
			int nHandle = (int)HitTestHandles(point);
			if (nHandle < 0)
			{
				// didn't hit a handle, so just return FALSE
				return false;
			}

			// otherwise, call helper function to do the tracking
			m_bAllowInvert = bAllowInvert;
			return TrackHandle(nHandle, frm, point, frmClipTo);
		}
		public bool TrackRubberBand(Control frm, Point point, bool bAllowInvert)
		{
			// simply call helper function to track from bottom right handle
			m_bAllowInvert = bAllowInvert;
			m_rect=new Rectangle(point.X, point.Y,0, 0);
			return TrackHandle((int)TrackerHit.hitBottomRight, frm, point, null);
		}
		public virtual TrackerHit HitTest(Point point) 
		{
			TrackerHit hitResult = TrackerHit.hitNothing;

			Rectangle rectTrue=new Rectangle();
			GetTrueRect(ref rectTrue);
			Debug.Assert(rectTrue.Left <= rectTrue.Right);
			Debug.Assert(rectTrue.Top <= rectTrue.Bottom);

			if (rectTrue.Contains(point))
			{
				if ((m_nStyle & (StyleFlags.resizeInside|StyleFlags.resizeOutside)) != 0)
					hitResult = HitTestHandles(point);
				else
					hitResult = TrackerHit.hitMiddle;
			}
			return hitResult;
		}
		protected int NormalizeHit(int nHandle) 
		{
			Debug.Assert(nHandle <= 8 && nHandle >= -1);
			if (nHandle == (int)TrackerHit.hitMiddle || nHandle ==(int)TrackerHit.hitNothing)
				return nHandle;

			HANDLEINFO handleInfo = HandleInfo[nHandle];
			if (m_rect.Width< 0)
			{
				nHandle = handleInfo.nInvertX;
				handleInfo = HandleInfo[nHandle];
			}
			if (m_rect.Height< 0)
				nHandle = handleInfo.nInvertY;
			return nHandle;
		}
		

		private void DrawDragRect(Graphics gs,Rectangle rect,Rectangle rectLast)
		{
			ControlPaint.DrawReversibleFrame(rectLast,Color.White,FrameStyle.Dashed);
			ControlPaint.DrawReversibleFrame(rect,Color.White,FrameStyle.Dashed);

		}

		public virtual void DrawTrackerRect(Rectangle Rect, Form ClipToFrm,Graphics gs,Control frm)
		{

			Rectangle rect = Rect;
			// convert to client coordinates
			if (ClipToFrm != null)
			{
				rect=ClipToFrm.RectangleToScreen(rect);
				rect=ClipToFrm.RectangleToClient(rect);
			}

			Size size=new Size(0, 0);
			if (!m_bFinalErase)
			{
				// otherwise, size depends on the style
				if ((m_nStyle & StyleFlags.hatchedBorder)!=0)
				{
					size.Width = size.Height = Math.Max(1, GetHandleSize(rect)-1);
					rect.Inflate(size);
				}
				else
				{
					size.Width = CX_BORDER;
					size.Height = CY_BORDER;
				}
			}

			if (m_bFinalErase || !m_bErase)
			{
				Rectangle rcScreen = frm.RectangleToScreen(rect);
				Rectangle rcLast = frm.RectangleToScreen(m_rectLast);
				DrawDragRect(gs,rcScreen,rcLast);
			}
			// remember last rectangles
			m_rectLast = rect;
			m_sizeLast = size;
		}

		public virtual void AdjustRect(int nHandle, ref Rectangle Rect)
		{
			if (nHandle ==(int)TrackerHit.hitMiddle)
				return;

			// convert the handle into locations within m_rect
			int px=-1, py=-1;
			int ix=-1,iy=-1;
			GetModifyPointers(nHandle,ref px, ref py,ref ix, ref iy,false);

			// enforce minimum width
			int nNewWidth = m_rect.Width;
			int nAbsWidth = m_bAllowInvert ? Math.Abs(nNewWidth) : nNewWidth;
			if (nAbsWidth < m_sizeMin.Width)
			{
				nNewWidth = nAbsWidth != 0 ? nNewWidth / nAbsWidth : 1;
				RECTINFO refrectinfo=RectInfo[px];
				px = GetRectInt((int)refrectinfo.nOffsetAcross)  +
					nNewWidth * m_sizeMin.Width * -refrectinfo.nSignAcross;
			}

			// enforce minimum height
			int nNewHeight = m_rect.Height;
			int nAbsHeight = m_bAllowInvert ? Math.Abs(nNewHeight) : nNewHeight;
			if ((py != -1)&&(nAbsHeight < m_sizeMin.Height))
			{
				nNewHeight = nAbsHeight != 0 ? nNewHeight / nAbsHeight : 1;
				Debug.Assert(py<4);
				RECTINFO refrectinfo=RectInfo[py];
				py = GetRectInt((int)refrectinfo.nOffsetAcross) +					
					nNewHeight * m_sizeMin.Width * -refrectinfo.nSignAcross;
			}
		}

		public virtual void OnChangedRect(Rectangle rectOld)
		{
		}

		public virtual uint GetHandleMask()
		{
			uint mask = 0x0F;   // always have 4 corner handles
			int size = m_nHandleSize*3;
			if (Math.Abs(m_rect.Width) - size > 4)
				mask |= 0x50;
			if (Math.Abs(m_rect.Height) - size > 4)
				mask |= 0xA0;
			return mask;
		}
		
		protected virtual TrackerHit HitTestHandles(Point point)
		{
			Rectangle Truerect=new Rectangle(0,0,0,0);
			uint mask = GetHandleMask();

			// see if hit anywhere inside the tracker
			GetTrueRect(ref Truerect);
			if (!Truerect.Contains(point))
				return TrackerHit.hitNothing;  // totally missed

			// see if we hit a handle
			for (int i = 0; i < 8; ++i)
			{
				if((mask&(1<<i))!=0)
				{
					GetHandleRect(i, ref Truerect);
					if (Truerect.Contains(point))
						return (TrackerHit)i;
				}
			}

			// last of all, check for non-hit outside of object, between resize handles
			if ((m_nStyle & StyleFlags.hatchedBorder) == 0)
			{
                // get normalized rectangle
                Rectangle rect = m_rect;
                NormalizeRect(ref rect);
				if ((m_nStyle & (StyleFlags.dottedLine|StyleFlags.solidLine)) != 0)
					rect.Inflate(+1, +1);
				if (!rect.Contains(point))
					return TrackerHit.hitNothing;  // must have been between resize handles
			}
			return TrackerHit.hitMiddle;   // no handle hit, but hit object (or object border)
		}

		protected void GetHandleRect(int nHandle, ref Rectangle Changerect)
		{
			System.Diagnostics.Debug.Assert(nHandle < 8);

            // get normalized rectangle of the tracker
            Rectangle rectT = m_rect;
            NormalizeRect(ref rectT);

			if ((m_nStyle & (StyleFlags.solidLine|StyleFlags.dottedLine)) != 0)
				rectT.Inflate(+1, +1);

			// since the rectangle itself was normalized, we also have to invert the
			//  resize handles.
			nHandle = NormalizeHit(nHandle);

			// handle case of resize handles outside the tracker
			int size = GetHandleSize(new Rectangle());
			if ((m_nStyle&StyleFlags.resizeOutside)!=0)
				rectT.Inflate(size, size);

			// calculate position of the resize handle
			int nWidth = rectT.Width;
			int nHeight = rectT.Height;
			Rectangle rect=new Rectangle();
			HANDLEINFO handleInfo = HandleInfo[nHandle];
			int[] arr=new int[4]{rectT.Left,rectT.Right,rectT.Top,rectT.Bottom};

			rect.X = arr[(int)handleInfo.nOffsetX];
			rect.Y =	arr[(int)handleInfo.nOffsetY];
			rect.X += size * handleInfo.nHandleX;
			rect.Y += size * handleInfo.nHandleY;
			rect.X += handleInfo.nCenterX * (nWidth - size) / 2;
			rect.Y += handleInfo.nCenterY * (nHeight - size) / 2;
			rect.Width = size;
			rect.Height = size;

			Changerect = rect;
		}

		protected int GetRectInt(int index)
		{
			switch(index) 
			{
				case 0:
					return m_rect.Left;				
				case 1:
					return m_rect.Right;
				case 2:
					return m_rect.Top;
				case 3:
					return m_rect.Bottom;
			default:
					return -1;
			}
		}

		protected void SetRectInt(int index,int p)
		{
			if( p >= 32768)
				p=0;
			switch(index) 
			{
				case 0://left
					m_rect.Width += (m_rect.X - p);					
					m_rect.X = p;
					break;
				case 1://right	
					m_rect.Width = p - m_rect.X;
					break;
				case 2://top
					m_rect.Height += m_rect.Y - p;
					m_rect.Y = p ;
					break;
				case 3://bottom
					m_rect.Height = p - m_rect.Y;
					break;
				default:
					break;
			}

		}

		protected void GetModifyPointers(int nHandle,ref int ppx,ref int ppy,ref int px,ref int py,bool bModify)
		{
			Debug.Assert(nHandle >= 0);
			Debug.Assert(nHandle <= 8);

			if (nHandle ==(int)TrackerHit.hitMiddle)
				nHandle = (int)TrackerHit.hitTopLeft;   // same as hitting top-left

			ppx = -1;
			ppy = -1;

			// fill in the part of the rect that this handle modifies
			//  (Note: handles that map to themselves along a given axis when that
			//   axis is inverted don't modify the value on that axis)

			HANDLEINFO handleInfo = HandleInfo[nHandle];

			if (handleInfo.nInvertX != nHandle)
			{
				ppx=(int)handleInfo.nOffsetX;
				if (bModify)
					px = GetRectInt(ppx);
			}
			else
			{
				// middle handle on X axis
				if (bModify)
					px = m_rect.Left + Math.Abs(m_rect.Width) / 2;
			}
			if (handleInfo.nInvertY != nHandle)
			{
				ppy=(int)handleInfo.nOffsetY;
				if (bModify)
					py = GetRectInt(ppy);
			}
			else
			{
				// middle handle on Y axis
				if (bModify)
					py = m_rect.Top + Math.Abs(m_rect.Height) / 2;
			}
		}

		protected virtual int GetHandleSize(Rectangle rect)
		{
			if (rect.IsEmpty)
				rect = m_rect;

			int size = m_nHandleSize;
			if ((m_nStyle & StyleFlags.resizeOutside)==0)
			{
				// make sure size is small enough for the size of the rect
				int sizeMax = Math.Min(Math.Abs(rect.Right - rect.Left),Math.Abs(rect.Bottom - rect.Top));
				if (size * 2 > sizeMax)
					size = sizeMax / 2;
			}
			return size;
		}

		protected bool TrackHandle(int nHandle,Control frm,Point point,Form frmClipTo)
		{
			Debug.Assert(nHandle >= 0);
			Debug.Assert(nHandle <= 8);   // handle 8 is inside the rect

			// don't handle if capture already set
			//if(frm.Capture) return false;

			Debug.Assert(!m_bFinalErase);

			// save original width & height in pixels
			int nWidth = m_rect.Width;
			int nHeight = m_rect.Height;

			// set capture to the window which received this message
			frm.Capture=true;
			Debug.Assert(frm.Capture);
			frm.Update();
			if (frmClipTo!=null)
				frmClipTo.Update();
			Rectangle rectSave = m_rect;

			// find out what x/y coords we are supposed to modify
			int px=0, py=0;
			int xDiff=0, yDiff=0;
			GetModifyPointers(nHandle,ref px,ref py,ref xDiff,ref yDiff,true);
			xDiff = point.X - xDiff;
			yDiff = point.Y - yDiff;

			// get DC for drawing
			Graphics gs;
			if (frmClipTo!=null)
			{
				// clip to arbitrary window by using adjusted Window DC
				gs=frmClipTo.CreateGraphics();
			}
			else
			{
				// otherwise, just use normal DC
				gs=frm.CreateGraphics();
			}

			Rectangle rectOld;
			bool bMoved = false;

			// get messages until capture lost or cancelled/accepted
			for (;;)
			{
				MSG msg=new MSG();
				if(GetMessage(ref msg, 0, 0, 0)!=1) break;
				if(!frm.Capture) break;

				switch (msg.message)
				{
						// handle movement/accept messages
					case WM_LBUTTONUP:
					case WM_MOUSEMOVE:
						rectOld = m_rect;
						// handle resize cases (and part of move)
						SetRectInt(px,LoWord(msg.lParam) - xDiff);
						SetRectInt(py,HiWord(msg.lParam) - yDiff);
						// handle move case
						if (nHandle == (int)TrackerHit.hitMiddle)
						{
							m_rect.Width=nWidth;
							m_rect.Height=nHeight;
						}
						// allow caller to adjust the rectangle if necessary
						AdjustRect(nHandle,ref m_rect);

						// only redraw and callback if the rect actually changed!
						m_bFinalErase = (msg.message == WM_LBUTTONUP);
						if (m_bFinalErase)
							goto ExitLoop;

						if (!rectOld.Equals(m_rect) || m_bFinalErase)
						{
							if (bMoved)
							{
								m_bErase = true;
								DrawTrackerRect(rectOld, frmClipTo, gs, frm);
							}
							OnChangedRect(rectOld);
							if (msg.message != WM_LBUTTONUP)
								bMoved = true;
						}
						if (m_bFinalErase)
							goto ExitLoop;

						if (!rectOld.Equals(m_rect))
						{
							m_bErase = false;
							DrawTrackerRect(m_rect, frmClipTo, gs, frm);
						}
						break;

						// handle cancel messages
					case WM_KEYDOWN:
						if (msg.wParam != 0x1B)//VK_ESCAPE
							break;
						goto default;
					case WM_RBUTTONDOWN:
						if (bMoved)
						{
							m_bErase = m_bFinalErase = true;
							DrawTrackerRect(m_rect, frmClipTo, gs, frm);
						}
						m_rect = rectSave;
						goto ExitLoop;

						// just dispatch rest of the messages
					default:
						DispatchMessage(ref msg);
						break;
				}
			}

			ExitLoop:
				gs.Dispose();
			frm.Capture=false;
			// restore rect in case bMoved is still FALSE
			if (!bMoved)
				m_rect = rectSave;
			m_bFinalErase = false;
			m_bErase = false;

			// return TRUE only if rect has changed
			return !rectSave.Equals(m_rect);
		}

        public void NormalizeRect(ref Rectangle rect)
        {
            Rectangle rc = new Rectangle(rect.Location,rect.Size);
            if(rect.Left > rect.Right)
            {
                rc.Width = -rect.Width;
                rc.X = rect.Right;
            }
            if(rect.Top > rect.Bottom)
            {
                rc.Height = -rect.Height;
                rc.Y = rect.Bottom;
            }
            rect = rc;
        }
	
		#region Helper functions

		static int MakeLong(int LoWord, int HiWord)  
		{  
			return (HiWord << 16) | (LoWord & 0xffff);  
		} 
 
		static IntPtr MakeLParam(int LoWord, int HiWord)  
		{  
			return (IntPtr) ((HiWord << 16) | (LoWord & 0xffff));  
		} 
 
		static int HiWord(int Number)  
		{  
			return (Number >> 16) & 0xffff;  
		} 
 
		static int LoWord(int Number)  
		{  
			return Number & 0xffff;  
		}
		
		#endregion
	}
}
