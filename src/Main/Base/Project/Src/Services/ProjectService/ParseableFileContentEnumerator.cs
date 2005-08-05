// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ParseableFileContentEnumerator.
	/// </summary>
	public class ParseableFileContentEnumerator : IEnumerator<KeyValuePair<string, string>>
	{
		void IEnumerator.Reset() {
			throw new NotSupportedException();
		}
		
		KeyValuePair<string, string> current;
		
		object IEnumerator.Current {
			get {
				return current;
			}
		}
		
		public KeyValuePair<string, string> Current {
			get {
				return current;
			}
		}
		
		public string CurrentFileName {
			get {
				return current.Key;
			}
		}
		
		public string CurrentFileContent {
			get {
				return current.Value;
			}
		}
		
		public void Dispose()
		{
		}
		
		ProjectItem[] projectItems;
		
		public ParseableFileContentEnumerator(IProject project) : this(project.Items.ToArray()) { }
		
		public ParseableFileContentEnumerator(ProjectItem[] projectItems)
		{
			this.projectItems = projectItems;
			Properties textEditorProperties = ((Properties)PropertyService.Get("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new Properties()));
			getParseableContentEncoding = Encoding.GetEncoding(textEditorProperties.Get("Encoding", 1252));
			pcd = new GetParseableContentDelegate(GetParseableFileContent);
			if (projectItems.Length > 0) {
				nextItem = projectItems[0];
			}
		}
		
		delegate string GetParseableContentDelegate(IProject project, string fileName);
		
		GetParseableContentDelegate pcd;
		
		Encoding getParseableContentEncoding;
		
		string GetParseableFileContent(IProject project, string fileName)
		{
			//Console.WriteLine("Reading {0} from disk", fileName);
			
			// Loading the source files is done asynchronously:
			// While one file is parsed, the next is already loaded from disk.
			string res = project.GetParseableFileContent(fileName);
			if (res != null)
				return res;
			
			// load file
			using (StreamReader r = new StreamReader(fileName, getParseableContentEncoding)) {
				return r.ReadToEnd();
			}
		}
		
		ProjectItem nextItem;
		IAsyncResult res;
		int index = 0;
		
		public int ItemCount {
			get {
				return projectItems.Length;
			}
		}
		
		public int Index {
			get {
				return index;
			}
		}
		
		public bool MoveNext()
		{
			ProjectItem item = nextItem;
			nextItem = (++index < projectItems.Length) ? projectItems[index] : null;
			if (item == null) return false;
			if (item.ItemType != ItemType.Compile)
				return MoveNext();
			string fileContent;
			try {
				if (res != null) {
					fileContent = pcd.EndInvoke(res);
				} else {
					fileContent = GetFileContent(item);
				}
			} catch (FileNotFoundException ex) {
				res = null;
				Console.WriteLine("ParseableFileContentEnumerator: " + ex.Message);
				return MoveNext(); // skip files that were not found
			} catch (IOException ex) {
				res = null;
				Console.WriteLine("ParseableFileContentEnumerator: " + ex.Message);
				return MoveNext(); // skip invalid files
			}
			if (nextItem != null && nextItem.ItemType == ItemType.Compile && CanReadAsync(nextItem))
				res = pcd.BeginInvoke(nextItem.Project, nextItem.FileName, null, null);
			else
				res = null;
			current = new KeyValuePair<string, string>(item.FileName, fileContent);
			return true;
		}
		
		string GetFileContent(ProjectItem item)
		{
			string fileName = item.FileName;
			IWorkbenchWindow window = FileService.GetOpenFile(fileName);
			if (window != null) {
				IViewContent viewContent = window.ViewContent;
				IEditable editable = viewContent as IEditable;
				if (editable != null) {
					//Console.WriteLine("Reading {0} from editable", fileName);
					return editable.Text;
				}
			}
			return GetParseableFileContent(item.Project, fileName);
		}
		
		bool CanReadAsync(ProjectItem item)
		{
			return !FileService.IsOpen(item.FileName);
		}
	}
}
