// SharpDevelop samples
// Copyright (c) 2007, AlphaSierraPapa
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

namespace ICSharpCode.NAnt
{
	/// <summary>
	/// Represents a NAnt build target.
	/// </summary>
	public class NAntBuildTarget
	{
		string name = String.Empty;
		bool isDefault;
		int line;
		int column;
		
		public NAntBuildTarget()
		{
		}
		
		/// <summary>
		/// Creates a new instance of the <see cref="NAntBuildTarget"/>
		/// with the specified name.
		/// </summary>
		/// <param name="name">The target name.</param>
		/// <param name="isDefault"><see langword="true"/> if the 
		/// target is the default target; otherwise 
		/// <see langword="false"/>.</param>
		/// <param name="line">The line number of the target element
		/// in the build file.</param>
		/// <param name="col">The column number of the target element
		/// in the build file.</param>
		public NAntBuildTarget(string name, bool isDefault, int line, int col)
		{
			this.name = name;
			this.isDefault = isDefault;
			this.line = line;
			this.column = col;
		}
		
		/// <summary>
		/// Gets the name of the target.
		/// </summary>
		public string Name {
			get {
				return name;
			}
		}
		
		/// <summary>
		/// Gets whether this is the default target.
		/// </summary>
		public bool IsDefault {
			get {
				return isDefault;
			}
		}
		
		/// <summary>
		/// Gets the line in the build file where this
		/// target can be found.
		/// </summary>
		public int Line {
			get {
				return line;
			}
		}
		
		/// <summary>
		/// Gets the column in the build file where this
		/// target can be found.
		/// </summary>
		public int Column {
			get {
				return column;
			}
		}
	}
}
