using LitD.WorldModule.WorldStructure.WorldHandlers;
using Microsoft.Xna.Framework;
using ProtoBuf;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace LitD.WorldModule.WorldStructure.WorldServices
{
    /// <summary> Отгрузчик регионов. </summary>
    internal class UnloadHandler
    {
        #region Unload logic

        /// <summary> Отгружает регионы. </summary>
        /// <param name="observerPosition"> Координаты наблюдателя. </param>
        public static void UnloadRegions(GameTime gameTime, Vector2 observerPosition, ref List<Region> regions, string worldDirectory)
        {
            int unloadTimer = (int)(gameTime.TotalGameTime.TotalSeconds) % WorldConstants.REGION_UNLOAD_FREQUENCY;
            if (unloadTimer == 0)
            {
                int observerX = (int)(observerPosition.X / (WorldConstants.REGION_WIDTH * WorldConstants.CHUNK_SIZE_IN_PIXELS));

                for (int i = 0; i < regions.Count; i++)
                {
                    int? regionX = regions[i]?.Position;

                    int loadDistanceMin = observerX - WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;
                    int loadDistanceMax = observerX + WorldConstants.NEAR_REGIONS_LOAD_DISTANCE;

                    if (regionX < loadDistanceMin || regionX > loadDistanceMax)
                    {
                        // если регион находится за пределами дальности подгрузки, то он выгружается
                        regions[i].SaveRegion(worldDirectory);
                        regions.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}
