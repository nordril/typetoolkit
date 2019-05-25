using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Nordril.TypeToolkit.Tests
{
    public static class TypeExtensionsTests
    {
        [Fact]
        public static void GetBaseNameReturnsBaseName()
        {
            Assert.Equal("IList", typeof(IList<int>).GetBaseName());
            Assert.Equal("IList", typeof(IList<>).GetBaseName());
            Assert.Equal("List", typeof(List<>).GetBaseName());
            Assert.Equal("List", typeof(List<List<int>>).GetBaseName());
            Assert.Equal("String", typeof(string).GetBaseName());
        }

        [Fact]
        public static void GetBaseFullNameReturnsFullBaseName()
        {
            Assert.Equal("System.Collections.Generic.IList", typeof(IList<int>).GetBaseFullName());
            Assert.Equal("System.Collections.Generic.IList", typeof(IList<>).GetBaseFullName());
            Assert.Equal("System.Collections.Generic.List", typeof(List<>).GetBaseFullName());
            Assert.Equal("System.Collections.Generic.List", typeof(List<List<int>>).GetBaseFullName());
            Assert.Equal("System.String", typeof(string).GetBaseFullName());
        }

        [Fact]
        public static void GetGenericArgumentsReturnsTypeArguments()
        {
            Assert.Equal(new[] { typeof(int) }, typeof(IList<int>).GetGenericArgumentsSafe());
            Assert.Equal(new Type[0], typeof(string).GetGenericArgumentsSafe());

            var ret = typeof(IList<>).GetGenericArgumentsSafe();

            Assert.Single(ret);
            Assert.True(ret[0].IsGenericParameter);
        }

        [Fact]
        public static void GetGenericDefinition()
        {
            Assert.Equal(typeof(IList<>), typeof(IList<int>).GetGenericTypeDefinitionSafe());
            Assert.Equal(typeof(IList<>), typeof(IList<List<bool>>).GetGenericTypeDefinitionSafe());
            Assert.Equal(typeof(List<>), typeof(List<int>).GetGenericTypeDefinitionSafe());
            Assert.Equal(typeof(IList<>), typeof(IList<>).GetGenericTypeDefinitionSafe());
            Assert.Equal(typeof(int), typeof(int).GetGenericTypeDefinitionSafe());
        }

        [Fact]
        public static void GetGenericName()
        {
            Assert.Equal("String", typeof(string).GetGenericName());
            Assert.Equal("System.String", typeof(string).GetGenericName(true));
            Assert.Equal("List<Int32>", typeof(List<int>).GetGenericName());
            Assert.Equal("List<List<List<Int32>>>", typeof(List<List<List<int>>>).GetGenericName());
            Assert.Equal("System.Collections.Generic.List<System.Collections.Generic.List<System.Collections.Generic.List<System.Int32>>>", typeof(List<List<List<int>>>).GetGenericName(true));
        }
    }
}
