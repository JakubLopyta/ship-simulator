using UnityEngine;

namespace Models.Models
{
    public interface IModel
    {
        Vector3 Calculate(Ship ship);
    }
}
