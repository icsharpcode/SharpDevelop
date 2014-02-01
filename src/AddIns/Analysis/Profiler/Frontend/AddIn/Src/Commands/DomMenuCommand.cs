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
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of DomMenuCommand.
	/// </summary>
	public abstract class DomMenuCommand : ProfilerMenuCommand
	{
		IAmbience ambience = new CSharpAmbience();
		
		public abstract override void Run();
		
		protected IMember GetMemberFromName(ITypeDefinition c, string name, ReadOnlyCollection<string> parameters)
		{
			if (name == null || c == null)
				return null;
			
			if (name == ".ctor" || name == ".cctor") // Constructor
				name = name.Replace('.', '#');
			
			if (name.StartsWith("get_") || name.StartsWith("set_")) {
				// Property Getter or Setter
				name = name.Substring(4);
				IProperty prop = c.Properties.FirstOrDefault(p => p.Name == name);
				if (prop != null)
					return prop;
			} else if (name.StartsWith("add_") || name.StartsWith("remove_")) {
				name = name.Substring(4);
				IEvent ev = c.Events.FirstOrDefault(e => e.Name == name);
				if (ev != null)
					return ev;
			}
			
			ambience.ConversionFlags = ConversionFlags.UseFullyQualifiedTypeNames | ConversionFlags.ShowParameterNames | ConversionFlags.StandardConversionFlags;
			IMethod matchWithSameName = null;
			IMethod matchWithSameParameterCount = null;
			foreach (IMethod method in c.Methods) {
				if (method.Name != name)
					continue;
				matchWithSameName = method;
				if (method.Parameters.Count != ((parameters == null) ? 0 : parameters.Count))
					continue;
				matchWithSameParameterCount = method;
				bool isCorrect = true;
				for (int i = 0; i < method.Parameters.Count; i++) {
					if (parameters[i] != ambience.ConvertSymbol(method.Parameters[i])) {
						isCorrect = false;
						break;
					}
				}
				
				if (isCorrect)
					return method;
			}
			
			return matchWithSameParameterCount ?? matchWithSameName;
		}

		protected ITypeDefinition GetClassFromName(string name)
		{
			if (name == null)
				return null;
			if (ProjectService.OpenSolution == null)
				return null;
			
			foreach (IProject project in SD.ProjectService.CurrentSolution.Projects) {
				ICompilation compilation = SD.ParserService.GetCompilation(project);
				IType type = compilation.FindType(new FullTypeName(name));
				ITypeDefinition definition = type.GetDefinition();
				if (definition != null)
					return definition;
			}

			return null;
		}
	}
}
