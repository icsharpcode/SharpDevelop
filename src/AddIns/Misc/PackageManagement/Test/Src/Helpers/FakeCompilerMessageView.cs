// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakeCompilerMessageView : ICompilerMessageView
	{
		public FakeMessageCategoryView FakeMessageCategoryView = new FakeMessageCategoryView();		
		public List<string> MessageViewCategoriesCreated = new List<string>();
		public FakeMessageCategoryView GetExistingReturnValue;
		public string CategoryNamePassedToCategoryExists;
		
		public string FirstMessageViewCategoryCreated {
			get { return MessageViewCategoriesCreated[0]; }
		}

		public IMessageViewCategory Create(string categoryName, string categoryDisplayName)
		{
			MessageViewCategoriesCreated.Add(categoryName);
			return FakeMessageCategoryView;
		}
		
		public IMessageViewCategory GetExisting(string categoryName)
		{
			CategoryNamePassedToCategoryExists = categoryName;
			return GetExistingReturnValue;
		}
	}
}
