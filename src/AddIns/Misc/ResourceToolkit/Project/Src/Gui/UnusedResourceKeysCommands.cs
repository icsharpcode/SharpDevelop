// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="c-hornung@gmx.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;

using Hornung.ResourceToolkit.Resolver;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Hides or shows the ICSharpCode.Core host resources in an UnusedResourceKeysViewContent.
	/// </summary>
	public class UnusedResourceKeysHideICSharpCodeCoreHostResourcesCommand : AbstractCheckableMenuCommand, IFilter<KeyValuePair<string, string>>
	{
		string icSharpCodeCoreHostResourceFileName;
		
		public override void Run()
		{
			base.Run();
			
			IFilterHost<KeyValuePair<string, string>> host = ((ToolBarCheckBox)this.Owner).Caller as IFilterHost<KeyValuePair<string, string>>;
			if (host != null) {
				if (this.IsChecked) {
					this.icSharpCodeCoreHostResourceFileName = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceFileName(null);
					host.RegisterFilter(this);
				} else {
					host.UnregisterFilter(this);
				}
			}
		}
		
		/// <summary>
		/// Determines if the specified item matches the current filter criteria.
		/// </summary>
		/// <param name="item">The item to test.</param>
		/// <returns><c>true</c>, if the specified item matches the current filter criteria, otherwise <c>false</c>.</returns>
		public bool IsMatch(KeyValuePair<string, string> item)
		{
			return !FileUtility.IsEqualFileName(item.Value, this.icSharpCodeCoreHostResourceFileName);
		}
	}
}
