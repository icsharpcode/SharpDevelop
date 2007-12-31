// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

using System;
using System.Collections.Generic;

namespace Debugger
{
	public class FunctionItem: ListItem
	{
		Function function;
		
		public Function Function {
			get {
				return function;
			}
		}
		
		public override int ImageIndex {
			get {
				return -1;
			}
		}
		
		public override string Name {
			get {
				return function.Name;
			}
		}
		
		public override string Text {
			get {
				return String.Empty;
			}
		}
		
		public override bool CanEditText {
			get {
				return false;
			}
		}
		
		public override string Type {
			get {
				return String.Empty;
			}
		}
		
		public override bool HasSubItems {
			get {
				return true;
			}
		}
		
		public override IList<ListItem> SubItems {
			get {
				List<ListItem> ret = new List<ListItem>();
				foreach(Value val in function.LocalVariables) {
					ret.Add(new ValueItem(val));
				}
				return ret.AsReadOnly();
			}
		}
		
		public FunctionItem(Function function)
		{
			this.function = function;
			this.function.Process.DebuggeeStateChanged += delegate {
				this.OnChanged(new ListItemEventArgs(this));
			};
		}
	}
}
