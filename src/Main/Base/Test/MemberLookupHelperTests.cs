// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ICSharpCode.SharpDevelop.Tests
{
	[TestFixture]
	public class MemberLookupHelperTests
	{
		IProjectContent msc = ProjectContentRegistry.Mscorlib;
		IProjectContent swf = ProjectContentRegistry.GetProjectContentForReference("System.Windows.Forms", "System.Windows.Forms");
		
		public IReturnType DictionaryRT {
			get {
				return new GetClassReturnType(msc, "System.Collections.Generic.Dictionary", 2);
			}
		}
		
		public IClass EnumerableClass {
			get {
				return msc.GetClass("System.Collections.Generic.IEnumerable", 1);
			}
		}
		
		[Test]
		public void TypeParameterPassedToBaseClassTest()
		{
			IReturnType[] stringInt = { msc.SystemTypes.String, msc.SystemTypes.Int32 };
			IReturnType rrt = new ConstructedReturnType(DictionaryRT, stringInt);
			IReturnType res = MemberLookupHelper.GetTypeParameterPassedToBaseClass(rrt, EnumerableClass, 0);
			Assert.AreEqual("System.Collections.Generic.KeyValuePair", res.FullyQualifiedName);
			ConstructedReturnType resc = res.CastToConstructedReturnType();
			Assert.AreEqual("System.String", resc.TypeArguments[0].FullyQualifiedName);
			Assert.AreEqual("System.Int32", resc.TypeArguments[1].FullyQualifiedName);
		}
		
		[Test]
		public void TypeParameterPassedToBaseClassSameClass()
		{
			IReturnType[] stringArr = { msc.SystemTypes.String };
			IReturnType rrt = new ConstructedReturnType(EnumerableClass.DefaultReturnType, stringArr);
			IReturnType res = MemberLookupHelper.GetTypeParameterPassedToBaseClass(rrt, EnumerableClass, 0);
			Assert.AreEqual("System.String", res.FullyQualifiedName);
		}
		
		[Test]
		public void GetCommonType()
		{
			IReturnType res = MemberLookupHelper.GetCommonType(msc,
			                                                   swf.GetClass("System.Windows.Forms.ToolStripButton").DefaultReturnType,
			                                                   swf.GetClass("System.Windows.Forms.ToolStripSeparator").DefaultReturnType);
			Assert.AreEqual("System.Windows.Forms.ToolStripItem", res.FullyQualifiedName);
		}
	}
}
