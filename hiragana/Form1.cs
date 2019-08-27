using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hiragana
{
    public partial class Form1 : Form
    {
        private readonly string _configPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/hiragana.config";
        GlyphCollection _collection = new GlyphCollection();
        List<Tuple<List<Glyph>, CheckBox>> _checkBoxes = new List<Tuple<List<Glyph>, CheckBox>>();

        public Form1()
        {
            InitializeComponent();

            var settings = LoadSettings();

            foreach (var kv in _collection.GlyphsByCategory)
            {
                var checkbox = new CheckBox();
                checkbox.Text = kv.Key;
                checkbox.Parent = layout;
                checkbox.AutoSize = true;
                checkbox.MinimumSize = new Size(64, 0);
                checkbox.Font = new Font(checkbox.Font.FontFamily, 15);
                if (settings.Contains(kv.Key))
                    checkbox.Checked = true;
                _checkBoxes.Add(new Tuple<List<Glyph>, CheckBox>(kv.Value, checkbox));
            }
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            var selectedGlyphs = new List<Glyph>();
            foreach (var glyphs in _checkBoxes.Where(x => x.Item2.Checked).Select(x => x.Item1))
                selectedGlyphs.AddRange(glyphs);
            if (selectedGlyphs.Count == 0)
            {
                MessageBox.Show("Por favor seleccione al menos una categoría.", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            SaveSettings(_checkBoxes.Where(x => x.Item2.Checked).Select(x => x.Item1[0].Category));
            Form newDialog;
            if (!SymbolFromSpeech.Checked)
                newDialog = new SpeechFromSymbolForm(selectedGlyphs);
            else
                newDialog = new SymbolFromSpeechForm(selectedGlyphs);
            newDialog.Show(this);
        }

        private static char[] Comma = {','};

        private HashSet<string> LoadSettings()
        {
            var ret = new HashSet<string>();
            try
            {
                using (var file = new StreamReader(_configPath, Encoding.UTF8))
                {
                    foreach (var s in file.ReadToEnd().Split(Comma, StringSplitOptions.RemoveEmptyEntries))
                        ret.Add(s);
                }
            }
            catch (IOException)
            {
            }
            return ret;
        }

        private void SaveSettings(IEnumerable<string> settings)
        {
            using (var file = new FileStream(_configPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var buffer = Encoding.UTF8.GetBytes(settings.Aggregate((x, y) => x + "," + y));
                file.Write(buffer, 0, buffer.Length);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var checkBox in _checkBoxes)
                checkBox.Item2.Checked = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (var checkBox in _checkBoxes)
                checkBox.Item2.Checked = false;
        }
    }
}
