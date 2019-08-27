using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace hiragana
{
    public partial class SpeechFromSymbolForm : Form
    {
        Random _rng = new Random();
        private List<Glyph> _glyphs;
        private Glyph _current;
        private List<PictureBox> _toDispose = new List<PictureBox>();

        public SpeechFromSymbolForm(List<Glyph> selectedGlyphs)
        {
            InitializeComponent();

            _glyphs = selectedGlyphs;
            SelectNextGlyph();
        }

        private void SelectNextGlyph()
        {
            var index = _rng.Next(0, _glyphs.Count - 1);
            _current = _glyphs[index];
            ImageTable.ColumnStyles.Clear();
            ImageTable.ColumnCount = 0;
            foreach (var pictureBox in _toDispose)
                pictureBox.Dispose();
            _toDispose.Clear();
            for (int i = 0; i < _current.Graphics.Count; i++)
            {
                ImageTable.ColumnCount = i + 1;
                var display = new PictureBox();
                display.AutoSize = true;
                display.Image = _current.Graphics[i];
                display.Parent = ImageTable;
                display.Margin = new Padding(0);
                _toDispose.Add(display);
                ImageTable.SetCellPosition(display, new TableLayoutPanelCellPosition(i, 0));
                ImageTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize, 0));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SelectNextGlyph();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var simpleSound = new SoundPlayer(_current.Audio);
            simpleSound.Play();
        }
    }
}
