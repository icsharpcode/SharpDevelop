// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using IronPython.Modules;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class GetMethodsFromSysLibraryTestFixture
	{
		SysModuleCompletionItems completionItems;
		MethodGroup exitMethodGroup;
		MethodGroup displayHookMethodGroup;
		MethodGroup setDefaultEncodingMethodGroup;
		
		[SetUp]
		public void Init()
		{
			PythonStandardModuleType moduleType = new PythonStandardModuleType(typeof(SysModule), "sys");
			completionItems = new SysModuleCompletionItems(moduleType);
			exitMethodGroup = completionItems.GetMethods("exit");
			displayHookMethodGroup = completionItems.GetMethods("displayhook");
			setDefaultEncodingMethodGroup = completionItems.GetMethods("setdefaultencoding");
		}
		
		[Test]
		public void TwoExitMethodsReturnedFromGetMethods()
		{
			Assert.AreEqual(2, exitMethodGroup.Count);
		}
		
		[Test]
		public void FirstMethodNameIsExit()
		{
			Assert.AreEqual("exit", exitMethodGroup[0].Name);
		}
		
		[Test]
		public void SecondMethodNameIsExit()
		{
			Assert.AreEqual("exit", exitMethodGroup[1].Name);
		}
		
		[Test]
		public void FirstMethodHasReturnType()
		{
			Assert.IsNotNull(exitMethodGroup[0].ReturnType);
		}
		
		[Test]
		public void SecondMethodHasReturnType()
		{
			Assert.IsNotNull(exitMethodGroup[1].ReturnType);
		}
		
		[Test]
		public void ExitMethodReturnsVoid()
		{
			IMethod method = exitMethodGroup[0];
			Assert.AreEqual("Void", method.ReturnType.Name);
		}
		
		[Test]
		[Ignore("TODO displayhook function replaced by BuiltinFunction field")]
		public void DisplayHookMethodDoesNotHaveCodeContextParameter()
		{
			IMethod method = displayHookMethodGroup[0];
			IParameter parameter = method.Parameters[0];
			Assert.AreEqual("value", parameter.Name);
		}
		
		[Test]
		[Ignore("TODO displayhook function replaced by BuiltinFunction field")]
		public void DisplayHookMethodReturnsVoid()
		{
			IMethod method = displayHookMethodGroup[0];
			Assert.AreEqual("Void", method.ReturnType.Name);
		}
		
		[Test]
		public void GetDefaultEncodingMethodReturnsString()
		{
			MethodGroup methodGroup = completionItems.GetMethods("getdefaultencoding");
			IMethod method = methodGroup[0];
			Assert.AreEqual("String", method.ReturnType.Name);
		}
		
		[Test]
		public void SetDefaultEncodingMethodDoesNotHaveCodeContextParameter()
		{
			IMethod method = setDefaultEncodingMethodGroup[0];
			IParameter parameter = method.Parameters[0];
			Assert.AreEqual("name", parameter.Name);
		}
		
		[Test]
		public void SetDefaultEncodingMethodDReturnsVoid()
		{
			IMethod method = setDefaultEncodingMethodGroup[0];
			Assert.AreEqual("Void", method.ReturnType.Name);
		}
	}
}
