// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Codons;
using ICSharpCode.TextEditor.Document;
using NUnit.Framework;

namespace PythonBinding.Tests
{
	/// <summary>
	/// Basic PythonBinding.addin file tests.
	/// </summary>
	[TestFixture]
	public class AddInFileTestFixture
	{
		AddIn addin;
		Codon fileFilterCodon;
		Codon pythonMenuCodon;
		Codon pythonRunMenuItemCodon;
		Codon pythonStopMenuItemCodon;
		Codon fileTemplatesCodon;
		Codon optionsPanelCodon;
		Codon parserCodon;
		Runtime pythonBindingRuntime;
		Codon additionalMSBuildPropertiesCodon;
		Codon languageBindingCodon;
		Codon projectFileFilterCodon;
		Codon codeCompletionBindingCodon;
		LazyConditionEvaluator isPythonRunningConditionEvaluator;
		Codon applicationSettingsOptionsCodon;
		Codon buildEventsCodon;
		Codon compilingOptionsCodon;
		Codon debugOptionsCodon;
		AddInReference formsDesignerAddInRef;
		Runtime formsDesignerRuntime;
		Codon displayBindingCodon;
		Codon convertCodeCodon;
		Codon pythonFileIconCodon;
		Codon pythonProjectIconCodon;
		Codon convertCSharpProjectCodon;
		Codon convertVBNetProjectCodon;
		
		[TestFixtureSetUp]
		public void SetupFixture()
		{
			using (TextReader reader = PythonBindingAddInFile.ReadAddInFile()) {
				addin = AddIn.Load(reader, String.Empty);
				
				fileFilterCodon = GetCodon("/SharpDevelop/Workbench/FileFilter", "Python");
				pythonMenuCodon = GetCodon("/SharpDevelop/Workbench/MainMenu", "Python");
				displayBindingCodon = GetCodon("/SharpDevelop/Workbench/DisplayBindings", "PythonDisplayBinding");
				
				const string runMenuExtensionPath = "/SharpDevelop/Workbench/MainMenu/Python";
				pythonRunMenuItemCodon = GetCodon(runMenuExtensionPath, "Run");
				pythonStopMenuItemCodon = GetCodon(runMenuExtensionPath, "Stop");
				
				fileTemplatesCodon = GetCodon("/SharpDevelop/BackendBindings/Templates", "Python");
				optionsPanelCodon = GetCodon("/SharpDevelop/Dialogs/OptionsDialog/ToolsOptions", "PythonOptionsPanel");
				parserCodon = GetCodon("/Workspace/Parser", "Python");
				additionalMSBuildPropertiesCodon = GetCodon("/SharpDevelop/MSBuildEngine/AdditionalProperties", "PythonBinPath");
				languageBindingCodon = GetCodon("/SharpDevelop/Workbench/LanguageBindings", "Python");
				projectFileFilterCodon = GetCodon("/SharpDevelop/Workbench/Combine/FileFilter", "PythonProject");
				codeCompletionBindingCodon = GetCodon("/AddIns/DefaultTextEditor/CodeCompletion", "Python");
				applicationSettingsOptionsCodon = GetCodon("/SharpDevelop/BackendBindings/ProjectOptions/Python", "Application");
				buildEventsCodon = GetCodon("/SharpDevelop/BackendBindings/ProjectOptions/Python", "BuildEvents");
				compilingOptionsCodon = GetCodon("/SharpDevelop/BackendBindings/ProjectOptions/Python", "CompilingOptions");
				debugOptionsCodon = GetCodon("/SharpDevelop/BackendBindings/ProjectOptions/Python", "DebugOptions");
				convertCodeCodon = GetCodon("/SharpDevelop/Workbench/MainMenu/Tools/ConvertCode", "ConvertToPython");
				pythonFileIconCodon = GetCodon("/Workspace/Icons", "PythonFileIcon");
				pythonProjectIconCodon = GetCodon("/Workspace/Icons", "PythonProjectIcon");
				convertCSharpProjectCodon = GetCodon("/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ProjectActions/Convert", "CSharpProjectToPythonProjectConverter");
				convertVBNetProjectCodon = GetCodon("/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ProjectActions/Convert", "VBNetProjectToPythonProjectConverter");
					
				// Get the PythonBinding runtime.
				foreach (Runtime runtime in addin.Runtimes) {
					if (runtime.Assembly == "PythonBinding.dll") {
						pythonBindingRuntime = runtime;
						
						foreach (LazyConditionEvaluator conditionEvaluator in runtime.DefinedConditionEvaluators) {
							if (conditionEvaluator.Name == "IsPythonRunning") {
								isPythonRunningConditionEvaluator = conditionEvaluator;
							}
						}
					} else if (runtime.Assembly == "$ICSharpCode.FormsDesigner/FormsDesigner.dll") {
						formsDesignerRuntime = runtime;
					}
				}
				
				// Get the forms designer dependency.
				foreach (AddInReference addInRef in addin.Manifest.Dependencies) {
					if (addInRef.Name == "ICSharpCode.FormsDesigner") {
						formsDesignerAddInRef = addInRef;
						break;
					}
				}
			}
		}
		
