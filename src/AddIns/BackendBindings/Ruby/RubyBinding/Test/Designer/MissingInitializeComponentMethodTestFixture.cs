// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
