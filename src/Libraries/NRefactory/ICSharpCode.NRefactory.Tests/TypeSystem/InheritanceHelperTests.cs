//
// BlobReader.cs
//
// Author:
//       Erik Källen
//
// Copyright (c) 2010-2013
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.TypeSystem
{
	[TestFixture]
	public class InheritanceHelperTests
	{
		[Test]
		public void DynamicAndObjectShouldBeConsideredTheSameTypeWhenMatchingSignatures()
		{
			string program = @"using System.Collections.Generic;
public class Base {
  public virtual void M1(object p) {}
  public virtual void M2(List<object> p) {}
  public virtual object M3() { return null; }
  public virtual List<object> M4() { return null; }
  public virtual void M5(dynamic p) {}
  public virtual void M6(List<dynamic> p) {}
  public virtual dynamic M7() { return null; }
  public virtual List<dynamic> M8() { return null; }
}

public class Derived : Base {
  public override void M1(dynamic p) {}
  public override void M2(List<dynamic> p) {}
  public override dynamic M3() { return null; }
  public override List<dynamic> M4() { return null; }
  public override void M5(object p) {}
  public override void M6(List<object> p) {}
  public override object M7() { return null; }
  public override List<object> M8() { return null; }
}";

			var unresolvedFile = new CSharpParser().Parse(program, "program.cs").ToTypeSystem();
			var compilation = new CSharpProjectContent().AddAssemblyReferences(CecilLoaderTests.Mscorlib).AddOrUpdateFiles(unresolvedFile).CreateCompilation();

			var dtype = (ITypeDefinition)ReflectionHelper.ParseReflectionName("Derived").Resolve(compilation);
			var btype = (ITypeDefinition)ReflectionHelper.ParseReflectionName("Base").Resolve(compilation);

			foreach (var name in new[] { "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8" }) {
				Assert.That(InheritanceHelper.GetBaseMember(dtype.Methods.Single(m => m.Name == name)), Is.EqualTo(btype.Methods.Single(m => m.Name == name)), name + " does not match");
			}

			foreach (var name in new[] { "M1", "M2", "M3", "M4", "M5", "M6", "M7", "M8" }) {
				Assert.That(InheritanceHelper.GetDerivedMember(btype.Methods.Single(m => m.Name == name), dtype), Is.EqualTo(dtype.Methods.Single(m => m.Name == name)), name + " does not match");
			}
		}
	}
}