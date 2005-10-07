#region license
// Copyright (c) 2005, Daniel Grunwald (daniel@danielgrunwald.de)
// All rights reserved.
//
// NRefactoryToBoo is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// NRefactoryToBoo is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with NRefactoryToBoo; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace NRefactoryToBooConverter
{
	/// <summary>
	/// This class tries to find out the type of an identifier by looking at the NRefactory AST.
	/// The possibilities inside the parser are very limited, we can only
	/// search for local variables and fields.
	/// </summary>
	public class VariableResolver
	{
		StringComparer nameComparer;
		
		public VariableResolver(StringComparer nameComparer)
		{
			this.nameComparer = nameComparer;
		}
		
		public TypeReference FindType(string name, Statement currentStatement)
		{
			INode node = currentStatement;
			while ((node = node.Parent) != null) {
				foreach (INode childNode in node.Children) {
					LocalVariableDeclaration varDecl = childNode as LocalVariableDeclaration;
					if (varDecl != null) {
						foreach (VariableDeclaration var in varDecl.Variables) {
							if (nameComparer.Equals(var.Name, name))
								return var.TypeReference;
						}
					}
				}
			}
			return null;
		}
	}
}
