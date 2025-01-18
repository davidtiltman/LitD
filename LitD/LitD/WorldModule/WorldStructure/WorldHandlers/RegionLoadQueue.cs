using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace LitD.WorldModule.WorldStructure.WorldHandlers
{
    /// <summary> Очередь для асинхронной подгрузки регионов. </summary>
    internal class RegionLoadQueue
    {
        /// <summary> Первый регион в очереди. </summary>
        private static Region _next = null;

        private static List<Region> _queue = new List<Region>();

        private static bool _collectionLock = false;

        /// <summary> Получить первый регион в очереди. </summary>
        /// <returns> Первый регион в очереди. </returns>
        public static Region Pop() 
        {
            if (!_collectionLock && _queue.Count > 0)
            {
                Region result = _next;

                _queue.RemoveAt(0);
                
                if (_queue.Count > 0)
                {
                    _next = _queue.ElementAt(0);
                }
                else
                {
                    _next = null;
                }
                return result;
            }
            return null;
        }

        /// <summary> Добавить регион в очередь. </summary>
        /// <param name="region"> Регион. </param>
        public static void Push(Region region)
        {
            _collectionLock = true;

            if (_queue.Select(r => r.Position).Contains(region.Position))
            {
                _collectionLock = false;
                return;
            }

            _queue.Add(region);

            if (_next == null)
            {
                _next = _queue.ElementAt(0);
            }

            _collectionLock = false;
        }
    }
}
