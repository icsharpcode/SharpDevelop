// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting.Tests.Utils;
using IronRuby.Compiler.Ast;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	[TestFixture]
	public class DeserializeMethodParametersTestFixture
	{
		RubyCodeDeserializer deserializer;
		MockComponentCreator componentCreator;
		
		[SetUp]
		public void Init()
		{
			componentCreator = new MockComponentCreator();
			MockDesignerLoaderHost mockDesignerLoaderHost = new MockDesignerLoaderHost();
			deserializer = new RubyCodeDeserializer(componentCreator);		
		}
		
		[Test]
		public void NegativeIntegerParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(-1);
			
			string code = "TestClass(-1)";
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}

		[Test]
		public void NegativeDoubleParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(-1.0);
			
			string code = "TestClass(-1.0)";
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}	
		
		[Test]
		public void EnumParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(AnchorStyles.Top);
			
			string code = "TestClass(System::Windows::Forms::AnchorStyles.Top)";
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void BooleanParameter()
		{			
			List<object> expectedArgs = new List<object>();
			expectedArgs.Add(true);
			
			string code = "TestClass(true)";
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);
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
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);
			List<object> args = deserializer.GetArguments(callExpression);

			Assert.AreEqual(expectedArgs, args);
		}
		
		[Test]
		public void MethodHasNoArguments()
		{
			string code = "TestClass()";
			MethodCall callExpression = RubyParserHelper.GetMethodCall(code);
			List<object> args = deserializer.GetArguments(callExpression);
			
			List<object> expectedArgs = new List<object>();
			Assert.AreEqual(expectedArgs, args);
		}
	}
}
