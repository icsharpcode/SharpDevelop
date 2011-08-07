// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHost : TextTemplatingHost, IMvcTextTemplateHost
	{
		string viewName = String.Empty;
		string controllerName = String.Empty;
		string @namespace = String.Empty;
		
		public MvcTextTemplateHost(
			ITextTemplatingAppDomainFactory appDomainFactory,
			ITextTemplatingAssemblyResolver assemblyResolver,
			string applicationBase)
			: base(appDomainFactory, assemblyResolver, applicationBase)
		{
			AddAssemblyReferenceForMvcHost();
			AddImports();
		}
		
		void AddAssemblyReferenceForMvcHost()
		{
			Refs.Add(GetType().Assembly.Location);
			Refs.Add(typeof(TextTemplatingHost).Assembly.Location);
		}
		
		void AddImports()
		{
			Imports.Add("ICSharpCode.AspNet.Mvc");
		}
		
		public string ViewName {
			get { return GetValueOrUseEmptyStringIfNull(viewName); }
			set { viewName = value; }
		}
		
		string GetValueOrUseEmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		public string ControllerName {
			get { return GetValueOrUseEmptyStringIfNull(controllerName); }
			set { controllerName = value; }
		}
		
		public string Namespace {
			get { return GetValueOrUseEmptyStringIfNull(@namespace); }
			set { @namespace = value; }
		}
	}
}
