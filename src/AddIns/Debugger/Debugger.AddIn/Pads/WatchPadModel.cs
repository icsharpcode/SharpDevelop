// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;
using System.Text;
using Debugger.AddIn.TreeModel;
using ICSharpCode.NRefactory;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Project.SavedData;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class TextNode : TreeNode, ISetText, IProjectSavedData
	{
		public TextNode(TreeNode parent, string text, SupportedLanguage language)
			: base(parent)
		{
			this.Name = text;
			this.Language = language;
			
			if (ProjectService.CurrentProject != null)
				ProjectName = ProjectService.CurrentProject.Name;
		}
		
		public override bool CanSetText {
			get {
				return true;
			}
		}
		
		public override bool SetText(string text)
		{
			this.Text = text;
			return true;
		}
		
		public bool SetName(string name)
		{
			this.Name = name;
			return true;
		}
		
		public SupportedLanguage Language { get; set; }
		
		#region IProjectSavedData implementation
		
		string savedString;
		
		public string SavedString {
			get {
				StringBuilder sb = new StringBuilder();
				sb.Append("ProjectName"); 			sb.Append('|'); sb.Append(ProjectName); 	sb.Append('|');
				sb.Append("ProjectSavedDataType"); 	sb.Append('|');	sb.Append(SavedDataType); 	sb.Append('|');
				sb.Append("FullName"); 				sb.Append('|'); sb.Append(FullName); 		sb.Append('|');
				sb.Append("Language"); 				sb.Append('|'); sb.Append(Language.ToString());
				
				savedString = sb.ToString();
				return savedString;
			}
			set { savedString = value; }
		}
		
		public ProjectSavedDataType SavedDataType {
			get {
				return ProjectSavedDataType.WatchVariables;
			}
		}
		
		public string ProjectName {
			get; internal set;
		}
		
		#endregion
	}
	
	public class ErrorInfoNode : ICorDebug.InfoNode
	{
		public ErrorInfoNode(string name, string text) : base(null, name, text)
		{
			IconImage = DebuggerResourceService.GetImage("Icons.16x16.Error");
		}
	}
}
