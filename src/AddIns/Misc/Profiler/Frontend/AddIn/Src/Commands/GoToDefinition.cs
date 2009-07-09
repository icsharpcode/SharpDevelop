// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.Profiler.Controls;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Profiler.AddIn.Commands
{
	/// <summary>
	/// Description of GoToDefinition
	/// </summary>
	public class GoToDefinition : ProfilerMenuCommand
	{
		IAmbience ambience = new CSharpAmbience();
		
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			var selectedItem = GetSelectedItems().FirstOrDefault();

			if (selectedItem != null) {
				IClass c = GetClassFromName(selectedItem.FullyQualifiedClassName);
				if (c != null) {
					IEntity member = GetMemberFromName(c, selectedItem.MethodName, selectedItem.Parameters);
					FilePosition position = c.ProjectContent.GetPosition(member ?? c);
					if (!position.IsEmpty && !string.IsNullOrEmpty(position.FileName)) {
						FileService.JumpToFilePosition(position.FileName, position.Line - 1, position.Column - 1);
					}
				}
			}
		}
		
		IMember GetMemberFromName(IClass c, string name, ReadOnlyCollection<string> parameters)
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

		IClass GetClassFromName(string name)
		{
			if (name == null)
				return null;
			if (ProjectService.OpenSolution == null)
				return null;
			
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				IProjectContent content = ParserService.GetProjectContent(project);
				if (content != null) {
					IClass c = content.GetClassByReflectionName(name, false);
					if (c != null)
						return c;
				}
			}

			return null;
		}
	}
}
