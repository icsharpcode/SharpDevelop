// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.Build.Tasks;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.MonoAddIn
{
	public class MonoGacReferencePanel : GacReferencePanel
	{		
		public MonoGacReferencePanel(ISelectReferenceDialog selectDialog) : base(selectDialog)
		{
		}
		
		protected override IList<DomAssemblyName> GetCacheContent()
		{
			List<DomAssemblyName> list = new List<DomAssemblyName>();
			foreach (MonoAssemblyName assemblyName in MonoGlobalAssemblyCache.GetAssemblyNames()) {
				list.Add(new DomAssemblyName(assemblyName.FullName));
			}
			return itemList;
		}
	}
}
