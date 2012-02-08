// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class TestableMvcTextTemplateHost : MvcTextTemplateHost
	{
		public TestableMvcTextTemplateHost()
			: base(null, null, null)
		{
		}
		
		public Assembly AssemblyToReturnFromLoadAssemblyFrom;
		public string FileNamePassedToLoadAssemblyFrom;
		
		protected override Assembly LoadAssemblyFrom(string fileName)
		{
			FileNamePassedToLoadAssemblyFrom = fileName;
			return AssemblyToReturnFromLoadAssemblyFrom;
		}
	}
}
