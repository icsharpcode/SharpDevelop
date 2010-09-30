// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.FormsDesigner;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using UnitTesting.Tests.Utils;

namespace ICSharpCode.Scripting.Tests.Designer
{
	[TestFixture]
	[RequiresSTA]
	public class ScriptingDesignerGeneratorTests
	{
		MockTextEditorOptions textEditorOptions;
		TestableScriptingDesignerGenerator generator;
		FormsDesignerViewContent formsDesignerView;
		MockTextEditorViewContent textEditorViewContent;
		MockOpenedFile formsDesignerOpenedFile;
		MockDesignerLoaderHost host;
		FakeDesignerSerializationManager serializationManager;
		FakeCodeDomSerializer serializer;
		MockMethod method;
		
		[Test]
		public void GetSourceFiles_FormDesignerViewHasOpenFile_ReturnsOneFile()
		{
			CreateDesignerGenerator();
			OpenedFile designerOpenedFile;
			IEnumerable<OpenedFile> files = generator.GetSourceFiles(out designerOpenedFile);
			int count = HowManyInCollection(files);
			
			Assert.AreEqual(1, count);
		}
		
		void CreateDesignerGenerator()
		{
			textEditorViewContent = new MockTextEditorViewContent();
			formsDesignerOpenedFile = new MockOpenedFile("test.py");
			textEditorViewContent.PrimaryFile = formsDesignerOpenedFile;
			formsDesignerView = new FormsDesignerViewContent(textEditorViewContent, formsDesignerOpenedFile);
			textEditorOptions = new MockTextEditorOptions();
			generator = new TestableScriptingDesignerGenerator(textEditorOptions);
			generator.Attach(formsDesignerView);
			generator.ParseInfoToReturnFromParseFile = generator.CreateParseInfoWithOneClass();
		}
		
		int HowManyInCollection(IEnumerable<OpenedFile> files)
		{
			int count = 0;
			foreach (OpenedFile file in files) {
				count++;
			}
			return count;
		}
		
		[Test]
		public void GetSourceFiles_FormsDesignerHasOpenFile_DesignerOpenedFileParameterIsSetToFormsDesignerViewOpenedFile()
		{
			CreateDesignerGenerator();
			OpenedFile designerOpenedFile;
			generator.GetSourceFiles(out designerOpenedFile);
			
			AssertAreEqual(formsDesignerOpenedFile, designerOpenedFile);
		}
		
