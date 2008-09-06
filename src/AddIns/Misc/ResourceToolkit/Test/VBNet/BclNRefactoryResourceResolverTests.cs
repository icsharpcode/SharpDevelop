// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;

using Hornung.ResourceToolkit;
using Hornung.ResourceToolkit.Resolver;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;

namespace ResourceToolkit.Tests.VBNet
{
	[TestFixture]
	public sealed class BclNRefactoryResourceResolverTests : AbstractVBNetResourceResolverTestFixture
	{
		// ********************************************************************************************************************************
		
		const string CodeLocalSRMDirectInitFullName = @"Class A
	Sub B
		Dim mgr As New System.Resources.ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly())
		mgr.GetString(""TestKey"")
	End Sub
	
	Sub C
		mgr.GetString(""TestKey"")
	End Sub
End Class
";
		
		[Test]
		public void LocalSRMDirectInitFullNameGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullName, 3, 18, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void LocalSRMDirectInitFullNameOutOfScope()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDirectInitFullName, 7, 18, null);
			TestHelper.CheckNoReference(rrr);
		}
		
		// ********************************************************************************************************************************
		
		const string CodeLocalSRMDeferredInitUsing = @"Imports System.Resources
Class A
	Sub B
		Dim mgr As ResourceManager
		
		mgr = New ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly())
		mgr.GetString(""TestKey"")
		mgr.GetString(
	End Sub
End Class
";
		
		[Test]
		public void LocalSRMDeferredInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDeferredInitUsing, 6, 18, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void LocalSRMDeferredInitUsingGetStringCompletion()
		{
			ResourceResolveResult rrr = Resolve(CodeLocalSRMDeferredInitUsing, 7, 16, '(');
			TestHelper.CheckReference(rrr, "Test.TestResources", null, "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeStaticPropertySRMFieldDirectInitUsing = @"Imports System.Resources
Class A
	Public Shared ReadOnly Property Resources As ResourceManager
		Get
			Return mgr
		End Get
	End Property
	
	Private Shared mgr As New ResourceManager(""Test.TestResources"", System.Reflection.Assembly.GetExecutingAssembly())
	
	Sub B()
		A.Resources.GetString(""TestKey"")
		Resources.GetString(""TestKey"")
	End Sub
End Class
";
		
		[Test]
		public void StaticPropertySRMFieldDirectInitUsingClassGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticPropertySRMFieldDirectInitUsing, 11, 25, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		[Test]
		public void StaticPropertySRMFieldDirectInitUsingGetString()
		{
			ResourceResolveResult rrr = Resolve(CodeStaticPropertySRMFieldDirectInitUsing, 12, 23, null);
			TestHelper.CheckReference(rrr, "Test.TestResources", "TestKey", "A", "A.B");
		}
		
		// ********************************************************************************************************************************
		
		const string CodeWithSyntaxErrorTooltipExceptionBug1 = @"Class A
	Sub B()
		(""OK"" & & foo)
	End Sub
End Class
";
		
		[Test]
		public void SyntaxErrorTooltipExceptionBug1()
		{
			ResourceResolveResult rrr = Resolve(CodeWithSyntaxErrorTooltipExceptionBug1, 2, 9, null);
			TestHelper.CheckNoReference(rrr);
		}
	}
}
