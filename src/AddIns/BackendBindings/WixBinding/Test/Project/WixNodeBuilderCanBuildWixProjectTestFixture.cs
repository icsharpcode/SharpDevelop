// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the WixProjectNodeBuilder can build project nodes for a
	/// WixProject.
	/// </summary>
	[TestFixture]
	public class WixNodeBuilderCanBuildWixProjectTestFixture
	{	
		[Test]
		public void CanBuildWixProject()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			WixProjectNodeBuilder builder = new WixProjectNodeBuilder();
			Assert.IsTrue(builder.CanBuildProjectTree(p));
		}
	}
}
