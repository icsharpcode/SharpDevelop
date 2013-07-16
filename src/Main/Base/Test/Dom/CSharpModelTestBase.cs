// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using NUnit.Framework;
using Rhino.Mocks;

namespace ICSharpCode.SharpDevelop.Dom
{
	public class CSharpModelTestBase
	{
		protected IProject project;
		protected IProjectContent projectContent;
		protected IEntityModelContext context;
		protected IUpdateableAssemblyModel assemblyModel;
		
		protected class RemovedAndAddedPair<T>
		{
			public IReadOnlyCollection<T> OldItems { get; private set; }
			public IReadOnlyCollection<T> NewItems { get; private set; }
			
			public RemovedAndAddedPair(IReadOnlyCollection<T> oldItems, IReadOnlyCollection<T> newItems)
			{
				this.OldItems = oldItems;
				this.NewItems = newItems;
			}
		}
		
		#region SetUp and other helper methods
		[SetUp]
		public virtual void SetUp()
		{
			SD.InitializeForUnitTests();
			SD.Services.AddStrictMockService<IParserService>();
			project = MockRepository.GenerateStrictMock<IProject>();
			projectContent = new CSharpProjectContent().AddAssemblyReferences(AssemblyLoader.Corlib);
			context = new ProjectEntityModelContext(project, ".cs");
			assemblyModel = new AssemblyModel(context);
			
			SD.ParserService.Stub(p => p.GetCompilation(project)).WhenCalled(c => c.ReturnValue = projectContent.CreateCompilation());
		}
		
		[TearDown]
		public virtual void TearDown()
		{
			SD.TearDownForUnitTests();
		}
		
		protected void AddCodeFile(string fileName, string code)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNull(oldFile, "Duplicate file name: " + fileName);
			var newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			assemblyModel.Update(oldFile, newFile);
		}
		
		IUnresolvedFile Parse(string fileName, string code)
		{
			var parser = new CSharpParser();
			var syntaxTree = parser.Parse(code, fileName);
			Assert.IsFalse(parser.HasErrors);
			return syntaxTree.ToTypeSystem();
		}
		
		protected void UpdateCodeFile(string fileName, string code)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNotNull(oldFile, "Could not update file (missing old file): " + fileName);
			var newFile = Parse(fileName, code);
			projectContent = projectContent.AddOrUpdateFiles(newFile);
			assemblyModel.Update(oldFile, newFile);
		}
		
		protected void RemoveCodeFile(string fileName)
		{
			var oldFile = projectContent.GetFile(fileName);
			Assert.IsNotNull(oldFile, "Could not remove file (missing old file): " + fileName);
			projectContent = projectContent.RemoveFiles(fileName);
			assemblyModel.Update(oldFile, null);
		}
		
		protected List<RemovedAndAddedPair<T>> ListenForChanges<T>(IModelCollection<T> collection)
		{
			var list = new List<RemovedAndAddedPair<T>>();
			collection.CollectionChanged += (oldItems, newItems) => list.Add(new RemovedAndAddedPair<T>(oldItems, newItems));
			return list;
		}
		#endregion
	}
}
