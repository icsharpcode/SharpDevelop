// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;

namespace ICSharpCode.AspNet.Mvc
{
	public static class IISExpressProcessStartInfo
	{
		public static ProcessStartInfo Create(WebProject project)
		{
			return new ProcessStartInfo(WebProjectService.IISExpressProcessLocation, GetSiteArgument(project));
		}
		
		static string GetSiteArgument(WebProject project)
		{
			return String.Format("/site:{0}", GetSiteName(project));
		}
		
		static string GetSiteName(WebProject project)
		{
			if (project.Name.Contains(" ")) {
				return String.Format("\"{0}\"", project.Name);
			}
			return project.Name;
		}
	}
}
