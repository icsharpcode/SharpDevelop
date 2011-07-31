// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement;

namespace PackageManagement.Tests.Helpers
{
	public class FakeMessageCategoryView : IMessageViewCategory
	{
		public bool IsClearCalled;

		public List<string> LinesAppended = new List<string>();
		
		public string FirstLineAppended {
			get { return LinesAppended[0]; }
		}
		
		public void AppendLine(string text)
		{
			LinesAppended.Add(text);
		}
		
		public void Clear()
		{
			IsClearCalled = true;
		}
	}
}
