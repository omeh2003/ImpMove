using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ImpMove.Core
{
    static class FilesMethod
    {

        public static void SaveIdList(string filein, List<ImpPoint> listin)
        {
            var file = filein;
            var list = listin;

            using (var f = File.CreateText(file))
            {
                var writer = new XmlSerializer(list.GetType());

                writer.Serialize(f, list);
            }
        }

        public static List<ImpPoint> ReadIdList(string filein)
        {
            var file = filein;
            if (!File.Exists(file)) return null;
            var l = new List<ImpPoint>();
            using (var f = new StreamReader(file))
            {
                var reader = new XmlSerializer(l.GetType());

                l = (List<ImpPoint>)reader.Deserialize(f);
            }

            return l;
        }

    }
}
