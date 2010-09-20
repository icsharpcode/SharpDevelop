// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xaml;
using System.Xml;
using System.Xml.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.XmlEditor;

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
					XDocument document = XDocument.Load(reader, LoadOptions.SetLineInfo);
					document.Declaration = null;
					if (Refactor(provider.TextEditor, document)) {
						using (provider.TextEditor.Document.OpenUndoGroup()) {
							provider.TextEditor.Document.Text = document.ToString();
						}
					}
				}
			} catch (XmlException e) {
				MessageService.ShowException(e);
			}
		}
		
		static XmlWriterSettings CreateSettings()
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Indent = true;
			settings.OmitXmlDeclaration = true;
			settings.NewLineOnAttributes = true;
			return settings;
		}
		
		protected abstract bool Refactor(ITextEditor editor, XDocument document);
	}
}
