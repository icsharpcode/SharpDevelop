// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHost : TextTemplatingHost, IMvcTextTemplateHost
	{
		string viewName = String.Empty;
		string viewDataTypeName = String.Empty;
		string @namespace = String.Empty;
		string masterPageFile = String.Empty;
		string primaryContentPlaceHolderID = String.Empty;
		MvcControllerName controllerName = new MvcControllerName();
		
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
			get { return viewName; }
			set { viewName = UseEmptyStringIfNull(value); }
		}
		
		string UseEmptyStringIfNull(string value)
		{
			if (value != null) {
				return value;
			}
			return String.Empty;
		}
		
		public string ViewDataTypeName {
			get { return viewDataTypeName; }
			set { viewDataTypeName = UseEmptyStringIfNull(value); }
		}
		
		public string ControllerName {
			get { return controllerName.Name; }
			set { controllerName.Name = value; }
		}
		
		public string ControllerRootName {
			get { return controllerName.RootName; }
			set { controllerName.RootName = value; }
		}
		
		public string Namespace {
			get { return @namespace; }
			set { @namespace = UseEmptyStringIfNull(value); }
		}
		
		public bool AddActionMethods { get; set; }
		public bool IsPartialView { get; set; }
		public bool IsContentPage { get; set; }
		
		public string MasterPageFile {
			get { return masterPageFile; }
			set { masterPageFile = UseEmptyStringIfNull(value); }
		}
		
		public string PrimaryContentPlaceHolderID {
			get { return primaryContentPlaceHolderID; }
			set { primaryContentPlaceHolderID = UseEmptyStringIfNull(value); }
		}
	}
}
