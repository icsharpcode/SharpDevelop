// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	public abstract class ChooseClassServiceBase
	{
		public Type ChooseClass()
		{
			var core = new ChooseClass(GetAssemblies());
			var window = new ChooseClassDialog(core);
			
			if (window.ShowDialog().Value) {
				return core.CurrentClass;
			}
			return null;
		}

		public abstract IEnumerable<Assembly> GetAssemblies();
	}
}
