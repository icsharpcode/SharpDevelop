/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 6/13/2013
 * Time: 17:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Arguments for the callback of <see cref="FindReferences.RenameReferencesInFile"/>.
	/// </summary>
	public class RenameCallbackArguments
	{
		public AstNode NodeToReplace { get; private set; }
		public AstNode NewNode { get; private set; }
		
		public RenameCallbackArguments(AstNode nodeToReplace, AstNode newNode)
		{
			if (nodeToReplace == null)
				throw new ArgumentNullException("nodeToReplace");
			if (newNode == null)
				throw new ArgumentNullException("newNode");
			this.NodeToReplace = nodeToReplace;
			this.NewNode = newNode;
		}
	}
}
