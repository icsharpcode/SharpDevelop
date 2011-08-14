// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewFileName : MvcFileName
	{
		string viewName = String.Empty;
		
		public string ViewName {
			get {
				EnsureViewNameIsNotNull();
				return viewName;
			}
			set { viewName = value; }
		}
		
		void EnsureViewNameIsNotNull()
		{
			viewName = ConvertToEmptyStringIfNull(viewName);
		}
		
		public MvcTextTemplateType TemplateType { get; set; }
		public MvcTextTemplateLanguage TemplateLanguage { get; set; }
		public bool IsPartialView { get; set; }
		
		public override string GetFileName()
		{
			string viewName = GetViewName();
			string viewFileExtension = GetViewFileExtension();
			return viewName + viewFileExtension;
		}
		
		string GetViewName()
		{
			if (viewName != null) {
				return viewName;
			}
			return "View1";
		}
		
		string GetViewFileExtension()
		{
			return
				MvcTextTemplateFileNameExtension
					.GetViewFileExtension(TemplateType, TemplateLanguage, IsPartialView);
		}
		
		public bool HasValidViewName()
		{
			return !String.IsNullOrEmpty(viewName);
		}
	}
}
