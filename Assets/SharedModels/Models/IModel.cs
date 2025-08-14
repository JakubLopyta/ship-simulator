using System;
using UnityEngine;

namespace Models.Models
{
    public interface IModel : IDisposable
    {
        void Calculate(Ship ship);
        Vector3 CalculateV3(Ship ship);
    }
}
