using ICSharpCode.Core.Presentation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SDCommandManager=ICSharpCode.Core.Presentation.CommandManager;

namespace ICSharpCode.Core.Presentation.Tests
{
	[TestFixture]
    public class InputBindingCategoryTests
    {

    	[SetUp]
    	public void SetuUp()
    	{
    	}
    	
    	[TearDown]
    	public void TearDown()
    	{
    		SDCommandManager.Reset();
    	}
    	
    	[Test]
		public void AddUnregisteredInputBindingCategoryTests()
		{
			var bindingInfo = new InputBindingInfo();
			var category = new InputBindingCategory("/test", "Test");
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			Assert.Throws<ArgumentException>(delegate { bindingInfo.Categories.Add(category); });
			
			SDCommandManager.RegisterInputBindingCategory(category);
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			Assert.DoesNotThrow(delegate { bindingInfo.Categories.Add(category); });
		}
		
    	[Test]
		public void AddManyUnregisteredInputBindingCategoriesTests()
		{
			var bindingInfo = new InputBindingInfo();
			
			var category1 = new InputBindingCategory("/test", "Test");
			var category2 = new InputBindingCategory("/test2", "Test");
			var categoryCollection = new InputBindingCategoryCollection { category1, category2 };
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			Assert.Throws<ArgumentException>(delegate { bindingInfo.Categories.AddRange(categoryCollection); });
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			SDCommandManager.RegisterInputBindingCategory(category1);
			Assert.Throws<ArgumentException>(delegate { bindingInfo.Categories.AddRange(categoryCollection); });
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			SDCommandManager.RegisterInputBindingCategory(category2);
			Assert.DoesNotThrow(delegate { bindingInfo.Categories.AddRange(categoryCollection); });
		}
        
    	[Test]
		public void AssignInputBindingCategoryCollectionWithUnregistredCategoryTests()
		{
			var bindingInfo = new InputBindingInfo();
			
			var category = new InputBindingCategory("/test", "Test");
			var categoryCollection = new InputBindingCategoryCollection { category };
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			Assert.Throws<ArgumentException>(delegate { bindingInfo.Categories = categoryCollection; });
			
			bindingInfo.Categories = new InputBindingCategoryCollection();
			SDCommandManager.RegisterInputBindingCategory(category);
			Assert.DoesNotThrow(delegate { bindingInfo.Categories = categoryCollection; });
		}
    }
}
