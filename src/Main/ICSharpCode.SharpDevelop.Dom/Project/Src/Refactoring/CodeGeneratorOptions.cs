// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom.Refactoring
{
	public class CodeGeneratorOptions
	{
		public bool BracesOnSameLine = true;
		public bool EmptyLinesBetweenMembers = true;
		string indentString = "\t";
		
		public string IndentString {
			get { return indentString; }
			set {
				if (string.IsNullOrEmpty(value)) {
					throw new ArgumentNullException("value");
				}
				indentString = value;
			}
		}
	}
}
