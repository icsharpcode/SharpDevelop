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
			var appDomainFactory = new TextTemplatingAppDomainFactory();
			string applicationBase = GetAssemblyBaseLocation();
			var assemblyResolver = new TextTemplatingAssemblyResolver(project);
			var textTemplatingVariables = new TextTemplatingVariables();
			var serviceProvider = new TextTemplatingServiceProvider();
			var context = new TextTemplatingHostContext(project);
			return new TextTemplatingHost(context, applicationBase);
		}
		
		string GetAssemblyBaseLocation()
		{
			string location = GetType().Assembly.Location;
			return Path.GetDirectoryName(location);
		}
	}
}
