namespace LitD.WorldModule
{
    /// <summary> Константы для генерации и обработки мира. </summary>
    internal class WorldConstants
    {
        /// <summary> Размер обычного тайла в пикселях. </summary>
        public const float DEFAULT_TILE_SIZE = 32;

        /// <summary> Длина стороны чанка в тайлах. Чанк квадратный. </summary>
        public const int CHUNK_SIZE = 8;

        /// <summary> Длина стороны чанка в пикселях. Чанк квадратный. </summary>
        public const int CHUNK_SIZE_IN_PIXELS = (int)DEFAULT_TILE_SIZE * CHUNK_SIZE;

        /// <summary> Дальность прорисовки чанков. </summary>
        public const int CHUNK_DRAW_DISTANCE = 8;

        /// <summary> Размер региона в чанках. </summary>
        public const int REGION_SIZE = 16;
    }
}
