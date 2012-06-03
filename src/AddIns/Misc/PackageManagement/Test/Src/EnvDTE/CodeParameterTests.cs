// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NUnit.Framework;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeParameterTests
	{
		CodeParameter parameter;
		
		void CreateParameter()
		{
			parameter = new CodeParameter(null);
		}
		
		[Test]
		public void Kind_Parameter_ReturnsParameter()
		{
			CreateParameter();
			
			vsCMElement kind = parameter.Kind;
			
			Assert.AreEqual(vsCMElement.vsCMElementParameter, kind);
		}
	}
}
