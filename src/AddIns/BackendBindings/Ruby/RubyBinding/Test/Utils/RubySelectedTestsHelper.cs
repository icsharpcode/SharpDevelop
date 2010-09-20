// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.RubyBinding;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.UnitTesting;
using UnitTesting.Tests.Utils;

namespace RubyBinding.Tests.Utils
{
	public static class RubySelectedTestsHelper
	{
		public static SelectedTests CreateSelectedTests(IProject project)
		{
			return new SelectedTests(project, null, null, null);
		}
		
		public static SelectedTests CreateSelectedTests(MockMethod method)
		{
			return new SelectedTests(method.MockDeclaringType.Project, null, method.MockDeclaringType, method);
		}
		
		public static SelectedTests CreateSelectedTests(MockClass c)
		{
			return new SelectedTests(c.Project, null, c, null);
		}
	}
}
