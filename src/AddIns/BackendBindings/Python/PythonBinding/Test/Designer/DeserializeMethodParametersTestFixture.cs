// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronPython.Compiler.Ast;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Designer
{
	[TestFixture]
	public class DeserializeCallParametersTestFixture
	{
		PythonCodeDeserializer deserializer;
		MockComponentCreator componentCreator;
		
		[SetUp]
		public void Init()
		{
			componentCreator = new MockComponentCreator();
			MockDesignerLoaderHost mockDesignerLoaderHost = new MockDesignerLoaderHost();
			deserializer = new PythonCodeDeserializer(componentCreator);		
		}
		
		[Test]
		public void NegativeIntegerParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(-1);
			
			string code = "TestClass(-1)";
			CallExpression callExpression = PythonParserHelper.GetCallExpression(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}

		[Test]
		public void NegativeDoubleParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(-1.0);
			
			string code = "TestClass(-1.0)";
			CallExpression callExpression = PythonParserHelper.GetCallExpression(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}	
		
		[Test]
		public void EnumParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(AnchorStyles.Top);
			
			string code = "TestClass(System.Windows.Forms.AnchorStyles.Top)";
			CallExpression callExpression = PythonParserHelper.GetCallExpression(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void BooleanParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(true);
			
			string code = "TestClass(True)";
			CallExpression callExpression = PythonParserHelper.GetCallExpression(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void LocalVariableInstance()
		{			
			string s = "abc";
			CreatedInstance instance = new CreatedInstance(typeof(string), new object[0], "localVariable", false);
			instance.Object = s;
			componentCreator.CreatedInstances.Add(instance);
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(s);
			
			string code = "TestClass(localVariable)";
			CallExpression callExpression = PythonParserHelper.GetCallExpression(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}		
		
	}
}
