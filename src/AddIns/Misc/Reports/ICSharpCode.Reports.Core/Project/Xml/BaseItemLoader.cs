// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core
{
	public class BaseItemLoader : MycroParser
	{
		protected override Type GetTypeByName(string ns, string name)
		{
			return typeof(BaseSection).Assembly.GetType(typeof(BaseSection).Namespace + "." + name);
		}
	}
}
