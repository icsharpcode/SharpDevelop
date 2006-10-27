// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using NoGoop.Controls;
using NoGoop.Obj;
using NoGoop.Util;
using NoGoop.Win32;

namespace NoGoop.ObjBrowser.LinkHelpers
{
	internal class HelpLinkItem
	{
		internal String                 _helpFile;
		internal int                    _helpContext;
		internal HelpLinkItem(String file,
							  int context)
		{
			_helpFile = file;
			_helpContext = context;
		}
		public override String ToString()
		{
			if (_helpContext != 0)
				return "0x" + _helpContext.ToString("X");
			else
				return _helpFile;
		}
	}
	// A single instance of this is used to resolve links
	// to either a help file, or a context within a help file.
	internal class HelpLinkHelper : ILinkTarget
	{
		protected static HelpLinkHelper _helpLinkHelper;
		protected Hashtable             _windowsHash = Hashtable.Synchronized(new Hashtable());
		public static HelpLinkHelper HLHelper
		{
			get {
				return _helpLinkHelper;
			}
		}
		
		static HelpLinkHelper()
		{
			_helpLinkHelper = new HelpLinkHelper();
		}
		
		internal static Object MakeLinkModifier(String helpFile, int context)
		{
			if (helpFile != null)
				return new HelpLinkItem(helpFile, context);
			return null;
		}
		
		internal static Object MakeLinkModifier(BasicInfo basicInfo)
		{
			if (basicInfo._helpFile == null)
				return null;
			
			return new HelpLinkItem(basicInfo._helpFile, basicInfo._helpContext);
		}
		
		// Make a link modifier suitable for COM help from the type
		internal static Object MakeLinkModifier(Type type)
		{
			BasicInfo basicInfo = TypeLibrary.FindMemberByType(type, TypeLibrary.WARN);
			if (basicInfo == null)
				return null;
			return MakeLinkModifier(basicInfo);
		}
		
		// Make a link modifier suitable for COM help from the method
		internal static Object MakeLinkModifier(MemberInfo memberInfo)
		{
			if (memberInfo.DeclaringType == null)
				return null;
			// Get the type
			BasicInfo typeInfo = 
				TypeLibrary.FindMemberByType(memberInfo.DeclaringType,
											 TypeLibrary.WARN);
			if (typeInfo == null)
				return null;
			// Get the member
			BasicInfo comMemberInfo = 
				typeInfo.FindMemberByName(memberInfo.Name);
			if (comMemberInfo == null)
				return null;
			
			return MakeLinkModifier(comMemberInfo);
		}
		
		// For linking to the type information for this object
		// The linkModifier is always a HelpLinkItem for this class
		public String GetLinkName(Object linkModifier)
		{
			return linkModifier.ToString();
		}
		
		// The linkModifier is always a HelpLinkItem for this class
		public void ShowTarget(Object linkModifier)
		{
			HelpLinkItem item = (HelpLinkItem)linkModifier;
			if (item._helpFile == null)
				return;
			Uri uri = new Uri(item._helpFile);
			if (!File.Exists(uri.LocalPath))
			{
				// Try it with adding in the language
				StringBuilder basePath = new StringBuilder();
				for (int i = 0; i < uri.Segments.Length - 1; i++)
					basePath.Append(uri.Segments[i]);
				basePath.Append(CultureInfo.CurrentUICulture.LCID);
				basePath.Append("/");
				basePath.Append(uri.Segments[uri.Segments.Length - 1]);
				basePath.Replace("%20", " ");
				basePath.Replace("%00", "");
				Uri secondUri = new Uri(basePath.ToString());
				if (!File.Exists(secondUri.LocalPath))
				{
					// FIXME - for some reason this does not seem to show
					// the second file, probably because there is something
					// in the first file name that terminates the string
					ErrorDialog.Show("Unable to open help file " 
									+ uri.LocalPath
									+ " (also tried " 
									+ secondUri.LocalPath + ")",
									"Unable to Open Help File",
									MessageBoxIcon.Error);
					return;
				}
				uri = secondUri;
			}
			TraceUtil.WriteLineInfo(this,
									"Help: " + uri + " " 
									+ item._helpContext);
			int helpWindow;
			if (item._helpContext != 0)
			{
				helpWindow = 
					Windows.HtmlHelp(ObjectBrowserForm.Instance.Handle,
									 uri.LocalPath,
									 Windows.HH_HELP_CONTEXT ,
									 item._helpContext);
				/*
				   This seems to be broken, workaround is above
				Help.ShowHelp(ObjectBrowser.ObjBrowser,
							  uri.LocalPath,
							  HelpNavigator.Topic,
							  item._helpContext);
				*/
			}
			else
			{
				helpWindow = Windows.HtmlHelp(ObjectBrowserForm.Instance.Handle,
									 uri.LocalPath,
									 Windows.HH_DISPLAY_TOPIC,
									 0);
			}
			TraceUtil.WriteLineInfo(this,
									"Help window: " + helpWindow);
			CheckPosition(helpWindow);
		}
		
		protected void CheckPosition(int helpWindow)
		{
			TraceUtil.WriteLineInfo(this, "Setting position: " + helpWindow);
			// If we have not heard of this window, set its size
			lock (this)
			{
				if (_windowsHash[helpWindow] == null)
				{
					_windowsHash.Add(helpWindow, helpWindow);
					Control c = ObjectBrowserForm.Instance;
					// Get placement (max, min, normal)
					WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
					wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
					Windows.GetWindowPlacement(c.Handle, ref wp);
					// Put the window in normal state if its not
					if (wp.showCmd != Windows.SW_SHOW_NORMAL)
					{
						wp.showCmd = Windows.SW_RESTORE;
						Windows.SetWindowPlacement(c.Handle, ref wp);
					}
					int deskTop = Windows.GetDesktopWindow();
					WINDOWINFO wi = new WINDOWINFO();
					wi.cbSize = Marshal.SizeOf(typeof(WINDOWINFO));
					Windows.GetWindowInfo(deskTop, ref wi);

					Rectangle p = c.Bounds;
					p.X = 0;
					p.Width = (int)wi.rcClient.right / 2;
					c.Bounds = p;
					// Set the size to split the width of the screen
					// between the two windows
					Windows.SetWindowPos(helpWindow,
										 Windows.HWND_TOP,
										 c.Bounds.X + c.Bounds.Width,
										 c.Bounds.Y,
										 wi.rcClient.right - c.Bounds.Width,
										 c.Bounds.Height,
										 0);
				}
			}
		}
	}
}
