using System;
using System.Collections.Generic;
using System.Linq;

namespace TennisScore
{
    public class BudgetCalculator
    {
        private readonly IBudgetRepository<Budget> _repo;

        public BudgetCalculator(IBudgetRepository<Budget> repo)
        {
            _repo = repo;
        }

        public int GetAmount(DateTime start, DateTime end)
        {
            if (start > end)
            {
                throw new ArgumentException();
            }

            var budgets = this._repo.GetAll();

            if (budgets.Count > 0)
            {
                var startTimeString = FormateDateTimeString(start);

                if (!HasData(budgets, startTimeString))
                {
                    return 0;
                }

                if (IsSameMonth(start, end))
                {
                    var budget = GetBudget(budgets, start);
                    return CalOneMonthAmount(start, end, budget);
                }
                else
                {

                    var startBudget = GetBudget(budgets, startTimeString);

                    var endBudget = GetBudget(budgets, end);

                    int tTotleAmount = CalOneMonthAmount(start, GetMonthLastDate(start), startBudget)
                                 + CalOneMonthAmount(GetMonthFirstDate(end), end, endBudget);
                    int tDateDiff = GetTimeDiff(start, end, "M");
                    if (tDateDiff == 2)
                    {
                        return tTotleAmount;
                    }
                    else
                    {
                        var tBudget = GetAmountRemoveFirstAndLastMonth(budgets, start, end);
                        //var tBudget = GetBudget(budgets, start.AddMonths(1));

                        return tTotleAmount + tBudget;
                    }

                }
            }

            return 0;
        }

        private int GetAmountRemoveFirstAndLastMonth(List<Budget> budgets, DateTime start, DateTime end)
        {
            var firstMonth = FormateDateTimeString(start);
            var lastMonth = FormateDateTimeString(end);

            //double sum = Table.Select(t => t.Amount ?? 0).Sum();

            var totalMonthBudget = budgets.Select(t => t.Amount).Sum();
            var firstMonthBudget = budgets.First();
            var lastMonthBudget = budgets.Last();

            return totalMonthBudget - (firstMonthBudget.Amount + lastMonthBudget.Amount);
        }

        private static Budget GetBudget(List<Budget> budgets, DateTime end)
        {
            var endTimeString = FormateDateTimeString(end);
            var endBudget = GetBudget(budgets, endTimeString);
            return endBudget;
        }

        public int GetTimeDiff(DateTime dtStart, DateTime dtEnd, string strType)
        {
            //DateTime dtStart = DateTime.Parse(strFrom);
            //DateTime dtEnd = DateTime.Parse(strTo);

            if (strType == "M")
            {
                int iMonths = dtEnd.Year * 12 + dtEnd.Month - (dtStart.Year * 12 + dtStart.Month) + 1;
                return iMonths;
            }
            else return 0;
        }

        private static DateTime GetMonthFirstDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        private static DateTime GetMonthLastDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }

        private static string FormateDateTimeString(DateTime end)
        {
            return end.ToString("yyyyMM");
        }

        private static Budget GetBudget(List<Budget> budgets, string startTimeString)
        {
            return budgets.First(w => w.YearMonth == startTimeString);
        }

        private bool HasData(List<Budget> budgets, string timeString)
        {
            return budgets.Any(w => w.YearMonth == timeString);
        }

        private int CalOneMonthAmount(DateTime start, DateTime end, Budget budget)
        {
            var daysInMonth = DateTime.DaysInMonth(start.Year, start.Month);
            var days = (end - start).Days + 1;

            return budget.Amount / daysInMonth * days;
        }

        private bool IsSameMonth(DateTime start, DateTime end)
        {
            return start.Year == end.Year && start.Month == end.Month;
        }
    }
}