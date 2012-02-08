// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcControllerFileName : MvcFileName
	{
		string controllerName = String.Empty;
		
		public string ControllerName {
			get {
				EnsureControllerNameIsNotNull();
				return controllerName;
			}
			set { controllerName = value; }
		}
		
		void EnsureControllerNameIsNotNull()
		{
			controllerName = ConvertToEmptyStringIfNull(controllerName);
		}
		
		public MvcTextTemplateLanguage Language { get; set; }
		
		public override string GetFileName()
		{
			string name = GetControllerName();
			return name + GetFileExtension();
		}
		
		string GetControllerName()
		{
			if (controllerName != null) {
				return controllerName;
			}
			return "Default1Controller";
		}
		
		string GetFileExtension()
		{
			return MvcTextTemplateFileNameExtension.GetControllerFileExtension(Language);
		}
		
		public bool HasValidControllerName()
		{
			return !String.IsNullOrEmpty(controllerName);
		}
	}
}
