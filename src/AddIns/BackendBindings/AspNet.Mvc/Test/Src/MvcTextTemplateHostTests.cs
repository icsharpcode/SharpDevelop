// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;
using NUnit.Framework;

namespace AspNet.Mvc.Tests
{
	[TestFixture]
	public class MvcTextTemplateHostTests
	{
		MvcTextTemplateHost host;
		
		void CreateHost()
		{
			host = new MvcTextTemplateHost(null, null, null);
		}
		
		[Test]
		public void ViewName_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ViewName = null;
			string viewName = host.ViewName;
			
			Assert.AreEqual(String.Empty, viewName);
		}
		
		[Test]
		public void ControllerName_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.ControllerName = null;
			string controllerName = host.ControllerName;
			
			Assert.AreEqual(String.Empty, controllerName);
		}
		
		[Test]
		public void Namespace_SetToNull_ReturnsEmptyString()
		{
			CreateHost();
			host.Namespace = null;
			string ns = host.Namespace;
			
			Assert.AreEqual(String.Empty, ns);
		}
	}
}
