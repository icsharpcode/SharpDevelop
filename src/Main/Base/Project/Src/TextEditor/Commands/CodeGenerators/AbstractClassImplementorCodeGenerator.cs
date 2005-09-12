// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.TextEditor;

using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public class AbstractClassImplementorCodeGenerator : InterfaceOrAbstractClassCodeGenerator
	{
		public override string CategoryName {
			get {
				return "Abstract class overridings";
			}
		}
		
		public override  string Hint {
			get {
				return "Choose abstract class to override";
			}
		}
		
		public AbstractClassImplementorCodeGenerator(IClass currentClass) : base(currentClass)
		{
			base.useOverrideKeyword = true;
			for (int i = 0; i < currentClass.BaseTypes.Count; i++) {
				IReturnType baseType = currentClass.GetBaseType(i);
				IClass baseClass = (baseType != null) ? baseType.GetUnderlyingClass() : null;
				if (baseClass != null && baseClass.ClassType == ClassType.Class && baseClass.IsAbstract) {
					Content.Add(new ClassWrapper(baseType));
				}
			}
		}
	}
}
