// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.PackageManagement
{
	public class PackageManagementMessageViewCategory : IMessageViewCategory
	{
		MessageViewCategory messageViewCategory;
		
		public PackageManagementMessageViewCategory(MessageViewCategory messageViewCategory)
		{
			this.messageViewCategory = messageViewCategory;
		}
		
		public void AppendLine(string text)
		{
			messageViewCategory.AppendLine(text);
		}
		
		public void Clear()
		{
			messageViewCategory.ClearText();
		}
	}
}
