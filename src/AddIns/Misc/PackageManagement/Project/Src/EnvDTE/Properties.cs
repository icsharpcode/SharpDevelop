// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Properties : MarshalByRefObject, IEnumerable
	{
		IPropertyFactory propertyFactory;
		
		public Properties(IPropertyFactory propertyFactory)
		{
			this.propertyFactory = propertyFactory;
		}
		
		public Properties()
		{
		}
		
		public virtual Property Item(string propertyName)
		{
			return propertyFactory.CreateProperty(propertyName);
		}
		
		public virtual IEnumerator GetEnumerator()
		{
			return propertyFactory.GetEnumerator();
		}
	}
}
