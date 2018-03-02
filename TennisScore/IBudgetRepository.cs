using System.Collections.Generic;

namespace TennisScore
{
    public interface IBudgetRepository<T>
    {
        List<Budget> GetAll();
    }
}