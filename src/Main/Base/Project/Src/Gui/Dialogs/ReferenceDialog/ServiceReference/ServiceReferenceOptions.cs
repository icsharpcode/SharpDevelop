// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public static class ServiceReferenceOptions
	{
		static Properties properties = PropertyService.Get("ServiceReferenceOptions", new Properties());
		
		public static bool HasSvcUtilPath {
			get { return !String.IsNullOrEmpty(SvcUtilPath); }
		}
		
		public static string SvcUtilPath {
			get { return properties.Get<string>("SvcUtilPath", String.Empty); }
			set { properties.Set("SvcUtilPath", value); }
		}
	}
}
