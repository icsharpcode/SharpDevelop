// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	/// <summary>
	/// Mocks IProject class that returns a dummy ambience.
	/// </summary>
	public class MockProject : AbstractProject
	{
		public MockProject()
		{
		}
		
		public override IAmbience GetAmbience()
		{
			return null;
		}
	}
}
