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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Management;

namespace ICSharpCode.AspNet.Mvc
{
	public class ProcessMonitor : ManagementEventWatcher, IDisposable
	{
		// Process Events
		public event EventHandler ProcessCreated;
		public event EventHandler ProcessDeleted;
		public event EventHandler ProcessModified;

		// WMI WQL process query strings
		static readonly string WMI_OPER_EVENT_QUERY = @"SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";
		static readonly string WMI_OPER_EVENT_QUERY_WITH_PROC = WMI_OPER_EVENT_QUERY + " and TargetInstance.Name LIKE '%{0}%'";

		public ProcessMonitor(string processName)
		{
			this.Query.QueryLanguage = "WQL";
			if (string.IsNullOrEmpty(processName))
			{
				this.Query.QueryString = WMI_OPER_EVENT_QUERY;
			}
			else
			{
				this.Query.QueryString =
					string.Format(WMI_OPER_EVENT_QUERY_WITH_PROC, processName);
			}

			this.EventArrived += new EventArrivedEventHandler(OnEventArrived);
		}
		
		private void OnEventArrived(object sender, EventArrivedEventArgs e)
		{
			string eventType = e.NewEvent.ClassPath.ClassName;
			switch (eventType)
			{
				case "__InstanceCreationEvent":
					if (ProcessCreated != null)
						ProcessCreated(this, EventArgs.Empty);
					break;
				case "__InstanceDeletionEvent":
					if (ProcessDeleted != null)
						ProcessDeleted(this, EventArgs.Empty); 
					break;
				case "__InstanceModificationEvent":
					if (ProcessModified != null)
						ProcessModified(this, EventArgs.Empty);
					break;
			}
		}
	}
}
