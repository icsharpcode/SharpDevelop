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
using System.Collections.Generic;
using System.IO;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AspNet.Mvc
{
	public class MvcTextTemplateRepository : IMvcTextTemplateRepository
	{
		string textTemplatesRootDirectory;
		IFileSystem fileSystem;
		
		public MvcTextTemplateRepository(string mvcAddInPath, IFileSystem fileSystem)
		{
			this.fileSystem = fileSystem;
			GetTextTemplatesRootDirectory(mvcAddInPath);
		}
		
		public MvcTextTemplateRepository(string mvcAddInPath)
			: this(mvcAddInPath, SD.FileSystem)
		{
		}
		
		public MvcTextTemplateRepository()
			: this(StringParser.GetValue("addinpath:ICSharpCode.AspNet.Mvc"))
		{
		}
		
		void GetTextTemplatesRootDirectory(string mvcAddInPath)
		{
			this.textTemplatesRootDirectory = Path.Combine(mvcAddInPath, "ItemTemplates");
		}
		
		public string GetMvcViewTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var fileName = new MvcViewTextTemplateFileName(textTemplatesRootDirectory, templateCriteria);
			return fileName.GetPath();
		}
		
		public string GetMvcControllerTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var fileName = new MvcControllerTextTemplateFileName(textTemplatesRootDirectory, templateCriteria);
			return fileName.GetPath();
		}
		
		public IEnumerable<MvcControllerTextTemplate> GetMvcControllerTextTemplates(MvcTextTemplateCriteria templateCriteria)
		{
			string templateFileName = GetDefaultMvcControllerTextTemplateFileName(templateCriteria);
			yield return CreateEmptyControllerTemplate(templateFileName);
			yield return CreateEmptyReadWriteControllerTemplate(templateFileName);
		}
		
		string GetDefaultMvcControllerTextTemplateFileName(MvcTextTemplateCriteria templateCriteria)
		{
			var defaultControllerTemplateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = templateCriteria.TemplateLanguage,
				TemplateName = "Controller",
				TemplateType = templateCriteria.TemplateType
			};
			return GetMvcControllerTextTemplateFileName(defaultControllerTemplateCriteria);
		}
		
		MvcControllerTextTemplate CreateEmptyControllerTemplate(string templateFileName)
		{
			return new MvcControllerTextTemplate() {
				Name = "Empty",
				Description = "Empty controller",
				FileName = templateFileName,
				AddActionMethods = false
			};
		}
		
		MvcControllerTextTemplate CreateEmptyReadWriteControllerTemplate(string templateFileName)
		{
			return new MvcControllerTextTemplate() {
				Name = "EmptyReadWrite",
				Description = "Controller with create, read, update and delete actions",
				FileName = templateFileName,
				AddActionMethods = true
			};
		}
		
		public IEnumerable<MvcViewTextTemplate> GetMvcViewTextTemplates(MvcTextTemplateCriteria templateCriteria)
		{
			foreach (string templateFileName in GetMvcViewTemplateFileNamesInFolder(templateCriteria)) {
				yield return new MvcViewTextTemplate(templateFileName);
			}
		}
		
		IEnumerable<FileName> GetMvcViewTemplateFileNamesInFolder(MvcTextTemplateCriteria templateCriteria)
		{
			string templatePath = GetMvcViewTemplatePath(templateCriteria);
			return fileSystem.GetFiles(DirectoryName.Create(templatePath), "*.tt");
		}
		
		string GetMvcViewTemplatePath(MvcTextTemplateCriteria templateCriteria)
		{
			var emptyViewTemplateCriteria = new MvcTextTemplateCriteria() {
				TemplateLanguage = templateCriteria.TemplateLanguage,
				TemplateName = "Empty",
				TemplateType = templateCriteria.TemplateType
			};
			string emptyViewTemplateFileName = GetMvcViewTextTemplateFileName(emptyViewTemplateCriteria);
			return Path.GetDirectoryName(emptyViewTemplateFileName);
		}
	}
}
