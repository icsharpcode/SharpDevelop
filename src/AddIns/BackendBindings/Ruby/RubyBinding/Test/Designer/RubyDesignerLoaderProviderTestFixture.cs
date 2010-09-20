// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
