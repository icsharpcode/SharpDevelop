// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public abstract class TextTemplatingCustomTool : ICustomTool
	{
		public abstract void GenerateCode(FileProjectItem item, CustomToolContext context);
		
		protected TextTemplatingHost CreateTextTemplatingHost(IProject project)
		{
			var context = new TextTemplatingHostContext(project);
			string applicationBase = GetAssemblyBaseLocation();
			return new TextTemplatingHost(context, applicationBase);
		}
		
		string GetAssemblyBaseLocation()
		{
			string location = GetType().Assembly.Location;
			return Path.GetDirectoryName(location);
		}
	}
}
