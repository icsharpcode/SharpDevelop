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
    }
}
