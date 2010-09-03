// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel.Design.Serialization;
using ICSharpCode.PythonBinding;
using NUnit.Framework;

namespace PythonBinding.Tests.Designer
{
	/// <summary>
	/// Tests the PythonDesignerLoaderProvider class.
	/// </summary>
	[TestFixture]
	public class PythonDesignerLoaderProviderTestFixture
	{
		PythonDesignerLoaderProvider provider;
		PythonDesignerGenerator generator;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{
			provider = new PythonDesignerLoaderProvider();
			generator = new PythonDesignerGenerator(null);
		}
		
		[Test]
		public void PythonDesignerLoaderCreated()
		{
			DesignerLoader loader = provider.CreateLoader(generator);
			using (IDisposable disposable = loader as IDisposable) {
				Assert.IsInstanceOf(typeof(PythonDesignerLoader), loader);
			}
		}
		
		[Test]
		public void CodeDomProviderIsNull()
		{
			Assert.IsNull(generator.CodeDomProvider);
		}
	}
}
