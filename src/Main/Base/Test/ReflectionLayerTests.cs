/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 17.05.2005
 * Time: 21:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

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
		IProjectContent pc = ProjectContentRegistry.GetMscorlibContent();
		
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
	}
}
