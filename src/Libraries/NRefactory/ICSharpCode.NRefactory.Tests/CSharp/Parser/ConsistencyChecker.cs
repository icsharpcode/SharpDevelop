// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.IO;
using ICSharpCode.NRefactory.Editor;
using NUnit.Framework;

namespace ICSharpCode.NRefactory.CSharp.Parser
{
	/// <summary>
	/// Provides utilities to check whether positions and/or tokens in an AST are valid.
	/// </summary>
	public static class ConsistencyChecker
	{
		static void PrintNode (AstNode node)
		{
			Console.WriteLine ("Parent:" + node.GetType ());
			Console.WriteLine ("Children:");
			foreach (var c in node.Children)
				Console.WriteLine (c.GetType () +" at:"+ c.StartLocation +"-"+ c.EndLocation + " Role: "+ c.Role);
			Console.WriteLine ("----");
		}
		
		public static void CheckPositionConsistency (AstNode node, string currentFileName, IDocument currentDocument = null)
		{
			if (currentDocument == null)
				currentDocument = new ReadOnlyDocument(File.ReadAllText(currentFileName));
			string comment = "(" + node.GetType ().Name + " at " + node.StartLocation + " in " + currentFileName + ")";
			var pred = node.StartLocation <= node.EndLocation;
			if (!pred)
				PrintNode (node);
			Assert.IsTrue(pred, "StartLocation must be before EndLocation " + comment);
			var prevNodeEnd = node.StartLocation;
			var prevNode = node;
			for (AstNode child = node.FirstChild; child != null; child = child.NextSibling) {
				bool assertion = child.StartLocation >= prevNodeEnd;
				if (!assertion) {
					PrintNode (prevNode);
					PrintNode (node);
				}
				Assert.IsTrue(assertion, currentFileName + ": Child " + child.GetType () +" (" + child.StartLocation  + ")" +" must start after previous sibling " + prevNode.GetType () + "(" + prevNode.StartLocation + ")");
				CheckPositionConsistency(child, currentFileName, currentDocument);
				prevNodeEnd = child.EndLocation;
				prevNode = child;
			}
			Assert.IsTrue(prevNodeEnd <= node.EndLocation, "Last child must end before parent node ends " + comment);
		}
		
		public static void CheckMissingTokens(AstNode node, string currentFileName, IDocument currentDocument = null)
		{
			if (currentDocument == null)
				currentDocument = new ReadOnlyDocument(File.ReadAllText(currentFileName));
			if (IsLeafNode(node)) {
				Assert.IsNull(node.FirstChild, "Token nodes should not have children");
			} else {
				var prevNodeEnd = node.StartLocation;
				var prevNode = node;
				for (AstNode child = node.FirstChild; child != null; child = child.NextSibling) {
					CheckWhitespace(prevNode, prevNodeEnd, child, child.StartLocation, currentFileName, currentDocument);
					CheckMissingTokens(child, currentFileName, currentDocument);
					prevNode = child;
					prevNodeEnd = child.EndLocation;
				}
				CheckWhitespace(prevNode, prevNodeEnd, node, node.EndLocation, currentFileName, currentDocument);
			}
		}
		
		static bool IsLeafNode(AstNode node)
		{

			if (node.NodeType == NodeType.Token)
				return true;
			if (node.NodeType == NodeType.Whitespace)
				return !(node is PragmaWarningPreprocessorDirective);
			return node is PrimitiveType || node is PrimitiveExpression || node is NullReferenceExpression;
		}
		
		static void CheckWhitespace(AstNode startNode, TextLocation whitespaceStart, AstNode endNode, TextLocation whitespaceEnd, string currentFileName, IDocument currentDocument)
		{
			if (whitespaceStart == whitespaceEnd || startNode == endNode)
				return;
			Assert.Greater(whitespaceStart.Line, 0);
			Assert.Greater(whitespaceStart.Column, 0);
			Assert.Greater(whitespaceEnd.Line, 0);
			Assert.Greater(whitespaceEnd.Column, 0);
			Assert.IsTrue(whitespaceEnd >= whitespaceStart, endNode.GetType().Name + ".StartLocation < " + startNode.GetType().Name + ".EndLocation: " + whitespaceEnd + " < " + whitespaceStart);
			int start = currentDocument.GetOffset(whitespaceStart.Line, whitespaceStart.Column);
			int end = currentDocument.GetOffset(whitespaceEnd.Line, whitespaceEnd.Column);
			string text = currentDocument.GetText(start, end - start);
			bool assertion = string.IsNullOrWhiteSpace(text);
			if (!assertion) {
				if (startNode.Parent != endNode.Parent)
					PrintNode (startNode.Parent);
				PrintNode (endNode.Parent);
			}
			Assert.IsTrue(assertion, "Expected whitespace between " + startNode.GetType () +":" + whitespaceStart + " and " + endNode.GetType () + ":" + whitespaceEnd
			              + ", but got '" + text + "' (in " + currentFileName + " parent:" + startNode.Parent.GetType () +")");
		}
	}
}
