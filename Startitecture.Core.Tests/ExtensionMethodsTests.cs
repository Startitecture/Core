// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionMethodsTests.cs" company="Startitecture">
//   Copyright (c) Startitecture. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Startitecture.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Startitecture.Core;

    /// <summary>
    /// The extension methods tests.
    /// </summary>
    [TestClass]
    public class ExtensionMethodsTests
    {
        /// <summary>
        /// The get property test.
        /// </summary>
        [TestMethod]
        public void GetProperty_ExpressionForProperty_MatchesExpected()
        {
            Expression<Func<FakeTestItem, object>> expression = item => item.TestInt;
            var actual = expression.GetProperty();
            Assert.IsNotNull(actual);
            Assert.AreEqual(nameof(FakeTestItem.TestInt), actual.Name);
        }

        /// <summary>
        /// The to property dictionary_ test object all properties_ matches expected.
        /// </summary>
        [TestMethod]
        public void PopulateDictionary_TestObjectAllProperties_MatchesExpected()
        {
            var item = new FakeTestItem { TestDateTime = DateTime.Today, TestInt = 23, TestString = "Test String" };
            IDictionary<string, object> expected = new Dictionary<string, object>
                                                       {
                                                           { "{Item}", item },
                                                           { "TestDateTime", item.TestDateTime },
                                                           { "TestInt", item.TestInt },
                                                           { "TestList", item.TestList },
                                                           { "TestString", item.TestString }
                                                       };

            var actual = new Dictionary<string, object>();
            actual.PopulateDictionary(item);
            string Func(KeyValuePair<string, object> x) => $"{x.Key}={x.Value}";

            var expectedCollection = expected.Select(Func).ToList();
            var actualCollection = actual.Select(Func).ToList();
            CollectionAssert.AreEqual(expectedCollection, actualCollection);
        }

        /// <summary>
        /// A test for GetPropertyValue.
        /// </summary>
        [TestMethod]
        public void GetPropertyValue_ExistingProperty_MatchesExpected()
        {
            var propertyInfo = typeof(FakeTestItem).GetProperty("TestString");
            var entity = new FakeTestItem { TestInt = 12, TestString = "Test" };
            const string Expected = "Test";
            var actual = propertyInfo.GetPropertyValue(entity);
            Assert.AreEqual(Expected, Convert.ToString(actual, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// A test for GetDifferences.
        /// </summary>
        [TestMethod]
        public void GetDifferences_AllValuesDifferentWithSpecifiedProperties_MatchesExpected()
        {
            var baseline = new FakeTestItem { TestDateTime = DateTime.MinValue, TestInt = 20, TestString = "TestString" };
            var comparison = new FakeTestItem { TestDateTime = DateTime.MaxValue, TestInt = 21, TestString = "TestString2" };
            var propertiesToCompare = new[] { "TestInt", "TestString" };
            IEnumerable<PropertyComparisonResult> expected = new List<PropertyComparisonResult>
                                                                 {
                                                                     new PropertyComparisonResult("TestInt", 20, 21),
                                                                     new PropertyComparisonResult("TestString", "TestString", "TestString2")
                                                                 };

            IEnumerable<PropertyComparisonResult> actual = baseline.GetDifferences(comparison, propertiesToCompare);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        /// <summary>
        /// A test for GetDifferences.
        /// </summary>
        [TestMethod]
        public void GetDifferences_AllValuesDifferent_MatchesExpected()
        {
            var baseline = new FakeTestItem { TestInt = 20, TestString = "TestString" };
            var comparison = new FakeTestItem { TestInt = 21, TestString = "TestString2" };
            var propertiesToCompare = Array.Empty<string>();
            var expected = new List<PropertyComparisonResult>
                               {
                                   new PropertyComparisonResult("TestInt", 20, 21),
                                   new PropertyComparisonResult("TestString", "TestString", "TestString2")
                               };

            var actual = baseline.GetDifferences(comparison, propertiesToCompare).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// A test for GetDifferences.
        /// </summary>
        [TestMethod]
        public void GetDifferences_AllValuesEqual_ReturnsEmpty()
        {
            var baseline = new FakeTestItem { TestInt = 20, TestString = "TestString" };
            var comparison = new FakeTestItem { TestInt = 20, TestString = "TestString" };
            var propertiesToCompare = Array.Empty<string>();
            IEnumerable<PropertyComparisonResult> expected = new List<PropertyComparisonResult>();
            var actual = baseline.GetDifferences(comparison, propertiesToCompare);
            CollectionAssert.AreEqual(expected.ToList(), actual.ToList());
        }

        /// <summary>
        /// The to runtime name test.
        /// </summary>
        [TestMethod]
        public void ToRuntimeName_NoGenerics_MatchesExpected()
        {
            var expected = nameof(FakeTestItem);
            var actual = typeof(FakeTestItem).ToRuntimeName();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The to runtime name test.
        /// </summary>
        [TestMethod]
        public void ToRuntimeName_SingleGeneric_MatchesExpected()
        {
            var expected = "List`1<String>";
            var actual = typeof(List<string>).ToRuntimeName();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The to runtime name test.
        /// </summary>
        [TestMethod]
        public void ToRuntimeName_MultipleGenerics_MatchesExpected()
        {
            var expected = "Tuple`4<String, Int32, Int32, Decimal>";
            var actual = typeof(Tuple<string, int, int, decimal>).ToRuntimeName();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// The get member test.
        /// </summary>
        [TestMethod]
        public void GetMember_MemberExpression_IsNotNull()
        {
            Expression<Func<FakeTestItem, object>> expression = item => item.TestInt;
            var actual = expression.GetMember();
            Assert.IsNotNull(actual);
        }

        /// <summary>
        /// The get member test.
        /// </summary>
        [TestMethod]
        public void GetPropertyName_MemberExpression_IsNotNull()
        {
            Expression<Func<FakeTestItem, object>> expression = item => item.TestInt;
            var actual = expression.GetPropertyName();
            Assert.AreEqual(nameof(FakeTestItem.TestInt), actual);
        }

        /// <summary>
        /// The get member test.
        /// </summary>
        [TestMethod]
        public void GetPropertyValue_MemberExpression_IsNotNull()
        {
            Expression<Func<FakeTestItem, object>> expression = item => item.TestInt;
            var fakeTestItem = new FakeTestItem { TestInt = 32 };
            var actual = fakeTestItem.GetPropertyValue(expression.GetPropertyName());
            Assert.AreEqual(32, actual);
        }
    }
}