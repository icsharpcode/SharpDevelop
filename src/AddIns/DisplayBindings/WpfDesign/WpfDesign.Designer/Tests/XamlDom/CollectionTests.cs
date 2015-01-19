using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media.Animation;
using ICSharpCode.WpfDesign.XamlDom;
using NUnit.Framework;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
    [TestFixture]
    public class CollectionTests
    {
        [Test]
        public void LineBreakNoCollection()
        {
            var isCollection = CollectionSupport.IsCollectionType(typeof(LineBreak));

            Assert.IsFalse(isCollection);
        }
        
        /// <summary>
        /// This test is here do demonstrate a peculiarity (or bug) with collections inside System.Windows.Input namespace.
        /// The problem is that inserting the first item into the collection will silently fail (Count still 0 after Insert), first item must use Add method.
        /// </summary>
        [Test]
		public void InputCollectionsPeculiarityOrBug()
		{
			// InputBindingCollection START
			var inputBindingCollection = new System.Windows.Input.InputBindingCollection();
			
			// NOTE: this silently fails (Count is 0 after insert)
			inputBindingCollection.Insert(0, new System.Windows.Input.MouseBinding());
			Assert.IsTrue(inputBindingCollection.Count == 0);
			
			inputBindingCollection.Add(new System.Windows.Input.MouseBinding());
			Assert.IsTrue(inputBindingCollection.Count == 1);
			
			inputBindingCollection.Insert(0, new System.Windows.Input.MouseBinding());
			Assert.IsTrue(inputBindingCollection.Count == 2);
			// InputBindingCollection END
			
			// CommandBindingCollection START
			var commandBindingCollection = new System.Windows.Input.CommandBindingCollection();
			
			// NOTE: this silently fails (Count is 0 after insert)
			commandBindingCollection.Insert(0, new System.Windows.Input.CommandBinding());
			Assert.IsTrue(commandBindingCollection.Count == 0);
			
			commandBindingCollection.Add(new System.Windows.Input.CommandBinding());
			Assert.IsTrue(commandBindingCollection.Count == 1);
			
			commandBindingCollection.Insert(0, new System.Windows.Input.CommandBinding());
			Assert.IsTrue(commandBindingCollection.Count == 2);
			// CommandBindingCollection END
			
			// List START (how it probably should work...)
			var list = new List<string>();
			
			// NOTE: this is successful for ordinary List<T>
			list.Insert(0, "A");
			Assert.IsTrue(list.Count == 1);
			
			list.Add("A");
			Assert.IsTrue(list.Count == 2);
			
			list.Insert(0, "A");
			Assert.IsTrue(list.Count == 3);
			// List END
		}
    }
}
