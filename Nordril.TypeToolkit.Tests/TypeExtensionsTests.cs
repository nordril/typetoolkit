using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Nordril.TypeToolkit.Tests
{
    public static class TypeExtensionsTests
    {
        public static IEnumerable<object[]> UnifiableWithData()
        {
            yield return new object[]
            {
                typeof(string),
                typeof(string),
                true,
            };

            yield return new object[]
            {
                typeof(int),
                typeof(int),
                true,
            };

            yield return new object[]
            {
                typeof(IEquatable<int>),
                typeof(int),
                true,
            };

            yield return new object[]
            {
                typeof(Manager),
                typeof(Manager),
                true,
            };

            yield return new object[]
            {
                typeof(Employee),
                typeof(Manager),
                true,
            };

            yield return new object[]
            {
                typeof(Person),
                typeof(Manager),
                true,
            };

            yield return new object[]
            {
                typeof(IEquatable<Person>),
                typeof(Manager),
                true,
            };

            yield return new object[]
            {
                typeof(MyList<>),
                typeof(MyList<>),
                true,
            };

            yield return new object[]
            {
                typeof(IEnumerable<>),
                typeof(MyList<>),
                true,
            };

            yield return new object[]
            {
                typeof(IEnumerable<int>),
                typeof(MyList<int>),
                true,
            };

            yield return new object[]
            {
                typeof(IComparable<>),
                typeof(Manager),
                false,
            };

            yield return new object[]
            {
                typeof(Manager),
                typeof(Car),
                false,
            };

            yield return new object[]
            {
                typeof(Manager),
                typeof(IEnumerable<Manager>),
                false,
            };

            yield return new object[]
            {
                typeof(IEquatable<Manager>),
                typeof(Manager),
                false,
            };

            yield return new object[]
            {
                typeof(Manager),
                typeof(string),
                false,
            };

            yield return new object[]
            {
                typeof(IEquatable<>),
                typeof(int),
                false,
            };

            yield return new object[]
            {
                typeof(Manager),
                typeof(Employee),
                false,
            };

            yield return new object[]
            {
                typeof(MyList<int>),
                typeof(IEnumerable<int>),
                false,
            };

            yield return new object[]
            {
                typeof(MyList<>),
                typeof(IEnumerable<>),
                false,
            };
        }

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
        public static void GetGenericArgumentsMethodInfoReturnsTypeArguments()
        {
            var mi = typeof(TestType).GetMethod(nameof(TestType.First), BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);

            Assert.Single(mi.GetGenericArgumentsSafe());
            Assert.True(mi.GetGenericArgumentsSafe()[0].IsGenericParameter);

            mi = mi.MakeGenericMethod(typeof(int));
            Assert.Single(mi.GetGenericArgumentsSafe());
            Assert.Equal(typeof(int), mi.GetGenericArgumentsSafe()[0]);

            mi = typeof(string).GetMethod(nameof(string.GetEnumerator), BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod);
            Assert.Equal(new Type[0], mi.GetGenericArgumentsSafe());
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

        [Theory]
        [MemberData(nameof(UnifiableWithData))]
        public static void UnifiableWithTest(Type type, Type that, bool expected)
        {
            var typeS = type.ToString();
            var thatS = that.ToString();

            if (expected)
                Assert.True(type.UnifiableWith(that, out var _));
            else
                Assert.False(type.UnifiableWith(that, out var _));
        }

        private class TestType
        {
            public T First<T>(IEnumerable<T> x) => x.First();
        }

        private class Manager : Employee
        {
            public int ManagerLevel { get; set; }
        }

        private class Employee : Person
        {
            public string Role { get; set; }
        }

        private class Person : IEquatable<Person>
        {
            public DateTime Birth { get; set; }
            public string Name { get; set; }

            public bool Equals(Person other) => Birth == other.Birth && Name == other.Name;
        }

        private class Car
        {
            public string Model { get; set; }
        }

        private class MyList<T> : IEnumerable<T>
        {
            public IEnumerator<T> GetEnumerator()
            {
                throw new NotImplementedException();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }
        }
    }
}
