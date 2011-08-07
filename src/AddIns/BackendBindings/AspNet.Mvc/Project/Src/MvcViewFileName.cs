// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcViewFileName
	{
		string viewName = String.Empty;
		string viewFolder = String.Empty;
		
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
		
		static string ConvertToEmptyStringIfNull(string text)
		{
			if (text != null) {
				return text;
			}
			return String.Empty;
		}
		
		public string ViewFolder { 
			get {
				EnsureViewFolderIsNotNull();
				return viewFolder;
			}
			set { viewFolder = value; }
		}
		
		void EnsureViewFolderIsNotNull()
		{
			viewFolder = ConvertToEmptyStringIfNull(viewFolder);
		}
		
		public override string ToString()
		{
			return GetPath();
		}
		
		public string GetPath()
		{
			string fileName = GetFileName();
			return Path.Combine(ViewFolder, fileName);
		}
		
		public string GetFileName()
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
