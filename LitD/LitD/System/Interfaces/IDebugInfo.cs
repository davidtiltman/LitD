using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LitD.System.Interfaces
{
    public interface IDebugInfo
    {
        /// <summary> Возвращает дебаг-информацию. </summary>
        /// <returns> Строка с дебаг-информацией. </returns>
        void GetDebugInfo(ref string debugInfo);
    }
}
