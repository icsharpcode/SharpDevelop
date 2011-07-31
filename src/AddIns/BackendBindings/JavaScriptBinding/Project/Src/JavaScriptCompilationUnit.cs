// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.JavaScriptBinding
{
	public class JavaScriptCompilationUnit : DefaultCompilationUnit
	{
		public JavaScriptCompilationUnit(
			IProjectContent projectContent,
			string fileName)
			: base(projectContent)
		{
			this.FileName = fileName;
			AddGlobalClass();
		}
		
		void AddGlobalClass()
		{
			GlobalClass = CreateGlobalClass();
			AddClass(GlobalClass);
		}
		
		public JavaScriptGlobalClass GlobalClass { get; private set; }
		
		JavaScriptGlobalClass CreateGlobalClass()
		{
			string className = GetGlobalClassName();
			return new JavaScriptGlobalClass(this, className);
		}
		
		string GetGlobalClassName()
		{
			return Path.GetFileNameWithoutExtension(FileName);
		}
		
		void AddClass(IClass c)
		{
			Classes.Add(c);
		}
	}
}
