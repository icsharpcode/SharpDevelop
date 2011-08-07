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
		
		public override string GetFileName()
		{
			string viewName = GetViewName();
			return viewName + ".aspx";
		}
		
		string GetViewName()
		{
			if (viewName != null) {
				return viewName;
			}
			return "View1";
		}
		
		public bool HasValidViewName()
		{
			return !String.IsNullOrEmpty(viewName);
		}
	}
}
