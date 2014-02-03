using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using EnigmaBreaker.Model;

namespace EnigmaBreaker.Screens.Helpers
{
    public static class HighScoresHelper
    {
        public static string HighScoresFilename = "LocalStorage.ebd";

        public static void Initialize()
        {
            //todo2
            string fullpath = "";
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, HighScoresFilename);

            if (!File.Exists(fullpath))
            {
                HighScoreData data = new HighScoreData(11);
                data.PlayerName[0] = "Yani";
                data.Level[0] = 1;
                data.Score[0] = 667;

                data.PlayerName[1] = "Dal";
                data.Level[1] = 1;
                data.Score[1] = 666;

                data.PlayerName[2] = "Luke";
                data.Level[2] = 1;
                data.Score[2] = 71;

                data.PlayerName[3] = "Nomis";
                data.Level[3] = 1;
                data.Score[3] = 69;

                data.PlayerName[4] = "Joe";
                data.Level[4] = 1;
                data.Score[4] = 51;

                data.PlayerName[5] = "Kevin";
                data.Level[5] = 1;
                data.Score[5] = 50;

                data.PlayerName[6] = "1nfinitum";
                data.Level[6] = 1;
                data.Score[6] = 49;

                data.PlayerName[7] = "Widi";
                data.Level[7] = 1;
                data.Score[7] = 40;

                data.PlayerName[8] = "Jmadrigal";
                data.Level[8] = 1;
                data.Score[8] = 36;

                data.PlayerName[9] = "Shelby";
                data.Level[9] = 1;
                data.Score[9] = 13;

                data.PlayerName[10] = "DoMaggio";
                data.Level[10] = 1;
                data.Score[10] = 7;

                SaveHighScores(data, HighScoresFilename);
            }
        }

        private static void SaveHighScores(HighScoreData data, string filename)
        {
            //todo2
            return;
            //// Get the path of the save game
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //// Open the file, creating it if necessary
            //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            //try
            //{
            //    // Convert the object to XML data and put it in the stream
            //    XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
            //    serializer.Serialize(stream, data);
            //}
            //finally
            //{
            //    // Close the file
            //    stream.Close();
            //}
        }

        public static void SaveHighScore(int score, string playerName, int currentLevel)
        {
            // Create the data to save
            HighScoreData data = LoadHighScores(HighScoresFilename);

            int scoreIndex = -1;
            for (int i = 0; i < data.Count; i++)
            {
                if (score > data.Score[i])
                {
                    scoreIndex = i;
                    break;
                }
            }

            if (scoreIndex > -1)
            {
                //New high score found ... do swaps
                for (int i = data.Count - 1; i > scoreIndex; i--)
                {
                    data.PlayerName[i] = data.PlayerName[i - 1];
                    data.Score[i] = data.Score[i - 1];
                    data.Level[i] = data.Level[i - 1];
                }

                data.PlayerName[scoreIndex] = playerName; //Retrieve User Name Here
                data.Score[scoreIndex] = score;
                data.Level[scoreIndex] = currentLevel;

                ClearFileContent(HighScoresFilename);
                SaveHighScores(data, HighScoresFilename);
            }
        }

        public static HighScoreData LoadHighScores(string filename)
        {
            //todo2
            return new HighScoreData();

            //HighScoreData data;

            //// Get the path of the save game
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //if (!File.Exists(fullpath))
            //{
            //    Initialize();
            //}

            //// Open the file
            //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            //try
            //{
            //    // Read the data from the file
            //    XmlSerializer serializer = new XmlSerializer(typeof(HighScoreData));
            //    data = (HighScoreData)serializer.Deserialize(stream);
            //}
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
