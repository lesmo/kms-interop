using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMS.Interop.OAuth.Utils;
using Xunit;

// ReSharper disable CheckNamespace
namespace KMS.Interop.OAuth.Utils.Tests {
    public class LexicographicalComparer_Tests {
        [Fact(DisplayName = "LexicographicalComparer.Compare")]
        public void Compare_Test() {
            var stringsToCompare = new[] {
                "A string",
                "another string"
            };
            var stringCompareResult = new LexicographicalComparer().Compare(
                stringsToCompare[0],
                stringsToCompare[1]
            );

            Assert.True(stringCompareResult < 0);
        }

        [Fact(DisplayName = "LexicographicalComparer.Compare (Sort)")]
        public void Compare_SortTest() {
            var someStrings = new[] {
                "abc",
                "Def",
                "Abc"
            };
            var someStringsLexicographicallySorted = new[] {
                "Abc",
                "Def",
                "abc"
            };

            var someStringsSortedAggregated =
                someStrings.OrderBy(b => b, new LexicographicalComparer()).Aggregate((i, j) => i + j);
            var someStringBadlySortedAggregated =
                someStrings.OrderBy(b => b).Aggregate((i, j) => i + j);
            var someStringsLexicographicallySortedAggregated =
                someStringsLexicographicallySorted.Aggregate((i, j) => i + j);

            Assert.Equal(
                someStringsSortedAggregated.ToCharArray(),
                someStringsLexicographicallySortedAggregated.ToCharArray()
            );
            Assert.NotEqual(
                someStringBadlySortedAggregated.ToCharArray(),
                someStringsLexicographicallySortedAggregated.ToCharArray()
            );
        }
    }
}
