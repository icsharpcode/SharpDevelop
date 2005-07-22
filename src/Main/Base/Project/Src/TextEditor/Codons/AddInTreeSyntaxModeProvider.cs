// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Xml;
using System.Collections;
using ICSharpCode.Core;
using ICSharpCode.TextEditor.Document;
namespace ICSharpCode.SharpDevelop.DefaultEditor.Codons
{
	/// <summary>
	/// Description of AddInTreeSyntaxModeProvider.
	/// </summary>
	public class AddInTreeSyntaxModeProvider : ISyntaxModeFileProvider
	{
		const string syntaxModePath = "/SharpDevelop/ViewContent/DefaultTextEditor/SyntaxModes";
		
		ArrayList syntaxModes;
		
		public ArrayList SyntaxModes {
			get {
				return syntaxModes;
			}
		}
		
		public AddInTreeSyntaxModeProvider()
		{
			try {
				syntaxModes = AddInTree.GetTreeNode(syntaxModePath).BuildChildItems(this);
			} catch (TreePathNotFoundException) {
				syntaxModes = new ArrayList();
			}
		}
		
		public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
		{
			Debug.Assert(syntaxMode is AddInTreeSyntaxMode);
			return ((AddInTreeSyntaxMode)syntaxMode).CreateTextReader();
		}
	}
}
