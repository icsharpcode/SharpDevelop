// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
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
	public class TemplateCompletionDataProvider : AbstractCompletionDataProvider
	{
		ImageList imageList = new ImageList();
		
		public override ImageList ImageList {
			get {
				return imageList;
			}
		}
		
		public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
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
			
			public string Text {
				get {
					return template.Shortcut + "\t" + template.Description;
				}
				set {
					throw new NotSupportedException();
				}
			}
			
			public string Description {
				get {
					return template.Text;
				}
			}
			
			public double Priority {
				get {
					return 0;
				}
			}
			
			public bool InsertAction(TextArea textArea, char ch)
			{
				((SharpDevelopTextAreaControl)textArea.MotherTextEditorControl).InsertTemplate(template);
				return false;
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
