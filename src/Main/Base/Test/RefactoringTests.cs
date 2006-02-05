// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.IO;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;

namespace ICSharpCode.SharpDevelop.Tests
{
	//[TestFixture]
	public class RefactoringTests
	{
		const string code = @"using System;
abstract class BaseClass {
	protected abstract void FirstMethod();
	
	protected virtual void SecondMethod()
	{
	}
}
class DerivedClass : BaseClass
{
	protected override void FirstMethod()
	{
		SecondMethod();
	}
}
class SecondDerivedClass : DerivedClass
{
	protected override void FirstMethod()
	{
		Console.Beep();
	}
	protected override void SecondMethod()
	{
		FirstMethod();
	}
}
";
		
		// TODO: Write unit tests for find references / find overrides / go to base class.
	}
}
