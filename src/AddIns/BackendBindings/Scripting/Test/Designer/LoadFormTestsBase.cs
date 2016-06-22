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

//using System;
//using System.Drawing;
//using System.Reflection;
//using System.Windows.Forms;
//
//using ICSharpCode.Scripting;
//using ICSharpCode.Scripting.Tests.Utils;
//using NUnit.Framework;
//
//namespace ICSharpCode.Scripting.Tests.Designer
//{
//	public abstract class LoadFormTestsBase
//	{		
//		MockComponentCreator componentCreator = new MockComponentCreator();
//		Form form;
//				
//		[TestFixtureSetUp]
//		public void SetUpFixture()
//		{			
//			BeforeSetUpFixture();
//			IComponentWalker walker = CreateComponentWalker(componentCreator);
//			form = walker.CreateComponent(Code) as Form;
//		}
//
//		[TestFixtureTearDown]
//		public void TearDownFixture()
//		{
//			if (form != null) {
//				form.Dispose();
//			}
//		}
//
//		/// <summary>
//		/// Called at the start of SetUpFixture method before anything is setup.
//		/// </summary>
//		public virtual void BeforeSetUpFixture()
//		{
//		}
//		
//		protected abstract IComponentWalker CreateComponentWalker(IComponentCreator componentCreator);
//		
//		/// <summary>
//		/// Gets the code that will be loaded.
//		/// </summary>
//		public virtual string Code {
//			get { return String.Empty; }
//		}
//		
//		protected MockComponentCreator ComponentCreator {
//			get { return componentCreator; }
//		}
//				
//		protected Form Form {
//			get { return form; }
//		}		
//	}
//}
