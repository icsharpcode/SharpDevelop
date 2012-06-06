// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

// This file is called 'Project_TypeGuids.cs' with an underscore to work around a bug in Visual Studio:
// http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=481981

using System;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Contains constants for the project type GUIDs.
	/// 
	/// Source: http://www.mztools.com/Articles/2008/MZ2008017.aspx
	/// </summary>
	public static class ProjectTypeGuids
	{
		public const string CSharp = "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
		public const string VBNet = "{F184B08F-C81C-45F6-A57F-5ABD9991F28F}";
		public const string CPlusPlus = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
		
		public const string WebApplication = "{349C5851-65DF-11DA-9384-00065B846F21}";
		public const string WebSite = "{E24C65DC-7377-472B-9ABA-BC803B73C61A}";
		public const string Silverlight = "{A1591282-1198-4647-A2B1-27E5FF5F6F3B}";
		public static readonly Guid PortableLibrary = Guid.Parse("{786C830F-07A1-408B-BD7F-6EE04809D6DB}");
	}
}
