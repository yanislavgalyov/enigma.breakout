using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using EnigmaBreaker.Model;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace EnigmaBreaker.Screens.Helpers
{
    public static class OptionsHelper
    {
        public static string OptionsFilename = "OptionsStorage.ebd";

        public static void Initialize()
        {
            //todo2
            throw new NotImplementedException();
            //var s = TitleContainer.OpenStream(filename);

            //string fullpath = Path.Combine(StorageContainer.TitleLocation, OptionsFilename);

            //if (!File.Exists(fullpath))
            //{
            //    OptionData data = new OptionData(true);

            //    SaveOptions(data, OptionsFilename);
            //}
        }

        private static void SaveOptions(OptionData data, string filename)
        {
            //todo2
            throw new NotImplementedException();
            //var s = TitleContainer.OpenStream(filename);
            
            //// Get the path of the save game
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //// Open the file, creating it if necessary
            //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            //try
            //{
            //    // Convert the object to XML data and put it in the stream
            //    XmlSerializer serializer = new XmlSerializer(typeof(OptionData));
            //    serializer.Serialize(stream, data);
            //}
            //finally
            //{
            //    // Close the file
            //    stream.Close();
            //}
        }

        public static void SaveOptions(string resolution)
        {
            try
            {
                OptionData data = LoadOptions(OptionsFilename);
                ClearFileContent(OptionsFilename);

                data.GameResolution = resolution;
                SaveOptions(data, OptionsFilename);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                OptionData data = new OptionData(true);
                data.GameResolution = resolution;
                SaveOptions(data, OptionsFilename);
            }
        }

        public static void SaveOptions(int volume)
        {
            try
            {
                OptionData data = LoadOptions(OptionsFilename);
                ClearFileContent(OptionsFilename);

                data.SoundVolume = volume;
                SaveOptions(data, OptionsFilename);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.InnerException);
                OptionData data = new OptionData(true);
                data.SoundVolume = volume;
                SaveOptions(data, OptionsFilename);
            }
        }

        public static void SaveOptions(string resolution, int volume)
        {
            // Create the data to save
            OptionData data = LoadOptions(OptionsFilename);
            data.GameResolution = resolution;
            data.SoundVolume = volume;

            SaveOptions(data, OptionsFilename);
        }

        public static OptionData LoadOptions(string filename)
        {
            //todo2
            return new OptionData();

            //OptionData data;

            //// Get the path of the save game
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //// Open the file
            //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            //try
            //{

            //    // Read the data from the file
            //    XmlSerializer serializer = new XmlSerializer(typeof(OptionData));
            //    data = (OptionData)serializer.Deserialize(stream);
            //}
            ////catch (Exception e)
            ////{
            ////    Debug.WriteLine(e.InnerException);
            ////    //data = new OptionData(true);
            ////}
            //finally
            //{
            //    // Close the file
            //    stream.Close();
            //}

            //return (data);
        }

        public static void ClearFileContent(string filename)
        {
            //todo2

            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //using (StreamWriter sw = new StreamWriter(fullpath))
            //{
            //    sw.Write("");
            //}
        }
    }
}
