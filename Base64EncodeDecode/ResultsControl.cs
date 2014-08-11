using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Base64EncodeDecode
{
    public partial class ResultsControl : UserControl
    {
        FileObject _data;
        TabControl _tabControl;

        public ResultsControl()
        {
            InitializeComponent();
        }

        public ResultsControl(FileObject data, string results, ref TabControl tabControl)
        {
            InitializeComponent();
            _data = data;
            _tabControl = tabControl;
            this.Text = "Results for " + _data.FileName;
            rtbResults.Text = results;
            if (!results.StartsWith("Input is not in a format that is valid for selected Action."))
                btnSaveResultAsFile.Enabled = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _tabControl.TabPages.Remove((TabPage)this.Parent);
        }

        private void btnSaveResultAsFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
            {
                InitialDirectory = Path.GetDirectoryName(_data.FileName),
                DefaultExt = ".tmp",
                FileName = "base64",
                FilterIndex = 1,
                Filter = "All Files|*.*"
            };

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, _data.FileData);
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbResults.Text);
        }
    }
}
