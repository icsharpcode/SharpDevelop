// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class SvcUtilMessageView
	{
		static MessageViewCategory category;
		
		public static MessageViewCategory Category {
			get {
				if (category == null) {
					MessageViewCategory.Create(ref category, "SvcUtil");
				}
				return category;
			}
		}

		public static void AppendLine(string text)
		{
			Category.AppendLine(text);
		}
		
		public static void ClearText()
		{
			Category.ClearText();
		}
	}
}
