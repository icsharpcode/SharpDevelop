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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;

using ICSharpCode.RubyBinding;
using ICSharpCode.Scripting;
using IronRuby.Compiler.Ast;
using Microsoft.Scripting;
using NUnit.Framework;
using RubyBinding.Tests.Utils;

namespace RubyBinding.Tests.Designer
{
	/// <summary>
	/// Tests that the RubyFormVisitor throws an exception if no InitializeComponent or 
	/// InitializeComponent method can be found.
	/// </summary>
	[TestFixture]
	public class MissingInitializeComponentMethodTestFixture : IComponentCreator
	{		
		string RubyCode = "class MainForm < System::Windows::Forms.Form\r\n" +
							"    def initialize()\r\n" +
							"        self.MissingMethod()\r\n" +
							"    end\r\n" +
							"\r\n" +
							"    def MissingMethod()\r\n" +
							"    end\r\n" +
							"end\r\n";
		[Test]
		[ExpectedException(typeof(RubyComponentWalkerException))]
		public void RubyFormWalkerExceptionThrown()
		{
			RubyComponentWalker walker = new RubyComponentWalker(this);
			walker.CreateComponent(RubyCode);
			Assert.Fail("Exception should have been thrown before this.");
		}
				
		public IComponent CreateComponent(Type componentClass, string name)
		{
			throw new NotImplementedException();
		}
		
		public void Add(IComponent component, string name)
		{
			throw new NotImplementedException();
		}
		
		public IComponent GetComponent(string name)
		{
			return null;
		}
		
		public IComponent RootComponent {
			get { return null; }
		}
		
		public object CreateInstance(Type type, ICollection arguments, string name, bool addToContainer)
		{
			throw new NotImplementedException();
		}
		
		public Type GetType(string typeName)
		{
			throw new NotImplementedException();
		}
		
		public PropertyDescriptor GetEventProperty(EventDescriptor e)
		{
			return null;
		}
		
		public object GetInstance(string name)
		{
			return null;
		}
	
		public IResourceReader GetResourceReader(CultureInfo info)
		{
			return null;
		}
		
		public IResourceWriter GetResourceWriter(CultureInfo info)
		{
			return null;
		}		
	}
}
