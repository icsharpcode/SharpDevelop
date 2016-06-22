// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.BrowserDisplayBinding
{
	public delegate void NewWindowExtendedEventHandler(object sender, NewWindowExtendedEventArgs e);
	
	public class NewWindowExtendedEventArgs : CancelEventArgs
	{
		Uri url;
		
		public Uri Url {
			get {
				return url;
			}
		}
		
		public NewWindowExtendedEventArgs(Uri url)
		{
			this.url = url;
		}
	}
	
	/// <summary>
	/// Microsoft didn't include the URL being surfed to in the NewWindow event args,
	/// but here is the workaround:
	/// </summary>
	public class ExtendedWebBrowser : WebBrowser
	{
		class WebBrowserExtendedEvents : System.Runtime.InteropServices.StandardOleMarshalObject, DWebBrowserEvents2
		{
			private ExtendedWebBrowser browser;
			
			public WebBrowserExtendedEvents(ExtendedWebBrowser browser)
			{
				this.browser = browser;
			}
			
			public void NewWindow3(object pDisp, ref bool cancel, ref object flags, ref string urlContext, ref string url)
			{
				NewWindowExtendedEventArgs e = new NewWindowExtendedEventArgs(new Uri(url));
				browser.OnNewWindowExtended(e);
				cancel = e.Cancel;
			}
		}
		

		[ComImport()]
		[Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
		[InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIDispatch)]
		[TypeLibType(TypeLibTypeFlags.FHidden)]
		interface DWebBrowserEvents2
		{
			[DispId(273)]
			void NewWindow3([InAttribute(), MarshalAs(UnmanagedType.IDispatch)] object pDisp,
			                [InAttribute(), OutAttribute()]                 ref bool cancel,
			                [InAttribute()]                                 ref object flags,
			                [InAttribute(), MarshalAs(UnmanagedType.BStr)]  ref string urlContext,
			                [InAttribute(), MarshalAs(UnmanagedType.BStr)]  ref string url);
		}
		
		public event NewWindowExtendedEventHandler NewWindowExtended;
		
		private AxHost.ConnectionPointCookie cookie;
		private WebBrowserExtendedEvents wevents;
		
		protected override void CreateSink()
		{
			base.CreateSink();
			wevents = new WebBrowserExtendedEvents(this);
			cookie = new AxHost.ConnectionPointCookie(this.ActiveXInstance, wevents, typeof(DWebBrowserEvents2));
		}
		
		protected override void DetachSink()
		{
			if (cookie != null) {
				cookie.Disconnect();
				cookie = null;
			}
			base.DetachSink();
		}
		
		protected virtual void OnNewWindowExtended(NewWindowExtendedEventArgs e)
		{
			if (NewWindowExtended != null) {
				NewWindowExtended(this, e);
			}
		}
	}
}
