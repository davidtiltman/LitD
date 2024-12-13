using Microsoft.Xna.Framework;

namespace LitD.World
{
    /// <summary> Отвечает за служебные операции над миром. </summary>
    internal class InWorldOperations
    {
        /// <summary> Преобразовывает координаты пикселя в мировые координаты. </summary>
        /// <param name="screenPosition"> Координаты на экране. </param>
        /// <returns> Мировые координаты </returns>
        public static Vector2 ConvertPixelToWorldPosition(Vector2 screenPosition)
        {
            return new Vector2(screenPosition.X / WorldConstants.DEFAULT_TILE_SIZE, screenPosition.Y / WorldConstants.DEFAULT_TILE_SIZE);
        }

        /// <summary> Проверяет существует ли чанк на координатах игрока. </summary>
        public static void IsChunkExists()
        {
            // если нет, то GenerateChunk
        }
    }
}
