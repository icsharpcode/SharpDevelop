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
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using TypeResolutionService = ICSharpCode.SharpDevelop.Designer.TypeResolutionService;

namespace ICSharpCode.WpfDesign.AddIn
{
	public class MyTypeFinder : XamlTypeFinder
	{
		OpenedFile file;
		readonly TypeResolutionService typeResolutionService = new TypeResolutionService();
		
		public static MyTypeFinder Create(OpenedFile file)
		{
			MyTypeFinder f = new MyTypeFinder();
			f.file = file;
			f.ImportFrom(CreateWpfTypeFinder());
			
			// DO NOT USE Assembly.LoadFrom!
			// use the special handling logic defined in TypeResolutionService!
			var compilation = SD.ParserService.GetCompilationForFile(file.FileName);
			foreach (var referencedAssembly in compilation.ReferencedAssemblies) {
				try {
					var assembly = f.typeResolutionService.LoadAssembly(referencedAssembly);
					if (assembly != null)
						f.RegisterAssembly(assembly);
				} catch (Exception ex) {
					ICSharpCode.Core.LoggingService.Warn("Error loading Assembly : " + referencedAssembly.FullAssemblyName, ex);
				}
			}
			return f;
		}
		
		public override Assembly LoadAssembly(string name)
		{
			if (string.IsNullOrEmpty(name)) {
				IProject pc = GetProject(file);
				if (pc != null) {
					return typeResolutionService.LoadAssembly(pc);
				}
				return null;
			} else {
				Assembly assembly = FindAssemblyInProjectReferences(name);
				if (assembly != null) {
					return assembly;
				}
				return base.LoadAssembly(name);
			}
		}
		
		public override Uri ConvertUriToLocalUri(Uri uri)
		{
			if (!uri.IsAbsoluteUri)
			{
				var compilation = SD.ParserService.GetCompilationForFile(file.FileName);
				var assembly = this.typeResolutionService.LoadAssembly(compilation.MainAssembly);
				var prj = SD.ProjectService.CurrentProject;

				if (uri.OriginalString.Contains(";"))
				{
					var parts = uri.OriginalString.Split(';');
					if (prj.Name == parts[0].Substring(1))
					{
						var newUri = new Uri(("file://" + Path.Combine(prj.Directory, parts[1].Substring("component".Length + 1))).Replace("\\", "/"), UriKind.RelativeOrAbsolute);
						return newUri;
					}
				}
				else
				{
					var strg = uri.OriginalString;
					if (strg.StartsWith("/"))
						strg = strg.Substring(1);
					var newUri = new Uri(("file://" + Path.Combine(prj.Directory, strg)).Replace("\\", "/"), UriKind.RelativeOrAbsolute);
					return newUri;
				}
			}
			
			return uri;
		}
		
		Assembly FindAssemblyInProjectReferences(string name)
		{
			IProject pc = GetProject(file);
			if (pc != null) {
				return FindAssemblyInProjectReferences(pc, name);
			}
			return null;
		}
		
		Assembly FindAssemblyInProjectReferences(IProject pc, string name)
		{
			ICompilation compilation = SD.ParserService.GetCompilation(pc);
			IAssembly assembly = compilation.ReferencedAssemblies.FirstOrDefault(asm => asm.AssemblyName == name);
			if (assembly != null) {
				return typeResolutionService.LoadAssembly(assembly);
			}
			return null;
		}
		
		public override XamlTypeFinder Clone()
		{
			MyTypeFinder copy = new MyTypeFinder();
			copy.file = this.file;
			copy.ImportFrom(this);
			return copy;
		}
		
		internal static IProject GetProject(OpenedFile file)
		{
			return SD.ProjectService.FindProjectContainingFile(file.FileName);
		}
	}
}
