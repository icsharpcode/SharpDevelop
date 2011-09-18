// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewTextTemplate
	{
		public MvcViewTextTemplate()
			: this(String.Empty)
		{
		}
		
		public MvcViewTextTemplate(string fileName)
		{
			this.Name = Path.GetFileNameWithoutExtension(fileName);
			this.FileName = fileName;
		}
		
		public string Name { get; set; }
		public string FileName { get; set; }
		
		public bool IsEmptyTemplate()
		{
			return "Empty".Equals(Name, StringComparison.InvariantCultureIgnoreCase);
		}
	}
}
