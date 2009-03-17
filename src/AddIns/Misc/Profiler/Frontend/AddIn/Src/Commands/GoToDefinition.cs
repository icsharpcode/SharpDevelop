// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Linq;
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
	public class GoToDefinition : AbstractMenuCommand
	{
		IAmbience ambience = new CSharpAmbience();
		
		/// <summary>
		/// Starts the command
		/// </summary>
		public override void Run()
		{
			IList<CallTreeNodeViewModel> list = ((QueryView)Owner).SelectedItems.ToList();
			
			if (list.Count == 0)
				return;
			
			CallTreeNodeViewModel selectedItem = list.First();

			if (selectedItem != null) {
				IClass c = GetClassFromName(selectedItem.FullyQualifiedClassName);
				if (c != null)
				{
					IMember member = GetMemberFromName(c, selectedItem.MethodName, selectedItem.Parameters);
					if (member != null) {
						IViewContent view = FileService.JumpToFilePosition(c.CompilationUnit.FileName, member.BodyRegion.BeginLine, member.BodyRegion.BeginColumn);
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
			
			if (name.StartsWith("get_") || name.StartsWith("set_")) // Property Getter or Setter
				name = name.Substring(4);
			
			foreach (IMethodOrProperty member in c.Methods)
			{
				if (member.Name != name)
					continue;
				if (member.Parameters.Count != ((parameters == null) ? 0 : parameters.Count))
					continue;
				bool isCorrect = true;
				for (int i = 0; i < member.Parameters.Count; i++) {
					if (parameters[i] != ambience.Convert(member.Parameters[i])) {
						isCorrect = false;
						break;
					}
				}
				
				if (isCorrect)
					return member;
			}
			
			foreach (IMethodOrProperty member in c.Properties)
			{
				if (member.Name != name)
					continue;
				if (member.Parameters.Count != ((parameters == null) ? 0 : parameters.Count))
					continue;
				
				bool isCorrect = true;
				for (int i = 0; i < member.Parameters.Count; i++) {
					if (parameters[i] != ambience.Convert(member.Parameters[i])) {
						isCorrect = false;
						break;
					}
				}
				
				if (isCorrect)
					return member;
			}
			
			return null;
		}

		IClass GetClassFromName(string name)
		{
			if (name == null)
				return null;
			
			foreach (IProjectContent content in ParserService.AllProjectContents)
			{
				foreach (IClass c in content.Classes)
				{
					if (name == c.FullyQualifiedName)
						return c;
				}
			}

			return null;
		}
	}
}
