// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ICSharpCode.AvalonEdit.Snippets;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Editor.AvalonEdit;

namespace ICSharpCode.AvalonEdit.AddIn.Snippets
{
	/// <summary>
	/// SnippetManager singleton.
	/// </summary>
	public sealed class SnippetManager
	{
		public static readonly SnippetManager Instance = new SnippetManager();
		readonly object lockObj = new object();
		static readonly List<CodeSnippetGroup> defaultSnippets = new List<CodeSnippetGroup> {
			new CodeSnippetGroup {
				Extensions = ".cs",
				Snippets = {
					new CodeSnippet {
						Name = "for",
						Description = "for loop",
						Text = "for (int ${counter=i} = 0; ${counter} < ${end}; ${counter}++) {\n\t${Selection}\n}"
					},
					new CodeSnippet {
						Name = "foreach",
						Description = "foreach loop",
						Text = "foreach (${var} ${element} in ${collection}) {\n\t${Selection}\n}"
					},
					new CodeSnippet {
						Name = "if",
						Description = "if statement",
						Text = "if (${condition}) {\n\t${Selection}\n}"
					},
					new CodeSnippet {
						Name = "ifelse",
						Description = "if-else statement",
						Text = "if (${condition}) {\n\t${Selection}\n} else {\n\t${Caret}\n}"
					},
					new CodeSnippet {
						Name = "while",
						Description = "while loop",
						Text = "while (${condition}) {\n\t${Selection}\n}"
					},
					new CodeSnippet {
						Name = "prop",
						Description = "Property",
						Text = "public ${Type=object} ${Property=Property} { get; set; }${Caret}"
					},
					new CodeSnippet {
						Name = "propg",
						Description = "Property with private setter",
						Text = "public ${Type=object} ${Property=Property} { get; private set; }${Caret}"
					},
					new CodeSnippet {
						Name = "propfull",
						Description = "Property with backing field",
						Text = "${type} ${toFieldName(name)};\n\npublic ${type=int} ${name=Property} {\n\tget { return ${toFieldName(name)}; }\n\tset { ${toFieldName(name)} = value; }\n}${Caret}"
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
							+ Environment.NewLine + "}${Caret}"
					},
					new CodeSnippet {
						Name = "ctor",
						Description = "Constructor",
						Text = "${refactoring:ctor}"
					},
					new CodeSnippet {
						Name = "switch",
						Description = "Switch statement",
						// dynamic switch snippet (inserts switch body dependent on condition)
						Text = "switch (${condition}) {\n\t${refactoring:switchbody}\n}"
					},
					new CodeSnippet {
						Name = "try",
						Description = "Try-catch statement",
						Text = "try {\n\t${Selection}\n} catch (Exception) {\n\t${Caret}\n\tthrow;\n}"
					},
					new CodeSnippet {
						Name = "trycf",
						Description = "Try-catch-finally statement",
						Text = "try {\n\t${Selection}\n} catch (Exception) {\n\t${Caret}\n\tthrow;\n} finally {\n\t\n}"
					},
					new CodeSnippet {
						Name = "tryf",
						Description = "Try-finally statement",
						Text = "try {\n\t${Selection}\n} finally {\n\t${Caret}\n}"
					},
				}
			}
		};
		
		readonly List<ISnippetElementProvider> snippetElementProviders;
		
		public List<ISnippetElementProvider> SnippetElementProviders {
			get { return snippetElementProviders; }
		}
		
		private SnippetManager()
		{
			snippetElementProviders = AddInTree.BuildItems<ISnippetElementProvider>("/SharpDevelop/ViewContent/AvalonEdit/SnippetElementProviders", null, false);
		}
		
		/// <summary>
		/// Loads copies of all code snippet groups.
		/// </summary>
		public List<CodeSnippetGroup> LoadGroups()
		{
			return PropertyService.Get("CodeSnippets", defaultSnippets);
		}
		
		/// <summary>
		/// Saves the set of groups.
		/// </summary>
		public void SaveGroups(IEnumerable<CodeSnippetGroup> groups)
		{
			lock (lockObj) {
				activeGroups = null;
				PropertyService.Set("CodeSnippets", groups.ToList());
			}
		}
		
		ReadOnlyCollection<CodeSnippetGroup> activeGroups;
		
		public ReadOnlyCollection<CodeSnippetGroup> ActiveGroups {
			get {
				lock (lockObj) {
					if (activeGroups == null)
						activeGroups = LoadGroups().AsReadOnly();
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
