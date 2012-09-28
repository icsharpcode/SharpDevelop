/*
 * Created by SharpDevelop.
 * User: Daniel
 * Date: 9/22/2012
 * Time: 12:42 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Refactoring;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop;

namespace PortSD4AddInToSD5
{
	[IssueDescription ("Usage of SD4 WorkbenchSingleton",
	                   Description = "Usage of SD4 WorkbenchSingleton",
	                   Category = IssueCategories.Notifications,
	                   Severity = Severity.Warning,
	                   IssueMarker = IssueMarker.Underline)]
	public class WorkbenchSingletonIssueProvider : ICodeIssueProvider
	{
		public IEnumerable<CodeIssue> GetIssues (BaseRefactoringContext context)
		{
			foreach (var invocationExpression in context.RootNode.Descendants.OfType<InvocationExpression>()) {
				var rr = context.Resolve(invocationExpression);
				var irr = rr as CSharpInvocationResolveResult;
				if (irr == null)
					continue;
				switch (irr.Member.FullName) {
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.AssertMainThread":
						yield return Issue(
							invocationExpression,
							script => {
								script.Replace(invocationExpression,
								               new IdentifierExpression("SD").Member("MainThread").Invoke("VerifyAccess"));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall":
						if (invocationExpression.Arguments.Count == 1) {
							// SD.MainThread.InvokeAsync(argument).FireAndForget();
							yield return Issue(
								invocationExpression,
								script => {
									script.Replace(invocationExpression,
									               new IdentifierExpression("SD").Member("MainThread")
									               .Invoke("InvokeAsync", invocationExpression.Arguments.Single().Clone())
									               .Invoke("FireAndForget"));
								});
						} else {
							yield return Issue(invocationExpression);
						}
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadCall":
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadFunction":
						if (invocationExpression.Arguments.Count == 1) {
							// SD.MainThread.InvokeIfRequired(argument);
							yield return Issue(
								invocationExpression,
								script => {
									script.Replace(invocationExpression.Target,
									               new IdentifierExpression("SD").Member("MainThread").Member("InvokeIfRequired"));
								});
						} else {
							yield return Issue(invocationExpression);
						}
						break;
				}
			}
		}
		
		CodeIssue Issue(AstNode node, Action<Script> fix = null)
		{
			return new CodeIssue("WorkbenchSingleton is obsolete", node.StartLocation, node.EndLocation,
			                     fix != null ? new CodeAction("Use SD5 API", fix) : null);
		}
	}
}
