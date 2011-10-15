// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Reflection;

using ICSharpCode.Core;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingAddIn : IAddIn
	{
		AddIn addIn;
		
		public TextTemplatingAddIn(AddIn addIn)
		{
			this.addIn = addIn;
		}
		
		public string PrimaryIdentity {
			get { return this.addIn.Manifest.PrimaryIdentity; }
		}
		
		public IEnumerable<IAddInRuntime> GetRuntimes()
		{
			foreach (Runtime runtime in this.addIn.Runtimes) {
				yield return new TextTemplatingAddInRuntime(runtime);
			}
		}
	}
}
