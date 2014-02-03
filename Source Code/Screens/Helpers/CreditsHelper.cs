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
    public static class CreditsHelper
    {
        public static string CreditsFilename = "LocalStorage2.ebd";

        public static void Initialize()
        {
            //todo2

            //string fullpath = Path.Combine(StorageContainer.TitleLocation, CreditsFilename);

            //if (!File.Exists(fullpath))
            //{
            //    CreditData data = new CreditData(8);
            //    data.Contributer[0] = "";
            //    data.Role[0] = "@Manager";

            //    data.Contributer[1] = "Luke";
            //    data.Role[1] = "";

            //    data.Contributer[2] = "";
            //    data.Role[2] = "@Programmers & Design";

            //    data.Contributer[3] = "Dal, Yani";
            //    data.Role[3] = "";

            //    data.Contributer[4] = "";
            //    data.Role[4] = "@Artists";

            //    data.Contributer[5] = "Nomis, Kevin, 1nfinitum";
            //    data.Role[5] = "";

            //    data.Contributer[6] = "";
            //    data.Role[6] = "@Sound Effects & Music";

            //    data.Contributer[7] = "Joe";
            //    data.Role[7] = "";

            //    SaveCredits(data, CreditsFilename);
            //}
        }

        private static void SaveCredits(CreditData data, string filename)
        {
            //todo2

            //// Get the path of the save game
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //// Open the file, creating it if necessary
            //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate);
            //try
            //{
            //    // Convert the object to XML data and put it in the stream
            //    XmlSerializer serializer = new XmlSerializer(typeof(CreditData));
            //    serializer.Serialize(stream, data);
            //}
            //finally
            //{
            //    // Close the file
            //    stream.Close();
            //}
        }

        public static CreditData LoadCredits(string filename)
        {
            //todo2
            return new CreditData();

            //CreditData data;

            //// Get the path of the save game
            //string fullpath = Path.Combine(StorageContainer.TitleLocation, filename);

            //// Open the file
            //FileStream stream = File.Open(fullpath, FileMode.OpenOrCreate, FileAccess.Read);
            //try
            //{

            //    // Read the data from the file
            //    XmlSerializer serializer = new XmlSerializer(typeof(CreditData));
            //    data = (CreditData)serializer.Deserialize(stream);
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
