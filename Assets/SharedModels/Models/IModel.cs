using System;

namespace Models.Models
{
    public interface IModel : IDisposable
    {
        void Calculate(Ship ship);
    }
}
