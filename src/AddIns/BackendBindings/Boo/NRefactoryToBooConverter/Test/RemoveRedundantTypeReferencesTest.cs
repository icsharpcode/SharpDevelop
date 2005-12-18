// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using NUnit.Framework;

namespace NRefactoryToBooConverter.Tests
{
	[TestFixture]
	public class RemoveRedundantTypeReferencesTest : TestHelper
	{
		protected override void ApplySettings(ConverterSettings settings)
		{
			base.ApplySettings(settings);
			settings.SimplifyTypeNames = true;
			settings.RemoveRedundantTypeReferences = true;
		}
		
		[Test]
		public void VoidMethod()
		{
			TestInClass("private void Main() {}", "private def Main():\n\tpass");
		}
		
		[Test]
		public void ConstructorCall()
		{
			TestStatement("MyClass m = new MyClass(4);", "m = MyClass(4)");
		}
		
		[Test]
		public void Cast()
		{
			TestStatement("MyClass m = (MyClass)variable;", "m = cast(MyClass, variable)");
			TestStatement("MyClass m = variable as MyClass;", "m = (variable as MyClass)");
		}
		
		[Test]
		public void PrimitiveAssignment()
		{
			TestStatement("string text = \"Text!\";", "text = 'Text!'");
			TestStatement("int a = 5;", "a = 5");
		}
		
		[Test]
		public void FieldAndLocalVariable()
		{
			TestInClass("private int var = 3;\n" +
			            "private void Main() {\n" +
			            "  int var = 5;\n" +
			            "}",
			            "private var = 3\n" +
			            "private def Main():\n" +
			            "\tvar as int = 5");
		}
	}
}
