// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom.Compiler;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.TextTemplating
{
	public class TextTemplatingFileGeneratorCustomTool : ICustomTool
	{		
		public void GenerateCode(FileProjectItem item, CustomToolContext context)
		{
			using (var generator = CreateTextTemplatingFileGenerator(item, context)) {
				generator.ProcessTemplate();
			}
		}
		
		protected virtual ITextTemplatingFileGenerator CreateTextTemplatingFileGenerator(
			FileProjectItem projectFile,
			CustomToolContext context)
		{
			var appDomainFactory = new TextTemplatingAppDomainFactory();
			string applicationBase = GetAssemblyBaseLocation();
			var host = new TextTemplatingHost(appDomainFactory, applicationBase);
			var textTemplatingCustomToolContext = new TextTemplatingCustomToolContext(context);
			
			return new TextTemplatingFileGenerator(host, projectFile, textTemplatingCustomToolContext);
		}
		
		string GetAssemblyBaseLocation()
		{
			string location = GetType().Assembly.Location;
			return Path.GetDirectoryName(location);
		}
	}
}