// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;

using Debugger;

namespace Debugger.AddIn.TreeModel
{
	public partial class Util
	{
		public static IEnumerable<AbstractNode> GetChildNodesOfObject(Expression expression, DebugType shownType)
		{
			if (shownType.BaseType != null) {
				yield return new BaseClassNode(expression, shownType.BaseType);
			}
			yield return new PrivateMembersNode(expression, shownType);
			yield return new StaticMembersNode(expression, shownType);
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.Public | BindingFlags.Instance)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
	
	public class BaseClassNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public BaseClassNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Image = DebuggerIcons.ImageList.Images[0]; // Class
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.BaseClass}");
			this.Type = shownType.FullName;
			this.ChildNodes = Util.GetChildNodesOfObject(expression, shownType);
		}
	}
	
	public class PrivateMembersNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public PrivateMembersNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.NonPublic | BindingFlags.Instance)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
	
	public class StaticMembersNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public StaticMembersNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.StaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			yield return new PrivateStaticMembersNode(expression, shownType);
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.Public | BindingFlags.Static)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
	
	public class PrivateStaticMembersNode: AbstractNode
	{
		Expression expression;
		DebugType shownType;
		
		public PrivateStaticMembersNode(Expression expression, DebugType shownType)
		{
			this.expression = expression;
			this.shownType = shownType;
			
			this.Name = StringParser.Parse("${res:MainWindow.Windows.Debug.LocalVariables.PrivateStaticMembers}");
			this.ChildNodes = GetChildNodes();
		}
		
		IEnumerable<AbstractNode> GetChildNodes()
		{
			foreach(Expression childExpr in expression.AppendObjectMembers(shownType, BindingFlags.NonPublic | BindingFlags.Static)) {
				yield return new ExpressionNode(childExpr);
			}
		}
	}
}