		void AssertAreEqual(OpenedFile expectedOpenedFile, OpenedFile actualOpenedFile)
		{	
			string fileName = actualOpenedFile.FileName.ToString();
			string expectedFileName = expectedOpenedFile.FileName.ToString();
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetSourceFiles_FormsDesignerHasOpenFile_FormsDesignerFileReturnedInFiles()
		{
			CreateDesignerGenerator();
			OpenedFile designerOpenedFile;
			IEnumerable<OpenedFile> files = generator.GetSourceFiles(out designerOpenedFile);
			IEnumerator<OpenedFile> enumerator = files.GetEnumerator();
			enumerator.MoveNext();
			OpenedFile file = enumerator.Current;
			
			AssertAreEqual(formsDesignerOpenedFile, file);
		}
		
		[Test]
		public void GetCompatibleMethods_OneClassMethod_CallsParseFile()
		{
			CreateDesignerGenerator();
			CallGetCompatibleMethods();
			Assert.IsTrue(generator.IsParseFileCalled);
		}
		
		ICollection CallGetCompatibleMethods()
		{
			MockEventDescriptor descriptor = new MockEventDescriptor("Click");
			return generator.GetCompatibleMethods(descriptor);
		}
		
		[Test]
		public void GetCompatibleMethods_OneClassMethod_ParseFilePassedFileNameOpenInTextEditor()
		{
			CreateDesignerGenerator();
			CallGetCompatibleMethods();
			
			string fileName = generator.FileNamePassedToParseFile;
			string expectedFileName = "test.py";
			
			Assert.AreEqual(expectedFileName, fileName);
		}
		
		[Test]
		public void GetCompatibleMethods_OneClassMethod_ParseFilePassedTextContentInTextEditor()
		{
			CreateDesignerGenerator();
			formsDesignerView.DesignerCodeFileContent = "code";
			CallGetCompatibleMethods();
			
			string textContent = generator.TextContentPassedToParseFile;
			string expectedTextContent = "code";
			
			Assert.AreEqual(expectedTextContent, textContent);
		}
		
		[Test]
		public void GetCompatibleMethods_OneMethodWithTwoParameters_MethodNameReturnedInCompatibleMethods()
		{
			CreateDesignerGenerator();
			ParseInformation parseInfo = generator.CreateParseInfoWithOneMethodWithTwoParameters("button1_click");
			generator.ParseInfoToReturnFromParseFile = parseInfo;
			ICollection methods = CallGetCompatibleMethods();
			
			string[] expectedMethods = new string[] { "button1_click" };
			
			Assert.AreEqual(expectedMethods, methods);
		}
		
		[Test]
		public void GetCompatibleMethods_OneMethodWithNoParameters_NoMethodsReturnedInCompatibleMethods()
		{
			CreateDesignerGenerator();
			ParseInformation parseInfo = generator.CreateParseInfoWithOneMethod("button1_click");
			generator.ParseInfoToReturnFromParseFile = parseInfo;
			ICollection methods = CallGetCompatibleMethods();
			
			Assert.AreEqual(0, methods.Count);
		}
		
		[Test]
		public void Detach_FormsDesignerViewAlreadyAttached_SetsViewContentToNull()
		{
			CreateDesignerGenerator();
			generator.Detach();
			Assert.IsNull(generator.ViewContent);
		}
		
		[Test]
		public void GenerateMethodBody_PassedDesignerHost_DesignerHostPassedToCodeDomSerializer()
		{
			CreateDesignerGenerator();
			CreateParametersForGenerateMethodBodyCall();
			CallGenerateMethodBody();
			
			Assert.AreEqual(host, serializer.HostPassedToGenerateInitializeComponentMethodBody);
		}
		
		void CreateParametersForGenerateMethodBodyCall()
		{
			serializer = new FakeCodeDomSerializer();
			generator.SerializerToReturnFromCreateCodeDomSerializer = serializer;
			method = MockMethod.CreateMockMethodWithoutAnyAttributes();
			host = new MockDesignerLoaderHost();
			serializationManager = new FakeDesignerSerializationManager();
		}
		
		string CallGenerateMethodBody()
		{
			return generator.GenerateMethodBody(method, host, serializationManager);
		}
		
		[Test]
		public void GenerateMethodBody_MethodBodyCreated_ReturnsMethodBodyFromCodeDomSerializer()
		{
			CreateDesignerGenerator();
			CreateParametersForGenerateMethodBodyCall();
			serializer.MethodBodyToReturnFromGenerateMethodBodyCall = "test";
			string methodBody = CallGenerateMethodBody();
			
			string expectedMethodBody = "test";
			Assert.AreEqual(expectedMethodBody, methodBody);
		}
		
		[Test]
		public void GenerateMethodBody_PassedDesignerSerializationManager_DesignerSerializationManagerPassedToCodeDomSerializer()
		{
			CreateDesignerGenerator();
			CreateParametersForGenerateMethodBodyCall();
			CallGenerateMethodBody();
			
			Assert.AreEqual(serializationManager, serializer.SerializationManagerGenerateInitializeComponentMethodBody);
		}
		
		[Test]
		public void GenerateMethodBody_TextEditorOptionsConvertTabsToSpacesIsFalse_MethodBeginColumnIsIndentPassedToGenerateMethodBody()
		{
			CreateDesignerGenerator();
			CreateParametersForGenerateMethodBodyCall();
			
			int beginLine = 1;
			int beginColumn = 3;
			int endLine = 2;
			int endColumn = 1;
			method.Region = new DomRegion(beginLine, beginColumn, endLine, endColumn);
			
			textEditorOptions.ConvertTabsToSpaces = false;
			
			CallGenerateMethodBody();
		
			int expectedIndent = 3;
			
			Assert.AreEqual(expectedIndent, serializer.InitialIndentPassedToGenerateInitializeComponentMethodBody);
		}
		
		[Test]
		public void GenerateMethodBody_ProjectHasRootNamespace_RootNamespacePassedToCodeDomSerializer()
		{
			CreateDesignerGenerator();
			CreateParametersForGenerateMethodBodyCall();
			
			method.MockDeclaringType.MockProjectContent.ProjectAsIProject.RootNamespace = "Test";
			
			CallGenerateMethodBody();
			
			string expectedRootNamespace = "Test";
			Assert.AreEqual(expectedRootNamespace, serializer.RootNamespacePassedToGenerateInitializeComponentMethodBody);
		}
	}
}
