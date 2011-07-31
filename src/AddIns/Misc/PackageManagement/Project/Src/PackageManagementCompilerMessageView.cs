// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementCompilerMessageView : ICompilerMessageView
	{
		public IMessageViewCategory Create(string categoryName, string categoryDisplayName)
		{
			MessageViewCategory view = null;
			MessageViewCategory.Create(ref view, categoryName, categoryDisplayName);
			return new PackageManagementMessageViewCategory(view);
		}
		
		public IMessageViewCategory GetExisting(string categoryName)
		{
			MessageViewCategory view = CompilerMessageView.Instance.GetCategory(categoryName);
			if (view != null) {
				return new PackageManagementMessageViewCategory(view);
			}
			return null;
		}
	}
}
