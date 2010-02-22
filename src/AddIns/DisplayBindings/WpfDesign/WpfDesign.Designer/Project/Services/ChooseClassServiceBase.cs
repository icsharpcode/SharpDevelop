// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Ivan Shumilin"/>
//     <version>$Revision$</version>
// </file>

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
