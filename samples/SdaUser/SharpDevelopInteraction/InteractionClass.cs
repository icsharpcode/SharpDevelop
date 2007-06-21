// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Gui;

namespace SharpDevelopInteraction
{
	/// <summary>
	/// If you want to control SharpDevelop's internals from the host application,
	/// you need to use a wrapper assembly and class like this.
	/// Make sure to inherit from MarshalByRefObject and create the instance using
	/// host.CreateInstanceInTargetDomain&lt;T&gt; in the host application.
	/// 
	/// The class itself will be responsible to use WorkbenchSingleton.SafeThread(Async)Call
	/// for all operations if SharpDevelop is running on its own thread.
	/// </summary>
	public class InteractionClass : MarshalByRefObject
	{
		public void MakeTransparent()
		{
			WorkbenchSingleton.SafeThreadAsyncCall(MakeTransparentInternal);
		}
		
		void MakeTransparentInternal()
		{
			WorkbenchSingleton.MainForm.Opacity *= 0.85;
			if (WorkbenchSingleton.MainForm.Opacity < 0.2)
				WorkbenchSingleton.MainForm.Opacity = 1;
		}
	}
}
