namespace LitD.WorldModule
{
    /// <summary> Хранилище для общих констант. </summary>
    internal class WorldConstants
    {
        /// <summary> Размер обычного тайла в пикселях. </summary>
        public const float DEFAULT_TILE_SIZE = 32;

        /// <summary> Длина стороны чанка в тайлах. Чанк квадратный. </summary>
        public const int CHUNK_SIZE = 4;

        /// <summary> Длина стороны чанка в пикселях. Чанк квадратный. </summary>
        public const int CHUNK_SIZE_IN_PIXELS = (int)DEFAULT_TILE_SIZE * CHUNK_SIZE;
    }
}
