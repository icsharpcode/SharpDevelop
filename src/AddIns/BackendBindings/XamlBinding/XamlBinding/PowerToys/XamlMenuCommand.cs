// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XamlBinding.PowerToys
{
	/// <summary>
	/// Description of XamlMenuCommand
	/// </summary>
	public abstract class XamlMenuCommand : AbstractMenuCommand
	{
		/// <summary>
		/// Starts the command
		/// </summary>
		public sealed override void Run()
		{
			try {
				ITextEditorProvider provider = WorkbenchSingleton.Workbench.ActiveViewContent as ITextEditorProvider;
				
				if (provider != null) {
					TextReader reader = provider.TextEditor.Document.CreateReader();
					XmlDocument document = new XmlDocument();
					document.Load(reader);
					Refactor(provider.TextEditor, document);
					StringWriter sWriter = new StringWriter();
				    XmlTextWriter writer = new XmlTextWriter(sWriter);
				    writer.Formatting = Formatting.Indented;
				    document.WriteTo(writer);
				    writer.Flush();
					provider.TextEditor.Document.Text = sWriter.ToString();
				}
			} catch (XmlException e) {
				MessageService.ShowError(e.Message);
			}
		}
		
		protected abstract void Refactor(ITextEditor editor, XmlDocument document);
	}
}
