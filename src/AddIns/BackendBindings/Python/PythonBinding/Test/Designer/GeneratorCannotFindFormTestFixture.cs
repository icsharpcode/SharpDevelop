// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;

using ICSharpCode.PythonBinding;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class GeneratorCannotFindFormTestFixture
	{
		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void CannotFindFormFromCodeCompileUnit()
		{
			//PythonProvider provider = new PythonProvider();
			CodeCompileUnit unit = new CodeCompileUnit();
			PythonDesignerGenerator generator = new PythonDesignerGenerator();
			generator.MergeFormChanges(unit);
		}
	}
}
