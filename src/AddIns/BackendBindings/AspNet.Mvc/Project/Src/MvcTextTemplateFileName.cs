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
using System.IO;

namespace ICSharpCode.AspNet.Mvc
{
	public abstract class MvcTextTemplateFileName
	{
		string textTemplatesRootFolder;
		MvcTextTemplateCriteria templateCriteria;
		
		public MvcTextTemplateFileName(
			string textTemplatesRootFolder,
			MvcTextTemplateCriteria templateCriteria)
		{
			this.textTemplatesRootFolder = textTemplatesRootFolder;
			this.templateCriteria = templateCriteria;
		}
		
		public string GetPath()
		{
			string fileName = GetTemplateFileName();
			string directory = GetTemplateFolder();
			return Path.Combine(directory, fileName);
		}
		
		string GetTemplateFileName()
		{
			return templateCriteria.TemplateName + ".tt";
		}
		
		string GetTemplateFolder()
		{
			string codeTemplatesDirectory = GetCodeTemplatesFolder();
			string subFolder = GetCodeTemplatesSubFolder();
			return Path.Combine(codeTemplatesDirectory, subFolder);
		}
		
		string GetCodeTemplatesFolder()
		{
			MvcTextTemplateLanguage language = templateCriteria.TemplateLanguage;
			string subFolder = String.Format("{0}\\CodeTemplates", language.ToString());
			return Path.Combine(textTemplatesRootFolder, subFolder);
		}
		
		protected abstract string GetCodeTemplatesSubFolder();
	}
}
