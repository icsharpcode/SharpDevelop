// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace TextTemplating.Tests.Helpers
{
	public class TestableFileProjectItem : FileProjectItem
	{
		string fileName;
		
		public TestableFileProjectItem(string fileName)
			: base(null, ItemType.None)
		{
			this.fileName = fileName;
		}
		
		public override string FileName {
			get { return fileName; }
			set { fileName = value; }
		}
	}
}
