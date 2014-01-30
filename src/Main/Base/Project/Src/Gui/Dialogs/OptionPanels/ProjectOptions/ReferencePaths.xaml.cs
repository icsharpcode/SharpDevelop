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
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Interaction logic for ReferencePathsXAML.xaml
	/// </summary>
	public  partial class ReferencePaths : ProjectOptionPanel
	{
		public ReferencePaths()
		{
			InitializeComponent();
			
			editor.BrowseForDirectory = true;
			editor.TitleText = StringParser.Parse("${res:Global.Folder}:");
			editor.ListCaption = StringParser.Parse("${res:Dialog.ProjectOptions.ReferencePaths}:");
			editor.ListChanged += delegate { IsDirty = true; };
		}
		
		
		public ProjectProperty<string> ReferencePath {
			get {
				return GetProperty("ReferencePath", "",TextBoxEditMode.EditEvaluatedProperty);
			}
		}
		
		
		protected override void Load(MSBuildBasedProject project, string configuration, string platform)
		{
			base.Load(project, configuration, platform);
			string prop  = GetProperty("ReferencePath", "", TextBoxEditMode.EditRawProperty).Value;
			
			string[] values = prop.Split(';');
			if (values.Length == 1 && values[0].Length == 0) {
				editor.LoadList(new string[0]);
			} else {
				editor.LoadList(values);
			}
		}
	
		
		protected override bool Save(MSBuildBasedProject project, string configuration, string platform)
		{
			ReferencePath.Value = String.Join(";", editor.GetList());
			return base.Save(project, configuration, platform);
		}
		
	}
}
