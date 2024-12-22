using System;
using System.Text.RegularExpressions;
using System.IO;
using ProtoBuf;
using LitD.WorldModule.WorldStructure;
using LitD.System.Constants;

namespace LitD.WorldModule
{
    /// <summary> Отвечает за загрузку и создание миров. </summary>
    internal class WorldLoader
    {
        /// <summary> Создает новый мир. </summary>
        /// <returns> Путь к файлу созданного мира. </returns>
        public static string CreateNew()
        {
            try
            {
                Regex forbiddenChars = new Regex("[/:]");
                string worldDirectory = $"Saves/{forbiddenChars.Replace(DateTime.Now.ToString(), "_")}";
                string worldFile = $"{worldDirectory}/{FileNameConstants.WORLD_FILE_NAME}.{FileNameConstants.WORLD_FILE_EXTENSION}";

                Directory.CreateDirectory(worldDirectory);
                File.Create(worldFile).Close();
                Directory.CreateDirectory(Path.Combine(worldDirectory, FolderNameConstants.WorldRegionFolderName));

                World world = new World("test world", worldDirectory);

                using (FileStream writer = new FileStream(worldFile, FileMode.Open))
                {
                    Serializer.Serialize<World>(writer, world);
                }

                return worldDirectory;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to create new world!\n{e}");
            }
        }

        /// <summary> Загрузка существующего мира из файла. </summary>
        public static void LoadWorld(string worldDirectory, out World world)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(worldDirectory, $"{FileNameConstants.WORLD_FILE_NAME}.{FileNameConstants.WORLD_FILE_EXTENSION}"), FileMode.Open))
            {
                world = Serializer.Deserialize<World>(fileStream);
            }
        }
    }
}
