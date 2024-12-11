namespace LitD.World
{
    internal class WorldManager
    {
        /*
         * todos
         * 1. после генерации записать мир в файл
         * 2. в файл должны записываться чанки по мере генерации
         */

        /// <summary> Создает новый мир. </summary>
        public static void CreateNewWorld()
        {
            /*
             * 1. создание файла мира
             * 2. генерация первых чанков
             */
        }

        /// <summary> Загрузка существующего мира. </summary>
        public static void LoadWorld()
        {
            // чтение файла мира
        }

        /// <summary> Проверяет существует ли чанк на координатах игрока. </summary>
        public static void IsChunkExists()
        {
            // если нет, то GenerateChunk
        }

        /// <summary> Генерация нового чанка. </summary>
        public static void GenerateChunk()
        {
        }
    }
}
