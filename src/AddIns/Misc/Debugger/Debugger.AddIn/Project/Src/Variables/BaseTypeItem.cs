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

using ICSharpCode.Core;

namespace Debugger
{
	public class BaseTypeItem: ListItem
	{
		Value val;
		DebugType type;
		
		public Value Value {
			get {
				return val;
			}
		}
		
		public DebugType DebugType {
			get {
				return type;
			}
		}
		
		public override int ImageIndex {
			get {
				return -1;
			}
		}
		
		public override string Name {
			get {
				return StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
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
				return type.FullName;
			}
		}
		
		public override bool HasSubItems {
			get {
				return true;
			}
		}
		
		public override IList<ListItem> SubItems {
			get {
				return GetSubItems();
			}
		}
		
		public BaseTypeItem(Value val, DebugType type)
		{
			this.val = val;
			this.type = type;
		}
		
		List<ListItem> GetMembers(BindingFlags flags)
		{
			List<ListItem> list = new List<ListItem>();
			foreach(Value v in val.GetMembers(type, flags)) {
				list.Add(new ValueItem(v));
			}
			return list;
		}
		
		IList<ListItem> GetSubItems()
		{
			List<ListItem> list = new List<ListItem>();
			
			string privateRes = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateMembers}");
			string staticRes = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}");
			string privateStaticRes = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateStaticMembers}");
			
			List<ListItem> publicInstance  = GetMembers(BindingFlags.Public | BindingFlags.Instance);
			List<ListItem> publicStatic    = GetMembers(BindingFlags.Public | BindingFlags.Static);
			List<ListItem> privateInstance = GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
			List<ListItem> privateStatic   = GetMembers(BindingFlags.NonPublic | BindingFlags.Static);
			
			if (type.BaseType != null) {
				list.Add(new BaseTypeItem(val, type.BaseType));
			}
			
			if (publicStatic.Count > 0) {
				list.Add(new FixedItem(-1, staticRes, String.Empty, String.Empty, true, publicStatic));
			}
			
			if (privateInstance.Count > 0 || privateStatic.Count > 0) {
				List<ListItem> nonPublicItems = new List<ListItem>();
				if (privateStatic.Count > 0) {
					nonPublicItems.Add(new FixedItem(-1, privateStaticRes, String.Empty, String.Empty, true, privateStatic));
				}
				nonPublicItems.AddRange(privateInstance);
				list.Add(new FixedItem(-1, privateRes, String.Empty, String.Empty, true, nonPublicItems));
			}
			
			list.AddRange(publicInstance);
			
			return list;
		}
	}
}
