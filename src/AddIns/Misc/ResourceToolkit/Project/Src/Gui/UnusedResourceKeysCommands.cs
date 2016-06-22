// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace Hornung.ResourceToolkit.Gui
{
	/// <summary>
	/// Hides or shows the ICSharpCode.Core host resources in an UnusedResourceKeysViewContent.
	/// </summary>
	public class UnusedResourceKeysHideICSharpCodeCoreHostResourcesCommand : AbstractCheckableMenuCommand, IFilter<ResourceItem>
	{
		string icSharpCodeCoreHostResourceFileName;
		
		public override void Run()
		{
			base.Run();
			
			IFilterHost<ResourceItem> host = ((ToolBarCheckBox)this.Owner).Caller as IFilterHost<ResourceItem>;
			if (host != null) {
				if (this.IsChecked) {
					this.icSharpCodeCoreHostResourceFileName = ICSharpCodeCoreResourceResolver.GetICSharpCodeCoreHostResourceSet(null).FileName;
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
		public bool IsMatch(ResourceItem item)
		{
			return !FileUtility.IsEqualFileName(item.FileName, this.icSharpCodeCoreHostResourceFileName);
		}
	}
}
