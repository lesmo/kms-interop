using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kms.Interop.OAuth.Utils;
using Xunit;

// ReSharper disable CheckNamespace
namespace Kms.Interop.OAuth.Utils.Tests {
    public class EnumExpansion_Tests {
        private enum TestEnum {
            HelloIAmAnEnum
        }

        [Fact()]
        public void CamelCaseToUnderlineString_Test() {
            // ReSharper disable once ConvertToConstant.Local
            var someEnumVal = TestEnum.HelloIAmAnEnum;

            Assert.Equal("hello_i_am_an_enum".ToCharArray(), someEnumVal.CamelCaseToUnderlineString().ToCharArray());
        }
    }
}
