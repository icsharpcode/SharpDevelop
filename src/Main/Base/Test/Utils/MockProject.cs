// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

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
