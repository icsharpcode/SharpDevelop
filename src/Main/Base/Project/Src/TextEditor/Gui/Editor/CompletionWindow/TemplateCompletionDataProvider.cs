// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor
{
	public class TemplateCompletionDataProvider : ICompletionDataProvider
	{
		ImageList imageList = new ImageList();
	
		public ImageList ImageList {
			get {
				return imageList;
			}
		}
		
		public string PreSelection {
			get {
				return String.Empty;
			}
		}
		
		public ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
		{
			
			imageList.Images.Add(IconService.GetBitmap("Icons.16x16.TextFileIcon"));
			CodeTemplateGroup templateGroup = CodeTemplateLoader.GetTemplateGroupPerFilename(fileName);
			if (templateGroup == null) {
				return null;
			}
			ArrayList completionData = new ArrayList();
			foreach (CodeTemplate template in templateGroup.Templates) {
				completionData.Add(new TemplateCompletionData(template));
			}
			
			return (ICompletionData[])completionData.ToArray(typeof(ICompletionData));
		}
		
		class TemplateCompletionData : ICompletionData, IComparable
		{
			CodeTemplate template;
			
			public int ImageIndex {
				get {
					return 0;
				}
			}
			
			public string[] Text {
				get {
					return new string[] { template.Shortcut + "\t" + template.Description };
				}
			}
			
			public string Description {
				get {
					return template.Text;
				}
			}
			
			public void InsertAction(TextEditorControl control)
			{
				((SharpDevelopTextAreaControl)control).InsertTemplate(template);
			}
			
			public TemplateCompletionData(CodeTemplate template) 
			{
				this.template = template;
			}
			
			
			
			#region System.IComparable interface implementation
			public int CompareTo(object obj)
			{
				if (obj == null || !(obj is TemplateCompletionData)) {
					return -1;
				}
				return template.Shortcut.CompareTo(((TemplateCompletionData)obj).template.Shortcut);
			}
			#endregion
			
			
		}
	}
}
