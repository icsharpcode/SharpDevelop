// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public abstract class EnumerableProjectItems : MarshalByRefObject, IEnumerable<global::EnvDTE.ProjectItem>
	{
		public EnumerableProjectItems()
		{
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		public IEnumerator<global::EnvDTE.ProjectItem> GetEnumerator()
		{
			return GetProjectItems().ToList().GetEnumerator();
		}
		
		protected abstract IEnumerable<global::EnvDTE.ProjectItem> GetProjectItems();
		
		internal virtual int Count {
			get { return GetProjectItems().Count(); }
		}
	}
}
