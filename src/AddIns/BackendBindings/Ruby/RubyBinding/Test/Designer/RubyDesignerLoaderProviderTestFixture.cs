// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.RubyBinding;
using NUnit.Framework;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests the RubyDesignerLoaderProvider class.
	/// </summary>
	[TestFixture]
	public class RubyDesignerLoaderProviderTestFixture
	{
		RubyDesignerLoaderProvider provider;
		RubyDesignerGenerator generator;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			provider = new RubyDesignerLoaderProvider();
			generator = new RubyDesignerGenerator(null);
		}
		
		[Test]
		public void RubyDesignerLoaderCreated()
		{
			DesignerLoader loader = provider.CreateLoader(generator);
			using (IDisposable disposable = loader as IDisposable) {
				Assert.IsInstanceOf(typeof(RubyDesignerLoader), loader);
			}
		}
		
		[Test]
		public void CodeDomProviderIsNull()
		{
			Assert.IsNull(generator.CodeDomProvider);
		}
	}
}
