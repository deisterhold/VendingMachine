using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vending.Iot.Models
{
    public enum VendingMachineColumn
    {
        Left = 1,
        Middle = 2,
        Right = 4
    }

    public class VendingMachine : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
