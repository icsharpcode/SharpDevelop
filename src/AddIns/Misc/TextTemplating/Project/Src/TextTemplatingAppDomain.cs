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
using System.IO;
using System.Reflection;

namespace ICSharpCode.TextTemplating
{
	[Serializable]
	public class TextTemplatingAppDomain : ITextTemplatingAppDomain
	{
		AppDomain appDomain;
		
		public TextTemplatingAppDomain(string applicationBase)
		{
			AppDomainSetup setupInfo = new AppDomainSetup();
			setupInfo.ApplicationBase = applicationBase;
			this.appDomain = AppDomain.CreateDomain("TextTemplatingAppDomain", null, setupInfo);
		}
		
		public AppDomain AppDomain {
			get { return this.appDomain; }
		}
		
		public void Dispose()
		{
			if (this.appDomain != null) {
				AppDomain.Unload(this.appDomain);
				this.appDomain = null;
			}
		}
	}
}
