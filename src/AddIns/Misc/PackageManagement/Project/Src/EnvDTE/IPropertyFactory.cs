// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public interface IPropertyFactory
	{
		Property CreateProperty(string name);
		IEnumerator<Property> GetEnumerator();
	}
}
