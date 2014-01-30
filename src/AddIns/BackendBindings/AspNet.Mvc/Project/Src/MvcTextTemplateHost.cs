// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Reflection;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateHost : TextTemplatingHost, IMvcTextTemplateHost
	{
		string viewName = String.Empty;
		string viewDataTypeName = String.Empty;
		string viewDataTypeAssemblyLocation = String.Empty;
		Type viewDataType;
		string @namespace = String.Empty;
		string masterPageFile = String.Empty;
		string primaryContentPlaceHolderID = String.Empty;
		MvcControllerName controllerName = new MvcControllerName();
		
		public MvcTextTemplateHost(
			ITextTemplatingAppDomainFactory appDomainFactory,
			ITextTemplatingAssemblyResolver assemblyResolver,
			string applicationBase)
			: this(new TextTemplatingHostContext(appDomainFactory, assemblyResolver, null, null), applicationBase)
		{
		}
		
		public MvcTextTemplateHost(TextTemplatingHostContext context, string applicationBase)
			: base(context, applicationBase)
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
		
		public string ViewDataTypeAssemblyLocation {
			get { return viewDataTypeAssemblyLocation; }
			set { viewDataTypeAssemblyLocation = UseEmptyStringIfNull(value); }
		}
		
		public PropertyInfo[] GetViewDataTypeProperties()
		{
			return ViewDataType.GetProperties();
		}
		
		public Type ViewDataType {
			get {
				if (viewDataType == null) {
					viewDataType = GetViewDataType();
				}
				return viewDataType;
			}
			set { viewDataType = value; }
		}
		
		Type GetViewDataType()
		{
			Assembly assembly = LoadAssemblyFrom(ViewDataTypeAssemblyLocation);
			return assembly.GetType(ViewDataTypeName);
		}
		
		protected virtual Assembly LoadAssemblyFrom(string fileName)
		{
			return Assembly.LoadFrom(fileName);
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
