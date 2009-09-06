// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
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
		
		protected IMember GetMemberFromName(IClass c, string name, ReadOnlyCollection<string> parameters)
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
					if (parameters[i] != ambience.Convert(method.Parameters[i])) {
						isCorrect = false;
						break;
					}
				}
				
				if (isCorrect)
					return method;
			}
			
			return matchWithSameParameterCount ?? matchWithSameName;
		}

		protected IClass GetClassFromName(string name)
		{
			if (name == null)
				return null;
			if (ProjectService.OpenSolution == null)
				return null;
			
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				IProjectContent content = ParserService.GetProjectContent(project);
				if (content != null) {
					IClass c = content.GetClassByReflectionName(name, true);
					if (c != null)
						return c;
				}
			}

			return null;
		}
	}
}
