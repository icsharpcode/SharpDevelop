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

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Utils
{
	/// <summary>
	/// Base class that is used when creating dialogs from the Wix xml.
	/// </summary>
	public class DialogLoadingTestFixtureBase : IComponentCreator, IFileLoader
	{
		List <CreatedComponent> createdComponents = new List<CreatedComponent>();
		List <string> bitmapFileNamesRequested = new List<string>();

		public DialogLoadingTestFixtureBase()
		{
		}
		
		public IComponent CreateComponent(Type componentClass, string name)
		{
			CreatedComponent c = new CreatedComponent(componentClass.FullName, name);
			createdComponents.Add(c);
			
			object instance = componentClass.Assembly.CreateInstance(componentClass.FullName);
			Control control = instance as Control;
			return (IComponent)instance;
		}
		
		public Bitmap LoadBitmap(string fileName)
		{
			bitmapFileNamesRequested.Add(fileName);
			return new Bitmap(10, 10);
		}
		
		/// <summary>
		/// Gets a list of the bitmap filenames requested through the GetBitmapFromFileName
		/// method.</summary>
		protected List<string> BitmapFileNamesRequested {
			get { return bitmapFileNamesRequested; }
		}
		
		/// <summary>
		/// Gets a list of the components created via the IComponentCreator.Create 
		/// method.</summary>
		protected List<CreatedComponent> CreatedComponents {
			get { return createdComponents; }
		}
	}
}
