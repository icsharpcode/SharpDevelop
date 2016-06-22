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
		public static readonly Guid SolutionFolder = Guid.Parse("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");
		
		public static readonly Guid CSharp    = Guid.Parse("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}");
		public static readonly Guid VB     = Guid.Parse("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}");
		public static readonly Guid CPlusPlus = Guid.Parse("{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}");
		
		public static readonly Guid WebApplication  = Guid.Parse("{349C5851-65DF-11DA-9384-00065B846F21}");
		public static readonly Guid WebSite         = Guid.Parse("{E24C65DC-7377-472B-9ABA-BC803B73C61A}");
		public static readonly Guid Silverlight     = Guid.Parse("{A1591282-1198-4647-A2B1-27E5FF5F6F3B}");
		public static readonly Guid PortableLibrary = Guid.Parse("{786C830F-07A1-408B-BD7F-6EE04809D6DB}");
	}
}
