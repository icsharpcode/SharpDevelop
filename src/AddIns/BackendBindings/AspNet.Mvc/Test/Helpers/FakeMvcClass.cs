// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.AspNet.Mvc;

namespace AspNet.Mvc.Tests.Helpers
{
	public class FakeMvcClass : IMvcClass
	{
		public FakeMvcClass(string fullyQualifiedName)
		{
			this.FullyQualifiedName = fullyQualifiedName;
		}
		
		public FakeMvcClass(string @namespace, string name)
		{
			this.Namespace = @namespace;
			this.Name = name;
		}
		
		public string FullyQualifiedName { get; set; }
		public string Name { get; set; }
		public string Namespace { get; set; }
	}
}
