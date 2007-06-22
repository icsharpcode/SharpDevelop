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

using ICSharpCode.Core;
using System;
using System.Collections;

namespace CodonCreation
{
	/// <summary>
	/// Class that can build an object out of a Codon in the .addin file.
	/// </summary>
	/// <remarks>http://en.wikipedia.org/wiki/Fraggle_Rock#Doozers</remarks>
	public class TestDoozer : IDoozer
	{
		public TestDoozer()
		{
		}
		
		/// <summary>
		/// Gets if the doozer handles codon conditions on its own.
		/// If this property return false, the item is excluded when the
		/// condition is not met.
		/// </summary>
		public bool HandleConditions {
			get {
				return false;
			}
		}
		
		// BuildItem:
		// This method actually builds an object which is used by the add-in
		//
		// The BuildItem gets following arguments :
		// a caller object               : this is the object which creates the item
		// a codon                       : the codon read from the .addin file, the doozer
		//                                 uses the information in the codon to build
		//                                 a custom codon object.
		// A arraylist with the subitems : if this codon has subitems in it's path here are the
		//                                 build items stored for these the codon may use these
		//                                 or not, if not they get lost, if any were there.
		//                                 'Normal' codons don't need them (then the tree-path is a list)
		//                                 But for example menuitems use them.
		public object BuildItem(object caller, Codon codon, ArrayList subItems)
		{
			return new TestCodon(codon.Properties["text"]);
		}
	}
}
