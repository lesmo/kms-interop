using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kms.Interop.OAuth.Utils;
using Xunit;
namespace Kms.Interop.OAuth.Utils.Tests {
    public class NameValueCollectionExpansion_Tests {
        [Fact(DisplayName = "NameValueCollection.SortByName")]
        public void SortByName_Test() {
            var nameValueCollection = new NameValueCollection() {
                {
                    "key2", "value2"
                }, {
                    "key1", "value1"
                }, {
                    "anotherkey1", "valuea1"
                }, {
                    "Anotherkey2", "valueA2"
                }
            };
            var sortedNameValueCollection = new[] {
                "Anotherkey2",
                "anotherkey1",
                "key1",
                "key2"
            };
            
            Assert.Equal(
                sortedNameValueCollection.Aggregate((i, j) => i + j).ToCharArray(),
                nameValueCollection.SortByName().AllKeys.Aggregate((i, j) => i + j).ToCharArray()
            );
        }
    }
}
