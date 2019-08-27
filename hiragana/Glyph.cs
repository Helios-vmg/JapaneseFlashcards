using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hiragana
{
    public class Glyph
    {
        public int Id;
        public string Category;
        public List<string> GraphicsPaths;
        private List<Bitmap> _graphics;
        public List<Bitmap> Graphics
        {
            get
            {
                return _graphics ?? (_graphics = GraphicsPaths.Select(x => new Bitmap(x)).ToList());
            }
        }
        public string Audio;
        public string Romanji;
        public string Unicode;
    }

    class GlyphCollection
    {
        public readonly Dictionary<string, List<Glyph>> GlyphsByCategory = new Dictionary<string, List<Glyph>>();

        private char[] Space = {' '};

        public GlyphCollection()
        {
            var csv = new CsvFile("data/data.csv");
            var tempList = new List<Glyph>();
            for (int row = 0; row < csv.RowCount; row++)
            {
                var glyph = new Glyph
                {
                    Id = csv.GetInt("id", row),
                    Category = csv.Get("category", row),
                    GraphicsPaths = csv.Get("graphics", row).Split(Space).Select(x => $"data/glyphs/{x}.png").ToList(),
                    Romanji = csv.Get("romanji", row),
                    Unicode = csv.Get("unicode", row),
                    Audio = $"data/audio/{csv.Get("audio", row)}.wav",
                };

                tempList.Add(glyph);
            }

            tempList.Sort((x, y) => x.Id - y.Id);

            foreach (var glyph in tempList)
            {
                List<Glyph> list;
                if (!GlyphsByCategory.TryGetValue(glyph.Category, out list))
                    GlyphsByCategory[glyph.Category] = list = new List<Glyph>();
                list.Add(glyph);
            }
        }
    }
}