		[Test]
		public void AddInName()
		{
			Assert.AreEqual(addin.Name, "Python Binding");
		}
		
		[Test]
		public void ManifestId()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding", addin.Manifest.PrimaryIdentity);
		}
		
		[Test]
		public void FormsDesignerAddInIsDependency()
		{
			Assert.IsNotNull(formsDesignerAddInRef);
		}
		
		[Test]
		public void FormsDesignerRuntimeExists()
		{
			Assert.IsNotNull(formsDesignerRuntime);
		}
		
		[Test]
		public void RuntimeExists()
		{
			Assert.IsNotNull(pythonBindingRuntime);
		}
		
		[Test]
		public void IsPythonRunningConditionExists()
		{
			Assert.IsNotNull(isPythonRunningConditionEvaluator);
		}
		
		[Test]
		public void IsPythonRunningConditionClassName()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.IsPythonRunningCondition", isPythonRunningConditionEvaluator.ClassName);
		}
		
		[Test]
		public void FileFilterExists()
		{
			Assert.IsNotNull(fileFilterCodon);
		}
		
		[Test]
		public void FileFilterExtension()
		{
			Assert.AreEqual("*.py", fileFilterCodon["extensions"]);
		}
		
		[Test]
		public void FileFilterDescription()
		{
			Assert.AreEqual("Python Files (*.py)", fileFilterCodon["name"]);
		}
		
		[Test]
		public void FileFilterInsertBefore()
		{
			Assert.AreEqual("Resources", fileFilterCodon.InsertBefore);
		}
		
		[Test]
		public void FileFilterInsertAfter()
		{
			Assert.AreEqual("Icons", fileFilterCodon.InsertAfter);
		}
		
		[Test]
		public void PythonMenuName()
		{
			Assert.AreEqual("&Python", pythonMenuCodon["label"]);
		}
		
		[Test]
		public void PythonMenuCodon()
		{
			Assert.AreEqual("MenuItem", pythonMenuCodon.Name);
		}
		
		[Test]
		public void PythonMenuCodonType()
		{
			Assert.AreEqual("Menu", pythonMenuCodon["type"]);
		}
		
		[Test]
		public void PythonMenuInsertBefore()
		{
			Assert.AreEqual("Tools", pythonMenuCodon.InsertBefore);
		}
		
		[Test]
		public void PythonMenuInsertAfter()
		{
			Assert.AreEqual("Search", pythonMenuCodon.InsertAfter);
		}		
		
		[Test]
		public void PythonMenuConditionExists()
		{
			Assert.AreEqual(1, pythonMenuCodon.Conditions.Length);
		}
		
		[Test]
		public void PythonMenuConditionName()
		{
			ICondition condition = pythonMenuCodon.Conditions[0];
			Assert.AreEqual("ActiveContentExtension", condition.Name);
		}
		
		[Test]
		public void PythonMenuConditionActiveExtension()
		{
			Condition condition = pythonMenuCodon.Conditions[0] as Condition;
			Assert.AreEqual(".py", condition["activeextension"]);
		}
		
		[Test]
		public void PythonStopMenuItemLabel()
		{
			Assert.AreEqual("&Stop", pythonStopMenuItemCodon["label"]);
		}
		
		[Test]
		public void PythonStopMenuItemIsMenuItem()
		{
			Assert.AreEqual("MenuItem", pythonStopMenuItemCodon.Name);
		}		
		
		[Test]
		public void PythonStopMenuItemClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.StopPythonCommand", pythonStopMenuItemCodon["class"]);
		}
		
		[Test]
		public void PythonStopMenuItemIcon()
		{
			Assert.AreEqual("Icons.16x16.Debug.StopProcess", pythonStopMenuItemCodon["icon"]);
		}		
		
		[Test]
		public void PythonRunMenuItemLabel()
		{
			Assert.AreEqual("&Run", pythonRunMenuItemCodon["label"]);
		}
		
		[Test]
		public void PythonRunMenuItemIsMenuItem()
		{
			Assert.AreEqual("MenuItem", pythonRunMenuItemCodon.Name);
		}
		
		[Test]
		public void PythonRunMenuItemClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.RunPythonCommand", pythonRunMenuItemCodon["class"]);
		}
		
		[Test]
		public void PythonRunMenuItemShortcut()
		{
			Assert.AreEqual("Control|Shift|R", pythonRunMenuItemCodon["shortcut"]);
		}
		
		[Test]
		public void PythonRunMenuItemIcon()
		{
			Assert.AreEqual("Icons.16x16.RunProgramIcon", pythonRunMenuItemCodon["icon"]);
		}
		
		[Test]
		public void PythonRunMenuHasSingleCondition()
		{
			Assert.AreEqual(1, pythonRunMenuItemCodon.Conditions.Length);
		}
		
		[Test]
		public void PythonRunMenuNotConditionExists()
		{
			NegatedCondition notCondition = pythonRunMenuItemCodon.Conditions[0] as NegatedCondition;
			Assert.IsNotNull(notCondition);
		}
		
		[Test]
		public void PythonRunMenuNotCondition()
		{
			NegatedCondition notCondition = pythonRunMenuItemCodon.Conditions[0] as NegatedCondition;
			
			// Use reflection to get the ICondition associated with the not 
			// condition.
			Type type = notCondition.GetType();
			FieldInfo fieldInfo = type.GetField("condition", BindingFlags.NonPublic | BindingFlags.Instance);
			ICondition condition = fieldInfo.GetValue(notCondition) as ICondition;
			
			Assert.AreEqual("IsPythonRunning", condition.Name);
		}
		
		[Test]
		public void PythonRunMenuConditionAction()
		{
			ICondition condition = pythonRunMenuItemCodon.Conditions[0];
			Assert.AreEqual(ConditionFailedAction.Disable, condition.Action);
		}

		[Test]
		public void PythonStopMenuConditionAction()
		{
			ICondition condition = pythonStopMenuItemCodon.Conditions[0];
			Assert.AreEqual(ConditionFailedAction.Disable, condition.Action);
		}
		
		[Test]
		public void PythonStopMenuConditionName()
		{
			ICondition condition = pythonStopMenuItemCodon.Conditions[0];
			Assert.AreEqual("IsPythonRunning", condition.Name);
		}
		
		[Test]
		public void FileTemplatesCodonType()
		{
			Assert.AreEqual("Directory", fileTemplatesCodon.Name);
		}
		
		[Test]
		public void FileTemplatesFolder()
		{
			Assert.AreEqual("./Templates", fileTemplatesCodon["path"]);
		}
		
		[Test]
		public void OptionsPanelCodonIsDialogPanel()
		{
			Assert.AreEqual("DialogPanel", optionsPanelCodon.Name);
		}
		
		[Test]
		public void OptionsPanelLabel()
		{
			Assert.AreEqual("Python", optionsPanelCodon["label"]);
		}
		
		[Test]
		public void OptionsPanelClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.PythonOptionsPanel", optionsPanelCodon["class"]);
		}		
		
		[Test]
		public void SupportedParserExtensions()
		{
			Assert.AreEqual(".py", parserCodon["supportedextensions"]);
		}
		
		[Test]
		public void ParserClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.PythonParser", parserCodon["class"]);
		}

		[Test]
		public void ParserProjectFileExtensions()
		{
			Assert.AreEqual(".pyproj", parserCodon["projectfileextension"]);
		}
		
		[Test]
		public void PythonBinPath()
		{
			Assert.AreEqual("${AddInPath:ICSharpCode.PythonBinding}", additionalMSBuildPropertiesCodon["text"]);
		}
		
		[Test]
		public void LanguageBindingSupportedExtensions()
		{
			Assert.AreEqual(".py", languageBindingCodon["supportedextensions"]);
		}
		
		[Test]
		public void LanguageBindingGuid()
		{
			Assert.AreEqual("{FD48973F-F585-4F70-812B-4D0503B36CE9}", languageBindingCodon["guid"]);
		}
		
		[Test]
		public void LanguageBindingProjectFileExtension()
		{
			Assert.AreEqual(".pyproj", languageBindingCodon["projectfileextension"]);
		}
		
		[Test]
		public void LanguageBindingClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.PythonLanguageBinding", languageBindingCodon["class"]);
		}
		
		[Test]
		public void ProjectFileFilterExtensions()
		{
			Assert.AreEqual("*.pyproj", projectFileFilterCodon["extensions"]);
		}
		
		[Test]
		public void ProjectFileFilterClass()
		{
			Assert.AreEqual("ICSharpCode.SharpDevelop.Project.LoadProject", projectFileFilterCodon["class"]);
		}
		
		[Test]
		public void ProjectFileFilterName()
		{
			Assert.AreEqual("Python Project Files (*.pyproj)", projectFileFilterCodon["name"]);
		}
		
		[Test]
		public void ProjectFileFilterInsertBefore()
		{
			Assert.AreEqual("AllFiles", projectFileFilterCodon.InsertBefore);
		}
				
		[Test]
		public void CodeCompletionBindingCodonName()
		{
			Assert.AreEqual("CodeCompletionBinding", codeCompletionBindingCodon.Name);
		}
		
		[Test]
		public void CodeCompletionBindingFileExtensions()
		{
			Assert.AreEqual(".py", codeCompletionBindingCodon["extensions"]);
		}
		
		[Test]
		public void CodeCompletionBindingClassName()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.PythonCodeCompletionBinding", codeCompletionBindingCodon["class"]);
		}
		
		[Test]
		public void ApplicationSettingsCodonClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.ApplicationSettingsPanel", applicationSettingsOptionsCodon["class"]);
		}
		
		[Test]
		public void ApplicationSettingsCodonLabel()
		{
			Assert.AreEqual("${res:Dialog.ProjectOptions.ApplicationSettings}", applicationSettingsOptionsCodon["label"]);
		}
		
		[Test]
		public void BuildEventsCodonClass()
		{
			Assert.AreEqual("ICSharpCode.SharpDevelop.Gui.OptionPanels.BuildEvents", buildEventsCodon["class"]);
		}
		
		[Test]
		public void BuildEventsCodonLabel()
		{
			Assert.AreEqual("${res:Dialog.ProjectOptions.BuildEvents}", buildEventsCodon["label"]);
		}		
		
		[Test]
		public void CompilingOptionsCodonClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.CompilingOptionsPanel", compilingOptionsCodon["class"]);
		}
		
		[Test]
		public void CompilingOptionsCodonLabel()
		{
			Assert.AreEqual("${res:Dialog.ProjectOptions.BuildOptions}", compilingOptionsCodon["label"]);
		}
		
		[Test]
		public void DebugOptionsCodonClass()
		{
			Assert.AreEqual("ICSharpCode.SharpDevelop.Gui.OptionPanels.DebugOptions", debugOptionsCodon["class"]);
		}
		
		[Test]
		public void DebugOptionsCodonLabel()
		{
			Assert.AreEqual("${res:Dialog.ProjectOptions.DebugOptions}", debugOptionsCodon["label"]);
		}
		
		[Test]
		public void PythonDisplayBindingExists()
		{
			Assert.IsNotNull(displayBindingCodon);
		}
		
		[Test]
		public void PythonDisplayBindingIsSecondary()
		{
			Assert.AreEqual("Secondary", displayBindingCodon["type"]);
		}
		
		[Test]
		public void PythonDisplayBindingFileNamePattern()
		{
			Assert.AreEqual(@"\.py$", displayBindingCodon["fileNamePattern"]);
		}
		
		[Test]
		public void PythonDisplayBindingLanguagePattern()
		{
			Assert.AreEqual(@"^Python$", displayBindingCodon["languagePattern"]);
		}
		
		[Test]
		public void PythonDisplayBindingClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.PythonFormsDesignerDisplayBinding", displayBindingCodon["class"]);
		}

		[Test]
		public void ConvertCodeCodonExists()
		{
			Assert.IsNotNull(convertCodeCodon);
		}
		
		[Test]
		public void ConvertCodeCodonIsMenuItem()
		{
			Assert.AreEqual("MenuItem", convertCodeCodon.Name);
		}
		
		[Test]
		public void ConvertCodeCodonInsertedAfterCSharp()
		{
			Assert.AreEqual("CSharp", convertCodeCodon.InsertAfter);
		}

		[Test]
		public void ConvertCodeCodonInsertedBeforeVBNet()
		{
			Assert.AreEqual("VBNet", convertCodeCodon.InsertBefore);
		}

		[Test]
		public void ConvertCodeCodonLabel()
		{
			Assert.AreEqual("Python", convertCodeCodon["label"]);
		}

		[Test]
		public void ConvertCodeCodonClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.ConvertToPythonMenuCommand", convertCodeCodon["class"]);
		}
		
		[Test]
		public void ConvertCodeMenuConditionAction()
		{
			ICondition condition = convertCodeCodon.Conditions[0];
			Assert.AreEqual(ConditionFailedAction.Disable, condition.Action);
		}		

		[Test]
		public void ConvertCodeMenuConditionName()
		{
			ICondition condition = convertCodeCodon.Conditions[0];
			Assert.AreEqual("ActiveContentExtension Or ActiveContentExtension", condition.Name);
		}

		[Test]
		public void ConvertCodeMenuConditionIsOrCondition()
		{
			OrCondition orCondition = convertCodeCodon.Conditions[0] as OrCondition;
			Assert.IsNotNull(orCondition);
		}
		
		[Test]
		public void ConvertCodeMenuConditionActiveExtension()
		{			
			OrCondition orCondition = convertCodeCodon.Conditions[0] as OrCondition;

			// Use reflection to get the ICondition associated with the not
			// condition.
			Type type = orCondition.GetType();
			FieldInfo fieldInfo = type.GetField("conditions", BindingFlags.NonPublic | BindingFlags.Instance);
			ICondition[] conditions = fieldInfo.GetValue(orCondition) as ICondition[];

			Condition csharpCondition = conditions[0] as Condition;
			Condition vbnetCondition = conditions[1] as Condition;
			
			Assert.AreEqual(2, conditions.Length);
			Assert.AreEqual(".cs", csharpCondition["activeextension"]);
			Assert.AreEqual(".vb", vbnetCondition["activeextension"]);
		}
		
		[Test]
		public void PythonFileIconCodonExists()
		{
			Assert.IsNotNull(pythonFileIconCodon);
		}

		[Test]
		public void PythonFileIconCodonExtensions()
		{
			Assert.AreEqual(".py", pythonFileIconCodon["extensions"]);
		}
		
		[Test]
		public void PythonFileIconCodonResource()
		{
			Assert.AreEqual("Python.ProjectBrowser.File", pythonFileIconCodon["resource"]);
		}
		
		[Test]
		public void PythonProjectIconCodonExists()
		{
			Assert.IsNotNull(pythonProjectIconCodon);
		}

		[Test]
		public void PythonProjectIconCodonLanguage()
		{
			Assert.AreEqual("Python", pythonProjectIconCodon["language"]);
		}
		
		[Test]
		public void PythonProjectIconCodonResource()
		{
			Assert.AreEqual("Python.ProjectBrowser.Project", pythonProjectIconCodon["resource"]);
		}
		
		[Test]
		public void ConvertToCSharpProjectCodonExists()
		{
			Assert.IsNotNull(convertCSharpProjectCodon);
		}

		[Test]
		public void ConvertToVBNetProjectCodonExists()
		{
			Assert.IsNotNull(convertVBNetProjectCodon);
		}
			
		[Test]
		public void ConvertToCSharpProjectLabel()
		{
			Assert.AreEqual("${res:ICSharpCode.PythonBinding.ConvertCSharpProjectToPythonProject}", convertCSharpProjectCodon["label"]);
		}

		[Test]
		public void ConvertToVBNetProjectLabel()
		{
			Assert.AreEqual("${res:ICSharpCode.PythonBinding.ConvertVBNetProjectToPythonProject}", convertVBNetProjectCodon["label"]);
		}
		
		[Test]
		public void ConvertToCSharpProjectClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.ConvertProjectToPythonProjectCommand", convertCSharpProjectCodon["class"]);
		}

		[Test]
		public void ConvertToVBNetProjectClass()
		{
			Assert.AreEqual("ICSharpCode.PythonBinding.ConvertProjectToPythonProjectCommand", convertVBNetProjectCodon["class"]);
		}
		
		[Test]
		public void ConvertToCSharpProjectConditionName()
		{
			ICondition condition = convertCSharpProjectCodon.Conditions[0];
			Assert.AreEqual("ProjectActive", condition.Name);
		}
		
		[Test]
		public void ConvertToCSharpProjectConditionActiveExtension()
		{
			Condition condition = convertCSharpProjectCodon.Conditions[0] as Condition;
			Assert.AreEqual("C#", condition["activeproject"]);
		}

		[Test]
		public void ConvertToVBNetProjectConditionName()
		{
			ICondition condition = convertVBNetProjectCodon.Conditions[0];
			Assert.AreEqual("ProjectActive", condition.Name);
		}
		
		[Test]
		public void ConvertToVBNetProjectConditionActiveExtension()
		{
			Condition condition = convertVBNetProjectCodon.Conditions[0] as Condition;
			Assert.AreEqual("VBNet", condition["activeproject"]);
		}
		
		/// <summary>
		/// Gets the codon with the specified extension path and name.
		/// </summary>
		Codon GetCodon(string extensionPath, string name)
		{
			if (addin.Paths.ContainsKey(extensionPath)) {
				ExtensionPath path = addin.Paths[extensionPath];
				return GetCodon(path.Codons, name);
			}
			return null;
		}
		
		/// <summary>
		/// Gets the codon with the specified name.
		/// </summary>
		Codon GetCodon(List<Codon> codons, string name)
		{
			foreach (Codon codon in codons) {
				if (codon.Id == name) {
					return codon;
				}
			}
			return null;
		}		
	}
}
