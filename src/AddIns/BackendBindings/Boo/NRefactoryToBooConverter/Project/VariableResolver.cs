// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.Ast;

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
