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
using ICSharpCode.NRefactory.PatternMatching;
using ICSharpCode.NRefactory.Refactoring;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.SharpDevelop;

namespace PortSD4AddInToSD5
{
	[IssueDescription ("Usage of SD4 WorkbenchSingleton",
	                   Description = "Usage of SD4 WorkbenchSingleton",
	                   Category = "SD4->SD5",
	                   Severity = Severity.Warning)]
	public class WorkbenchSingletonIssueProvider : CodeIssueProvider
	{
		public override IEnumerable<CodeIssue> GetIssues(BaseRefactoringContext context, string subIssue = null)
		{
			foreach (var mre in context.RootNode.Descendants.OfType<MemberReferenceExpression>()) {
				var rr = context.Resolve(mre);
				var mrr = rr as MemberResolveResult;
				if (mrr == null)
					continue;
				switch (mrr.Member.FullName) {
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWin32Window":
						yield return Issue(
							mre,
							script => {
								script.Replace(mre, new IdentifierExpression("SD").Member("WinForms").Member("MainWin32Window"));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.Workbench":
						yield return Issue(
							mre,
							script => {
								script.Replace(mre, new IdentifierExpression("SD").Member("Workbench"));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.StatusBar":
						yield return Issue(
							mre,
							script => {
								script.Replace(mre, new IdentifierExpression("SD").Member("StatusBar"));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.InvokeRequired":
						yield return Issue(
							mre,
							script => {
								script.Replace(mre, new IdentifierExpression("SD").Member("MainThread").Member("InvokeRequired"));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainWindow":
						yield return Issue(
							mre,
							script => {
								script.Replace(mre, new IdentifierExpression("SD").Member("Workbench").Member("MainWindow"));
							});
						break;
				}
			}
			foreach (var invocationExpression in context.RootNode.Descendants.OfType<InvocationExpression>()) {
				var rr = context.Resolve(invocationExpression);
				var irr = rr as CSharpInvocationResolveResult;
				if (irr == null)
					continue;
				switch (irr.Member.FullName) {
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.CallLater":
						yield return Issue(
							invocationExpression,
							script => {
								script.Replace(invocationExpression,
								               new IdentifierExpression("SD").Member("MainThread").Invoke("CallLater", invocationExpression.Arguments.Select(e => e.Clone())));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.AssertMainThread":
						yield return Issue(
							invocationExpression,
							script => {
								script.Replace(invocationExpression,
								               new IdentifierExpression("SD").Member("MainThread").Invoke("VerifyAccess"));
							});
						break;
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadAsyncCall":
						{
							// SD.MainThread.InvokeAsync(argument).FireAndForget();
							Expression arg = invocationExpression.Arguments.First().Clone();
							if (invocationExpression.Arguments.Count > 1)
								arg = new LambdaExpression { Body = arg.Invoke(invocationExpression.Arguments.Skip(1).Select(a => a.Clone())) };
							yield return Issue(
								invocationExpression,
								script => {
									script.Replace(invocationExpression,
									               new IdentifierExpression("SD").Member("MainThread")
									               .Invoke("InvokeAsyncAndForget", arg));
								});
							break;
						}
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadCall":
					case "ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.SafeThreadFunction":
						{
							// SD.MainThread.InvokeIfRequired(argument);
							Expression arg = invocationExpression.Arguments.First().Clone();
							if (invocationExpression.Arguments.Count > 1)
								arg = new LambdaExpression { Body = arg.Invoke(invocationExpression.Arguments.Skip(1).Select(a => a.Clone())) };
							yield return Issue(
								invocationExpression,
								script => {
									script.Replace(invocationExpression,
									               new IdentifierExpression("SD").Member("MainThread").Invoke("InvokeIfRequired", arg));
								});
							break;
						}
					case "ICSharpCode.SharpDevelop.IMessageLoop.InvokeAsync":
						// We used to recommend SD.MainThread.InvokeAsync(...).FireAndForget(),
						// but it's better to use SD.MainThread.InvokeAsyncAndForget(...)
						if (invocationExpression.Clone().Invoke("FireAndForget").IsMatch(invocationExpression.Parent.Parent)) {
							var ident = invocationExpression.Parent.GetChildByRole(Roles.Identifier);
							yield return new CodeIssue(
								ident.StartLocation, ident.EndLocation,
								"Use InvokeAsyncAndForget() instead",
								new CodeAction("Use InvokeAsyncAndForget() instead",
								               script => {
								               	var newInvocation = (InvocationExpression)invocationExpression.Clone();
								               	((MemberReferenceExpression)newInvocation.Target).MemberName = "InvokeAsyncAndForget";
								               	script.Replace(invocationExpression.Parent.Parent, newInvocation);
								               },
								               ident));
						}
						break;
				}
			}
		}
		
		CodeIssue Issue(AstNode node, Action<Script> fix = null)
		{
			return new CodeIssue(node.StartLocation, node.EndLocation, "WorkbenchSingleton is obsolete",
			                     fix != null ? new CodeAction("Use SD5 API", fix, node) : null);
		}
	}
}
