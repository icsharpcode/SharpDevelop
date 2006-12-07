/*
 * User: dickon
 * Date: 06/12/2006
 * Time: 12:53
 * 
 */

using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.IO;

using ICSharpCode.TextEditor.Document;

namespace SharpDbTools.Forms
{
	/// <summary>
	/// Implementation specifically for SQLTool, based on
	/// a copy-and-paste reuse of ICSharpCode.TextEditor.ResourceSyntaxModeProvider
	/// </summary>
	public class SQLToolResourceSyntaxModeProvider: ISyntaxModeFileProvider
	{
		List<SyntaxMode> syntaxModes = null;
		
		public ICollection<SyntaxMode> SyntaxModes {
			get {
				return syntaxModes;
			}
		}
		
		public SQLToolResourceSyntaxModeProvider()
		{
			Assembly assembly = this.GetType().Assembly;
			Stream syntaxModeStream = assembly.GetManifestResourceStream("SharpDbTools.Resources.SyntaxModes.xml");
			if (syntaxModeStream != null) {
				syntaxModes = SyntaxMode.GetSyntaxModes(syntaxModeStream);
			} else {
				syntaxModes = new List<SyntaxMode>();
			}
		}
		
		public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
		{
			Assembly assembly = this.GetType().Assembly;
			return new XmlTextReader(assembly.GetManifestResourceStream("SharpDbTools.Resources." + syntaxMode.FileName));
		}
		
		public void UpdateSyntaxModeList()
		{
			// resources don't change during runtime
		}
	}
}
