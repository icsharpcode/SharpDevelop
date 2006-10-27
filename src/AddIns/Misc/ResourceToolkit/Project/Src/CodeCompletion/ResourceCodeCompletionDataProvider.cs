// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email=""/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Hornung.ResourceToolkit.ResourceFileContent;
using ICSharpCode.NRefactory.PrettyPrinter;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;

namespace Hornung.ResourceToolkit.CodeCompletion
{
	/// <summary>
	/// Provides code completion data for resource keys.
	/// </summary>
	public class ResourceCodeCompletionDataProvider : AbstractCompletionDataProvider
	{
		readonly IResourceFileContent content;
		readonly IOutputAstVisitor outputVisitor;
		readonly string preEnteredName;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceCodeCompletionDataProvider" /> class.
		/// </summary>
		/// <param name="content">The resource file content to be presented to the user.</param>
		/// <param name="outputVisitor">The NRefactory output visitor to be used to generate the inserted code. If <c>null</c>, the key is inserted literally.</param>
		/// <param name="preEnteredName">The type name which should be pre-entered in the 'add new' dialog box if the user selects the 'add new' entry.</param>
		public ResourceCodeCompletionDataProvider(IResourceFileContent content, IOutputAstVisitor outputVisitor, string preEnteredName)
		{
			if (content == null) {
				throw new ArgumentNullException("content");
			}
			this.content = content;
			this.outputVisitor = outputVisitor;
			this.preEnteredName = preEnteredName;
			this.InsertSpace = false;
		}
		
		/// <summary>
		/// Generates the completion data. This method is called by the text editor control.
		/// </summary>
		public override ICompletionData[] GenerateCompletionData(string fileName, ICSharpCode.TextEditor.TextArea textArea, char charTyped)
		{
			List<ICompletionData> list = new List<ICompletionData>();
			
			list.Add(new NewResourceCodeCompletionData(this.content, this.outputVisitor, this.preEnteredName));
			
			foreach (KeyValuePair<string, object> entry in this.content.Data) {
				list.Add(new ResourceCodeCompletionData(entry.Key, ResourceResolverService.FormatResourceDescription(this.content, entry.Key), this.outputVisitor));
			}
			
			return list.ToArray();
		}
		
		/// <summary>
		/// Gets if pressing 'key' should trigger the insertion of the currently selected element.
		/// </summary>
		public override CompletionDataProviderKeyResult ProcessKey(char key)
		{
			if (key == '.') {
				// don't auto-complete on pressing '.' (this character is commonly used in resource key names)
				return CompletionDataProviderKeyResult.NormalKey;
			}
			return base.ProcessKey(key);
		}
		
	}
}
