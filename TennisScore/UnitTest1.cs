using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace TennisScore
{
    [TestClass]
    public class UnitTest1
    {
        IBudgetRepository<Budget> _repo = Substitute.For<IBudgetRepository<Budget>>();
        private BudgetCalculator _budgetCalculator;

        [TestMethod]
        public void OneAmount()
        {
            var list = new List<Budget>();
            list.Add(new Budget() { YearMonth = "201801", Amount = 31 });
            _repo.GetAll().Returns(list);
            TotalAmountShouldBe(new DateTime(2018, 1, 1), new DateTime(2018, 1, 15), 15);
        }

        [TestMethod]
        public void EmptyBudget()
        {
            GivenBudgets(new List<Budget>());
            TotalAmountShouldBe(new DateTime(2018, 1, 1), new DateTime(2018, 1, 1), 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Exception()
        {
            _repo.GetAll().Returns(new List<Budget>());
            TotalAmountShouldBe(new DateTime(2018, 1, 1), new DateTime(2018, 1, 1), 0);
            BudgetCalculator budgetCalculator = new BudgetCalculator(_repo);

            var amount = budgetCalculator.GetAmount(new DateTime(2018, 2, 1), new DateTime(2018, 1, 1));
        }

        [TestMethod]
        public void NoAmountData()
        {
            var list = new List<Budget>();
            list.Add(new Budget() { YearMonth = "201801", Amount = 31 });
            _repo.GetAll().Returns(list);

            TotalAmountShouldBe(new DateTime(2016, 1, 1), new DateTime(2016, 1, 5), 0);
        }

        [TestMethod]
        public void CrossMonth()
        {
            var list = new List<Budget>
            {
                new Budget() { YearMonth = "201802", Amount = 280 },
                new Budget() { YearMonth = "201803", Amount = 3100 }
            };
            _repo.GetAll().Returns(list);

            TotalAmountShouldBe(new DateTime(2018, 2, 5), new DateTime(2018, 3, 5), 740);
        }

        [TestMethod]
        public void Test0605To0805()
        {
            var list = new List<Budget>
            {
                new Budget() { YearMonth = "201806", Amount = 300 },
                new Budget() { YearMonth = "201807", Amount = 3100 },
                new Budget() { YearMonth = "201808", Amount = 31000 }
            };
            _repo.GetAll().Returns(list);

            TotalAmountShouldBe(new DateTime(2018, 6, 5), new DateTime(2018, 8, 5), 8360);
        }

        [TestMethod]
        public void Test0605To0905()
        {
            var list = new List<Budget>
            {
                new Budget() { YearMonth = "201809", Amount = 0 },
                new Budget() { YearMonth = "201810", Amount = 3100 },
                new Budget() { YearMonth = "201811", Amount = 30 },
                new Budget() { YearMonth = "201812", Amount = 0 }
            };
            _repo.GetAll().Returns(list);

            TotalAmountShouldBe(new DateTime(2018, 9, 10), new DateTime(2018, 12, 10), 3130);
        }

        private void TotalAmountShouldBe(DateTime startDate, DateTime endDate, int expected)
        {
            AmountShouldBe(expected, _budgetCalculator.GetAmount(startDate, endDate));
        }

        private void GivenBudgets(List<Budget> budgets)
        {
            _repo.GetAll().Returns(budgets);
        }

        [TestInitialize]
        public void InitBudgetCalculator()
        {
            _budgetCalculator = new BudgetCalculator(_repo);
        }

        private static void AmountShouldBe(int expected, int amount)
        {
            Assert.AreEqual(expected, amount);
        }
    }
}