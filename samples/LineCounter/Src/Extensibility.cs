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
using ICSharpCode.Core;

namespace LineCounterAddin
{
	// Improvement 2
	
	public interface ICountingAlgorithm
	{
		void CountLines(LineCountInfo info);
	}

	public class CountingAlgorithmGeneric : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesGeneric(info);
		}
	}
	public class CountingAlgorithmCStyle : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesCStyle(info);
		}
	}
	public class CountingAlgorithmVBStyle : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesVBStyle(info);
		}
	}
	public class CountingAlgorithmXmlStyle : ICountingAlgorithm {
		public void CountLines(LineCountInfo info) {
			LineCounterBrowser.CountLinesXMLStyle(info);
		}
	}

	public class CountingAlgorithmDoozer : IDoozer
	{
		public bool HandleConditions {
			get {
				// our doozer cannot handle conditions, let SharpDevelop
				// do that for us
				return false;
			}
		}
		
		public object BuildItem(object caller, Codon codon, System.Collections.ArrayList subItems)
		{
			return new CountingAlgorithmDescriptor(codon.AddIn,
			                                       codon.Properties["extensions"],
			                                       codon.Properties["class"]);
		}
	}

	public class CountingAlgorithmDescriptor
	{
		AddIn addIn;
		internal string[] extensions;
		string className;
		
		public CountingAlgorithmDescriptor(AddIn addIn, string extensions, string className)
		{
			this.addIn = addIn;
			this.extensions = extensions.ToLowerInvariant().Split(';');
			this.className = className;
		}
		
		public bool CanCountLines(LineCountInfo info)
		{
			return (Array.IndexOf(extensions, info.FileType.ToLowerInvariant()) >= 0);
		}
		
		ICountingAlgorithm cachedAlgorithm;
		
		public ICountingAlgorithm GetAlgorithm()
		{
			if (cachedAlgorithm == null) {
				cachedAlgorithm = (ICountingAlgorithm)addIn.CreateObject(className);
			}
			return cachedAlgorithm;
		}
	}
}
