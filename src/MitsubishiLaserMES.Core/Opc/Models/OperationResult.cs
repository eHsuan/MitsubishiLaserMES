using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
namespace MitsubishiLaserOpc.Models
{
    public sealed class OperationResult
    {
        private OperationResult(bool succeeded, string error)
        {
            Succeeded = succeeded;
            Error = error;
        }

        public bool Succeeded { get; private set; }
        public string Error { get; private set; }

        public static OperationResult Success()
        {
            return new OperationResult(true, null);
        }

        public static OperationResult Failure(string error)
        {
            return new OperationResult(false, error);
        }
    }
}
