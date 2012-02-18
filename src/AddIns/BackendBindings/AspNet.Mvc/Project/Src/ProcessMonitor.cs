// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
