// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// SnippetManager singleton.
	/// </summary>
	public sealed class SnippetManager
	{
		readonly object lockObj = new object();
		internal readonly List<CodeSnippetGroup> defaultSnippets = new List<CodeSnippetGroup> {
			new CodeSnippetGroup {
				Extensions = ".cs",
				Snippets = {
					new CodeSnippet {
						Name = "for",
						Description = "for loop",
						Text = "for (int ${counter=i} = 0; ${counter} < ${end}; ${counter}++) {\n\t${Selection}\n}",
						Keyword = "for"
					},
					new CodeSnippet {
						Name = "foreach",
						Description = "foreach loop",
						Text = "foreach (${var} ${element} in ${collection}) {\n\t${Selection}\n}",
						Keyword = "foreach"
					},
					/*new CodeSnippet {
						Name = "ff",
						Description = "foreach loop",
						Text = "foreach (var ${toElementName(items)} in ${items}) {\n\t${Selection}\n}",
						Keyword = "foreach"
					},*/
					new CodeSnippet {
						Name = "if",
						Description = "if statement",
						Text = "if (${condition}) {\n\t${Selection}\n}",
						Keyword = "if"
					},
					new CodeSnippet {
						Name = "ifelse",
						Description = "if-else statement",
						Text = "if (${condition}) {\n\t${Selection}\n} else {\n\t${Caret}\n}",
						Keyword = "if"
					},
					new CodeSnippet {
						Name = "while",
						Description = "while loop",
						Text = "while (${condition}) {\n\t${Selection}\n}",
						Keyword = "while"
					},
					new CodeSnippet {
						Name = "prop",
						Description = "Property",
						Text = "public ${Type=object} ${Property=Property} { get; set; }${Caret}",
						Keyword = "event" // properties can be declared where events can be.
					},
					new CodeSnippet {
						Name = "propg",
						Description = "Property with private setter",
						Text = "public ${Type=object} ${Property=Property} { get; private set; }${Caret}",
						Keyword = "event"
					},
					new CodeSnippet {
						Name = "propfull",
						Description = "Property with backing field",
						Text = "${type} ${toFieldName(name)};\n\npublic ${type=int} ${name=Property} {\n\tget { return ${toFieldName(name)}; }\n\tset { ${toFieldName(name)} = value; }\n}${Caret}",
						Keyword = "event"
					},
					new CodeSnippet {
						Name = "propall",
						Description = "Allows to implement properties for all fields in the class",
						Text = "${refactoring:propall}${Caret}",
						Keyword = "event"
					},
					new CodeSnippet {
						Name = "propdp",
						Description = "Dependency Property",
						Text = "public static readonly DependencyProperty ${name}Property =" + Environment.NewLine
							+ "\tDependencyProperty.Register(\"${name}\", typeof(${type}), typeof(${ClassName})," + Environment.NewLine
							+ "\t                            new FrameworkPropertyMetadata());" + Environment.NewLine
							+ "" + Environment.NewLine
							+ "public ${type=int} ${name=Property} {" + Environment.NewLine
							+ "\tget { return (${type})GetValue(${name}Property); }" + Environment.NewLine
							+ "\tset { SetValue(${name}Property, value); }"
							+ Environment.NewLine + "}${Caret}",
						Keyword = "event"
					},
					new CodeSnippet {
						Name = "ctor",
						Description = "Constructor",
						Text = "public ${ClassName}(${anchor:parameterList})\n{\n\t${refactoring:ctor}\n}",
						Keyword = "event"
					},
					new CodeSnippet {
						Name = "switch",
						Description = "Switch statement",
						// dynamic switch snippet (inserts switch body dependent on condition)
						Text = "switch (${condition}) {\n\t${refactoring:switchbody}\n}",
						Keyword = "switch"
					},
					new CodeSnippet {
						Name = "try",
						Description = "Try-catch statement",
						Text = "try {\n\t${Selection}\n} catch (Exception) {\n\t${Caret}\n\tthrow;\n}",
						Keyword = "try"
					},
					new CodeSnippet {
						Name = "trycf",
						Description = "Try-catch-finally statement",
						Text = "try {\n\t${Selection}\n} catch (Exception) {\n\t${Caret}\n\tthrow;\n} finally {\n\t\n}",
						Keyword = "try"
					},
					new CodeSnippet {
						Name = "tryf",
						Description = "Try-finally statement",
						Text = "try {\n\t${Selection}\n} finally {\n\t${Caret}\n}",
						Keyword = "try"
					},
					new CodeSnippet {
						Name = "using",
						Description = "Using statement",
						Text = "using (${resource=null}) {\n\t${Selection}\n}",
						Keyword = "try" // using is not a good keyword, because it is usable outside of method bodies as well.
					},
				}
			},
			new CodeSnippetGroup {
				Extensions = ".vb",
				Snippets = {
					new CodeSnippet {
						Name = "If",
						Description = "If statement",
						Text = "If ${condition} Then\n" +
							"\t${Selection}\n" +
							"End If",
						Keyword = "If"
					},
					new CodeSnippet {
						Name = "IfElse",
						Description = "If-Else statement",
						Text = "If ${condition} Then\n" +
							"\t${Selection}\n" +
							"Else\n" +
							"\t${Caret}\n" +
							"End If",
						Keyword = "If"
					},
					new CodeSnippet {
						Name = "For",
						Description = "For loop",
						Text = "For ${counter=i} As ${type=Integer} = ${start=0} To ${end}\n" +
							"\t${Selection}\n" +
							"Next ${counter}",
						Keyword = "For"
					},
					new CodeSnippet {
						Name = "ForStep",
						Description = "For loop with Step",
						Text = "For ${counter=i} As ${type=Integer} = ${start=0} To ${end} Step ${step=1}\n" +
							"\t${Selection}\n" +
							"Next ${counter}",
						Keyword = "For"
					},
					new CodeSnippet {
						Name = "DoLoopUn",
						Description = "Do ... Loop Until statement",
						Text = "Do\n" +
							"\t${Selection}\n" +
							"Loop Until ${expression}",
						Keyword = "Do"
					},
					new CodeSnippet {
						Name = "DoLoopWh",
						Description = "Do ... Loop While statement",
						Text = "Do\n" +
							"\t${Selection}\n" +
							"Loop While ${expression}",
						Keyword = "Do"
					},
					new CodeSnippet {
						Name = "DoWhile",
						Description = "Do While ... Loop statement",
						Text = "Do While ${expression}\n" +
							"\t${Selection}\n" +
							"Loop",
						Keyword = "Do"
					},
					new CodeSnippet {
						Name = "DoUntil",
						Description = "Do Until ... Loop statement",
						Text = "Do Until ${expression}\n" +
							"\t${Selection}\n" +
							"Loop",
						Keyword = "Do"
					},
					new CodeSnippet {
						Name = "ForEach",
						Description = "For Each statement",
						Text = "For Each ${item} As ${type} In ${collection}\n" +
							"\t${Selection}\n" +
							"Next",
						Keyword = "For"
					},
					new CodeSnippet {
						Name = "IfElseIf",
						Description = "If ... ElseIf ... End If statement",
						Text = @"If ${condition1} Then
	${Selection}
ElseIf ${condition2} Then
	${Caret}
Else

End If",
						Keyword = "If"
					},
					new CodeSnippet {
						Name = "While",
						Description = "While statement",
						Text = @"While ${condition}
	${Selection}
End While",
						Keyword = "While"
					},
					new CodeSnippet {
						Name = "ctor",
						Description = "Constructor",
						Text = @"Public Sub New(${anchor:parameterList})
	${refactoring:ctor}
End Sub",
						Keyword = "Sub"
					},
					new CodeSnippet {
						Name = "Select",
						Description = "Select statement",
						Text = @"Select Case ${variable}
    Case ${case1}
		${Selection}
    Case Else
		${Caret}
End Select",
						Keyword = "Select"
					},
					new CodeSnippet {
						Name = "Try",
						Description = "Try-catch statement",
						Text = "Try\n\t${Selection}\nCatch ${var=ex} As ${Exception=Exception}\n\t${Caret}\n\tThrow\nEnd Try",
						Keyword = "Try"
					},
					new CodeSnippet {
						Name = "TryCF",
						Description = "Try-catch-finally statement",
						Text = "Try\n\t${Selection}\nCatch ${var=ex} As ${Exception=Exception}\n\t${Caret}\n\tThrow\nFinally\n\t\nEnd Try",
						Keyword = "Try"
					},
					new CodeSnippet {
						Name = "TryF",
						Description = "Try-finally statement",
						Text = "Try\n\t${Selection}\nFinally\n\t${Caret}\nEnd Try",
						Keyword = "Try"
					},
					new CodeSnippet {
						Name = "Using",
						Description = "Using statement",
						Text = @"Using ${var=obj} As ${type}
	${Selection}
End Using",
						Keyword = "Using"
					},
					new CodeSnippet {
						Name = "propfull",
						Description = "Property",
						Text = @"Private ${toFieldName(name)} As ${type}
Public Property ${name=Property} As ${type=Integer}
	Get
		Return ${toFieldName(name)}
	End Get
	Set(${value=value} As ${type})
		${toFieldName(name)} = ${value}
	End Set
End Property${Caret}",
						Keyword = "Property"
					},
				}
			}
		};
		
		public static readonly SnippetManager Instance = new SnippetManager();
		
		readonly List<ISnippetElementProvider> snippetElementProviders;
		
		public List<ISnippetElementProvider> SnippetElementProviders {
			get { return snippetElementProviders; }
		}
		
		private SnippetManager()
		{
			foreach (var g in defaultSnippets)
				g.Freeze();
			snippetElementProviders = AddInTree.BuildItems<ISnippetElementProvider>("/SharpDevelop/ViewContent/AvalonEdit/SnippetElementProviders", null, false);
		}
		
		/// <summary>
		/// Loads copies of all code snippet groups.
		/// </summary>
		public List<CodeSnippetGroup> LoadGroups()
		{
			var savedSnippets = PropertyService.Get("CodeSnippets", new List<CodeSnippetGroup>());
			
			// HACK: clone all groups to ensure we use instances independent from the PropertyService
			// this can be removed in SD5 where PropertyService.Get deserializes a new instance on every call.
			savedSnippets = savedSnippets.Select(g => new CodeSnippetGroup(g)).ToList();
			
			foreach (var group in savedSnippets) {
				var defaultGroup = defaultSnippets.FirstOrDefault(i => i.Extensions == group.Extensions);
				if (defaultGroup != null) {
					var merged = group.Snippets.Concat(
						defaultGroup.Snippets.Except(
							group.Snippets,
							new ByMemberComparer<CodeSnippet, string>(s => s.Name)
						).Select(s => new CodeSnippet(s)) // clone snippets so that defaultGroup is not modified
					).OrderBy(s => s.Name).ToList();
					group.Snippets.Clear();
					group.Snippets.AddRange(merged);
				}
			}
			
			foreach (var group in defaultSnippets.Except(savedSnippets, new ByMemberComparer<CodeSnippetGroup, string>(g => g.Extensions))) {
				savedSnippets.Add(new CodeSnippetGroup(group));
			}
			
			return savedSnippets;
		}
		
		sealed class ByMemberComparer<TObject, TMember> : IEqualityComparer<TObject>
		{
			readonly Func<TObject, TMember> selector;
			readonly IEqualityComparer<TMember> memberComparer = EqualityComparer<TMember>.Default;
			
			public ByMemberComparer(Func<TObject, TMember> selector)
			{
				this.selector = selector;
			}
			
			public bool Equals(TObject x, TObject y)
			{
				return memberComparer.Equals(selector(x), selector(y));
			}
			
			public int GetHashCode(TObject obj)
			{
				return memberComparer.GetHashCode(selector(obj));
			}
		}
		
		/// <summary>
		/// Saves the set of groups.
		/// </summary>
		public void SaveGroups(IEnumerable<CodeSnippetGroup> groups)
		{
			lock (lockObj) {
				activeGroups = null;
				List<CodeSnippetGroup> modifiedGroups = new List<CodeSnippetGroup>();
				
				foreach (var group in groups) {
					var defaultGroup = defaultSnippets.FirstOrDefault(i => i.Extensions == group.Extensions);
					
					IEnumerable<CodeSnippet> saveSnippets = group.Snippets;
					
					if (defaultGroup != null) {
						saveSnippets = group.Snippets.Except(defaultGroup.Snippets, CodeSnippetComparer.Instance);
					}
					
					// save all groups, even if they're empty
					var copy = new CodeSnippetGroup() {
						Extensions = group.Extensions
					};
					copy.Snippets.AddRange(saveSnippets);
					modifiedGroups.Add(copy);
				}
				
				PropertyService.Set("CodeSnippets", modifiedGroups);
			}
		}
		
		ReadOnlyCollection<CodeSnippetGroup> activeGroups;
		
		public ReadOnlyCollection<CodeSnippetGroup> ActiveGroups {
			get {
				lock (lockObj) {
					if (activeGroups == null) {
						activeGroups = LoadGroups().AsReadOnly();
						foreach (var g in activeGroups)
							g.Freeze();
					}
					return activeGroups;
				}
			}
		}
		
		public CodeSnippetGroup FindGroup(string extension)
		{
			foreach (CodeSnippetGroup g in ActiveGroups) {
				string[] extensions = g.Extensions.Split(';');
				foreach (string gext in extensions) {
					if (gext.Equals(extension, StringComparison.OrdinalIgnoreCase))
						return g;
				}
			}
			return null;
		}
		
		public CodeSnippet FindSnippet(string extension, string name)
		{
			CodeSnippetGroup g = FindGroup(extension);
			if (g != null) {
				return g.Snippets.FirstOrDefault(s => s.Name == name);
			}
			return null;
		}
	}
}
