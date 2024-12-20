using LitD.System.Constants;
using LitD.System.SerializableTypes;
using Microsoft.Xna.Framework;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LitD.WorldModule.WorldStructure
{
    /// <summary> Самый крупный элемент мира - он содержит в себе набор чанков. </summary>
    [ProtoContract]
    internal class Region
    {
        /// <summary> Координаты региона относительно друг друга. </summary>
        [ProtoMember(1)]
        public SerializableVector2 Position { get; private set; }

        /// <summary> Чанки региона. </summary>
        [ProtoMember(2)]
        private Chunk[] _regionChunks = new Chunk[(int)Math.Pow(WorldConstants.REGION_SIZE, 2)];

        #region Initialize

        /// <summary> Конструктор, создающий новый регион на координатах. </summary>
        /// <param name="position"> Координаты нового региона. </param>
        public Region(Vector2 position)
        {
            Position = position;

            for (int y = 0; y < WorldConstants.REGION_SIZE; y++)
            {
                for (int x = 0; x < WorldConstants.REGION_SIZE; x++)
                {
                    _regionChunks[y * WorldConstants.REGION_SIZE + x] = ChunkGenerator.GenerateChunk(new Vector2(
                        Position.X * WorldConstants.REGION_SIZE +  x,
                        Position.Y * WorldConstants.REGION_SIZE + y
                    ));
                }
            }
        }

        private Region()
        { }

        #endregion

        /// <summary> Возвращает список чанков. </summary>
        /// <returns> Список чанков. </returns>
        public List<Chunk> GetChunks()
        {
            List<Chunk> chunks = new List<Chunk>();
            foreach (var chunk in _regionChunks)
            {
                chunks.Add(chunk);
            }

            return chunks;
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
                _regionChunks[(int)position.Y * WorldConstants.REGION_SIZE + (int)position.X] = chunk;
            }
            catch
            {
                throw new IndexOutOfRangeException("New chunk position is out of region bounds");
            }
        }

        /// <summary> Сериализация региона. </summary>
        public void SaveRegion(string worldDirectory)
        {
            string filePath = GetRegionFilePath(worldDirectory, Position);
            File.Create(filePath).Close();

            using (FileStream file = new FileStream(filePath, FileMode.Open))
            {
                Serializer.Serialize(file, this);
            }
        }

        /// <summary> Преобразует координаты региона в путь к файлу региона. </summary>
        /// <param name="position"> Координаты региона. </param>
        /// <returns> Путь к файлу. </returns>
        public static string GetRegionFilePath(string worldDirectory, Vector2 position)
        {
            string path = string.Empty;

            path += $"{worldDirectory}/";
            path += $"{FolderNameConstants.WorldRegionFolderName}/";
            path += $"{position.X}_{position.Y}.{FileNameConstants.REGION_FILE_EXTENSION}";

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
    }
}
