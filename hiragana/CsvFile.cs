using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hiragana
{
    class CsvFile
    {
        public int RowCount { get; private set; }
        private readonly Dictionary<string, int> _header;
        private readonly List<string> _data; 

        public CsvFile(string path)
        {
            using (var file = new StreamReader(path, Encoding.UTF8))
            {
                var headerLine = file.ReadLine();
                int i = 0;
                _header = new Dictionary<string, int>();
                foreach (var s in SplitCsvLine(headerLine))
                    _header[s] = i++;

                _data = new List<string>();
                RowCount = 0;
                while (true)
                {
                    var line = file.ReadLine();
                    if (string.IsNullOrEmpty(line))
                        break;
                    var split = SplitCsvLine(line);
                    if (split.Count != _header.Count)
                        InvalidCsvFile();
                    _data.AddRange(split);
                    RowCount++;
                }
            }
        }

        private static void InvalidCsvFile()
        {
            throw new Exception("Invalid CSV file");
        }

        private static List<string> SplitCsvLine(string line)
        {
            var ret = new List<string>();
            StringBuilder accum = null;
            int state = 0;
            foreach (var c in line)
            {
                switch (state)
                {
                    case -1:
                        InvalidCsvFile();
                        break;
                    case 0:
                        if (c != '"')
                            state = -1;
                        else
                        {
                            accum = new StringBuilder();
                            state = 1;
                        }
                        continue;
                    case 1:
                        if (c == '"')
                        {
                            ret.Add(accum.ToString());
                            state = 2;
                        }
                        else
                            accum.Append(c);
                        continue;
                    case 2:
                        if (c != ',')
                            state = -1;
                        else
                            state = 0;
                        break;
                    default:
                        throw new Exception();
                }
            }
            if (state != 2)
                InvalidCsvFile();
            return ret;
        }

        public string Get(string column, int row)
        {
            if (row < 0 || row >= RowCount)
                throw new IndexOutOfRangeException();
            int columnIndex;
            if (!_header.TryGetValue(column, out columnIndex))
                throw new KeyNotFoundException();
            return _data[columnIndex + row*_header.Count];
        }

        public int GetInt(string column, int row)
        {
            return Convert.ToInt32(Get(column, row));
        }
    }
}
