using LitD.System.Constants;
using LitD.System.SerializableTypes;
using LitD.WorldModule.WorldGeneration;
using Microsoft.Xna.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LitD.WorldModule.WorldStructure
{
    /// <summary> Самый крупный элемент мира - он содержит в себе набор чанков. </summary>
    [ProtoContract]
    internal class Region
    {
        /// <summary> Координаты региона по X. Измеряется в регионах: смещение в 1 регион = X + 1. </summary>
        [ProtoMember(1)]
        public int Position { get; private set; }

        /// <summary> Чанки региона. </summary>
        [ProtoMember(2)]
        private List<Chunk> _regionChunks = new List<Chunk>();

        // это пока будет здесь, потому что константу нельзя объявить с использованием Math.Abs
        private static int _regionHeightInChunks = Math.Abs(WorldConstants.WORLD_HIGHEST_CHUNK) + Math.Abs(WorldConstants.WORLD_LOWEST_CHUNK);
        private static int _regionSizeInChunks = WorldConstants.REGION_WIDTH * _regionHeightInChunks;

        #region Initialize

        /// <summary> Конструктор, создающий новый регион на координатах. </summary>
        /// <param name="position"> Координаты нового региона. </param>
        public Region(int xPosition)
        {
            Position = xPosition;

            Chunk[] temp = new Chunk[_regionSizeInChunks];

            for (int y = WorldConstants.WORLD_LOWEST_CHUNK, index = 0; y < WorldConstants.WORLD_HIGHEST_CHUNK; y++)
            {
                for (int x = 0; x < WorldConstants.REGION_WIDTH; x++, index++)
                {
                    temp[index] = ChunkGenerator.GenerateChunk(new Vector2(
                        Position * WorldConstants.REGION_WIDTH + x,
                        y
                    ));
                }
            }

            _regionChunks = temp.ToList();
        }

        private Region()
        { }

        #endregion

        /// <summary> Возвращает список чанков. </summary>
        /// <returns> Список чанков. </returns>
        public List<Chunk> GetChunks()
        {
            return _regionChunks;
        }

        /// <summary>
        /// Переписывает чанк в регионе.
        /// </summary>
        /// <param name="chunk"> Чанк. </param>
        /// <param name="position"> Координаты чанка относительно начала региона (слева сверху вправо вниз). </param>
        public void SetChunk(Chunk chunk, Vector2 position)
        {
            try
            {
                _regionChunks[(int)position.Y * _regionHeightInChunks + (int)position.X] = chunk;
            }
            catch
            {
                throw new IndexOutOfRangeException("New chunk position is out of region bounds");
            }
        }

        /// <summary> Возвращает чанки региона в указанном отрезке по высоте. </summary>
        /// <param name="topY"> Верхняя граница. </param>
        /// <param name="bottomY"> Нижняя граница. </param>
        /// <returns> Массив чанков. </returns>
        public Chunk[] GetChunkArrayInYRange(int topY, int bottomY)
        {
            List<Chunk> inRange = new List<Chunk>();

            int yOffset = Math.Abs(WorldConstants.WORLD_LOWEST_CHUNK);
            int index = -1;

            for (int y = topY; y >= bottomY; y--)
            {
                for (int x = 0; x < WorldConstants.REGION_WIDTH; x++)
                {
                    index = (yOffset + y) * WorldConstants.REGION_WIDTH + x;
                    if (index < 0 ||
                        index >= WorldConstants.REGION_WIDTH * (Math.Abs(WorldConstants.WORLD_LOWEST_CHUNK) + Math.Abs(WorldConstants.WORLD_HIGHEST_CHUNK)))
                        continue;

                    inRange.Add(_regionChunks[index]);
                }
            }

            return inRange.ToArray();
        }
           

        /// <summary> Сериализация региона. </summary>
        public void SaveRegion(string worldDirectory)
        {
            string filePath = GetRegionFilePath(worldDirectory, Position);
            File.Create(filePath).Close();

            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                Serializer.Serialize<Region>(file, this);
            }
        }

        /// <summary> Преобразует координаты региона в путь к файлу региона. </summary>
        /// <param name="position"> Координаты региона. </param>
        /// <returns> Путь к файлу. </returns>
        public static string GetRegionFilePath(string worldDirectory, int xPosition)
        {
            string path = string.Empty;

            path += $"{worldDirectory}/";
            path += $"{FolderNameConstants.WorldRegionFolderName}/";
            path += $"{xPosition}.{FileNameConstants.REGION_FILE_EXTENSION}";

            return path;
        }

        /// <summary> Инициализирует спрайты содержимых сущностей. </summary>
        public void InitializeEntitySprites()
        {
            foreach (Chunk chunk in GetChunks())
            {
                chunk.InitializeEntitySprites();
            }
        }

        /// <summary> Проверяет виден ли регион какому-либо наблюдателю. </summary>
        /// <returns> False, если в регионе нет видимых чанков. True, если есть. </returns>
        public bool IsVisible()
        {
            return GetChunks().Any(c => c.IsVisible);
        }
    }
}
