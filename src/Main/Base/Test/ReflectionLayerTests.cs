// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using NUnit.Framework;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class ReflectionLayerTests
	{
		IProjectContent pc = ProjectContentRegistry.Mscorlib;
		
		[Test]
		public void InheritanceTest()
		{
			IClass c = pc.GetClass("System.SystemException");
			IClass c2 = pc.GetClass("System.Exception");
			Assert.IsNotNull(c, "c is null");
			Assert.IsNotNull(c2, "c2 is null");
			Assert.AreEqual(3, c.BaseTypes.Count); // 2 interfaces
			Assert.AreEqual("System.Exception", c.BaseTypes[0]);
			Assert.AreSame(c2, c.BaseClass);
			
			List<IClass> subClasses = new List<IClass>();
			foreach (IClass subClass in c.ClassInheritanceTree) {
				subClasses.Add(subClass);
			}
			Assert.AreEqual(5, subClasses.Count, "ClassInheritanceTree length");
			Assert.AreEqual("System.SystemException", subClasses[0].FullyQualifiedName);
			Assert.AreEqual("System.Exception", subClasses[1].FullyQualifiedName);
			Assert.AreEqual("System.Runtime.Serialization.ISerializable", subClasses[2].FullyQualifiedName);
			Assert.AreEqual("System.Runtime.InteropServices._Exception", subClasses[3].FullyQualifiedName);
			Assert.AreEqual("System.Object", subClasses[4].FullyQualifiedName);
		}
		
		[Test]
		public void ParameterComparisonTest()
		{
			DefaultParameter p1 = new DefaultParameter("a", pc.GetClass("System.String").DefaultReturnType, null);
			DefaultParameter p2 = new DefaultParameter("b", new GetClassReturnType(pc, "System.String"), null);
			List<IParameter> a1 = new List<IParameter>();
			List<IParameter> a2 = new List<IParameter>();
			a1.Add(p1);
			a2.Add(p2);
			Assert.AreEqual(0, DiffUtility.Compare(a1, a2));
		}
		
		DefaultMethod GetMethod(IClass c, string name) {
			IMethod result = c.Methods.Find(delegate(IMethod m) { return m.Name == name; });
			Assert.IsNotNull(result, "Method " + name + " not found");
			return (DefaultMethod)result;
		}
		
		[Test]
		public void GenericDocumentationTagNamesTest()
		{
			DefaultClass c = (DefaultClass)pc.GetClass("System.Collections.Generic.List");
			Assert.AreEqual("T:System.Collections.Generic.List`1",
			                c.DocumentationTag);
			Assert.AreEqual("M:System.Collections.Generic.List`1.Add(`0)",
			                GetMethod(c, "Add").DocumentationTag);
			Assert.AreEqual("M:System.Collections.Generic.List`1.AddRange(System.Collections.Generic.IEnumerable{`0})",
			                GetMethod(c, "AddRange").DocumentationTag);
			Assert.AreEqual("M:System.Collections.Generic.List`1.ConvertAll``1(System.Converter{`0,``0})",
			                GetMethod(c, "ConvertAll").DocumentationTag);
		}
		
		[Test]
		public void InnerClassReferenceTest()
		{
			IClass c = pc.GetClass("System.Environment");
			Assert.IsNotNull(c, "System.Environment not found");
			IReturnType rt = GetMethod(c, "GetFolderPath").Parameters[0].ReturnType;
			Assert.IsNotNull(rt, "ReturnType is null");
			Assert.AreEqual("System.Environment.SpecialFolder", rt.FullyQualifiedName);
			IClass inner = rt.GetUnderlyingClass();
			Assert.IsNotNull(inner, "UnderlyingClass");
			Assert.AreEqual("System.Environment.SpecialFolder", inner.FullyQualifiedName);
		}
		
		[Test]
		public void InnerClassesTest()
		{
			IClass c = pc.GetClass("System.Environment.SpecialFolder");
			Assert.IsNotNull(c, "c is null");
			Assert.AreEqual("System.Environment.SpecialFolder", c.FullyQualifiedName);
		}
		
		[Test]
		public void VoidTest()
		{
			IClass c = pc.GetClass("System.Void");
			Assert.IsNotNull(c, "System.Void not found");
			Assert.AreSame(c.DefaultReturnType, ReflectionReturnType.Void, "ReflectionReturnType.Void is c.DefaultReturnType");
		}
	}
}
