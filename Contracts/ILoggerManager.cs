using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ILoggerManager
    {
        public void LogInfo(string message);
        public void LogDebug(string message);
        public void LogWarn(string message);
        public void LogError(string message);
    }
}
