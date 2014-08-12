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

namespace Base64EncodeDecode
{
    public partial class frmMain : Form
    {
        private List<FileObject> _data = new List<FileObject>();
        private string _inputFilePath = null;
        private int _stringInputTabCount = 0;
        private Encoding _encoding;

        /// <summary>
        /// Initializes a new instance of the <see cref="frmMain"/> class.
        /// </summary>
        public frmMain()
        {
            InitializeComponent();
            ResetForm();
            _inputFilePath = "c:\\";
            _stringInputTabCount = 0;

            txtFileName.AllowDrop = true;
            txtFileName.DragOver += new DragEventHandler(DropFileInControlt_DragOver);
            txtFileName.DragDrop += new DragEventHandler(DropFileInControlt_DragDrop);

            rtbInputText.AllowDrop = true;
            rtbInputText.DragOver += new DragEventHandler(DropFileInControlt_DragOver);
            rtbInputText.DragDrop += new DragEventHandler(DropFileInControlt_DragDrop);

            this.tableLayoutPanel1.CellPaint += tableLayoutPanel1_CellPaint;
        }

        private void DropFileInControlt_DragOver(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.Move;

        }

        private void DropFileInControlt_DragDrop(object sender, DragEventArgs e)
        {
            SetBusy();
            if (e.Data.GetData("FileDrop") != null)
            {
                string[] names = e.Data.GetData("FileDrop") as string[];

                if (sender is TextBox)
                {
                    int fileCount = 0;
                    foreach (string fileName in names)
                    {
                        if (fileCount++ == 0)
                            ((TextBox)sender).Text = fileName;
                        else
                            ((TextBox)sender).Text += ";" + fileName;
                    }
                }
                else if (sender is RichTextBox)
                {
                    if (names.Length > 1)
                    {
                        MessageBox.Show("Control only supports Drag and Drop for 1 file at a time.", "Failed to Drag and Drop File");
                    }
                    else
                    {
                        try
                        {
                            using (Stream stream = File.Open(names[0], FileMode.Open, FileAccess.Read))
                            {
                                byte[] data = new byte[(int)stream.Length];
                                stream.Read(data, 0, (int)stream.Length);
                                string dataString = GetString(data);
                                if (dataString.Contains("\0\0"))
                                {
                                    MessageBox.Show("The decoded data appears to be binary and can not be displayed as text.  File name has been placed in the file name entry section instead.", "Binary Data Detected", MessageBoxButtons.OK);
                                    txtFileName.Text = names[0];
                                    rdoFileFormat.Checked = true;
                                }
                                else
                                {
                                    ((RichTextBox)sender).Text = dataString;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message, "Error");
                        }
                    }
                }
            }
            SetNotBusy();
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            SetBusy();
            try
            {
                _data.Clear();
                OpenFileDialog openFileDialog1 = new OpenFileDialog();

                openFileDialog1.InitialDirectory = _inputFilePath;
                openFileDialog1.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    int fileCount = 0;
                    foreach (string file in openFileDialog1.FileNames)
                    {
                        if (fileCount++ == 0)
                            txtFileName.Text = file;
                        else
                            txtFileName.Text += ";" + file;
                    }

                    _inputFilePath = Path.GetDirectoryName(openFileDialog1.FileNames[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
            SetNotBusy();
        }

        private void rdoFileFormat_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoFileFormat.Checked)
            {
                txtFileName.Enabled = true;
                btnOpenFile.Enabled = true;
                btnPasteFromClipboard.Enabled = false;
                rtbInputText.Enabled = false;
                rdoEncNone.Enabled = false;
                rdoEncAscii.Enabled = false;
                rdoEncUTF8.Enabled = false;
                rdoEncUTF7.Enabled = false;
                rdoEncUTF32.Enabled = false;
                rdoEncUnicode.Enabled = false;
                rdoEncBigEndianUnicode.Enabled = false;
                lblEncDesc.Enabled = false;
            }
        }

        private void rdoTextString_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoTextString.Checked)
            {
                txtFileName.Enabled = false;
                btnOpenFile.Enabled = false;
                btnPasteFromClipboard.Enabled = true;
                rtbInputText.Enabled = true;
                rdoEncNone.Enabled = true;
                rdoEncAscii.Enabled = true;
                rdoEncUTF8.Enabled = true;
                rdoEncUTF7.Enabled = true;
                rdoEncUTF32.Enabled = true;
                rdoEncUnicode.Enabled = true;
                rdoEncBigEndianUnicode.Enabled = true;
                lblEncDesc.Enabled = true;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _data.Clear();

            Application.Exit();
        }

        private void btnEncode_Click(object sender, EventArgs e)
        {
            SetBusy();
            try
            {
                if (rdoFileFormat.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtFileName.Text))
                    {
                        MessageBox.Show("Please select a file to Encode.");
                    }
                    else
                    {
                        LoadFileObjects();
                        if (_data.Count == 1)
                        {
                            AddTab("Results for " + _data[0].FileName, _data[0], EncodeFile(_data[0].FileData));
                        }
                        else if (_data.Count > 1)
                        {
                            foreach (FileObject data in _data)
                            {
                                try
                                {
                                    AddTab("Results for " + data.FileName, data, EncodeFile(data.FileData));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error: " + ex.Message, "Error");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(rtbInputText.Text))
                    {
                        MessageBox.Show("Please provide a string to Encode.");
                    }
                    else
                    {
                        byte[] data = GetBytes(rtbInputText.Text);
                        string dataString = EncodeFile(data);
                        string fileName = (_inputFilePath.EndsWith("\\")) ? string.Concat(_inputFilePath, "base64.tmp") : string.Concat(_inputFilePath, "\\base64.tmp");
                        FileObject fileData = new FileObject() { FileName = fileName, FileData = GetBytes(dataString) };
                        AddTab(string.Concat("Results for String input (", ++_stringInputTabCount, ")"), fileData, dataString);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
            SetNotBusy();
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            SetBusy();
            try
            {
                if (rdoFileFormat.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtFileName.Text))
                    {
                        MessageBox.Show("Please select a file to Decode.");
                    }
                    else
                    {
                        LoadFileObjects();
                        if (_data.Count == 1)
                        {
                            AddTab("Results for " + _data[0].FileName, _data[0], DecodeFile(_data[0].FileData));
                        }
                        else if (_data.Count > 1)
                        {
                            foreach (FileObject data in _data)
                            {
                                try
                                {
                                    AddTab("Results for " + data.FileName, data, DecodeFile(data.FileData));
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Error: " + ex.Message, "Error");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(rtbInputText.Text))
                    {
                        MessageBox.Show("Please provide a string to Decode.");
                    }
                    else
                    {
                        byte[] data = GetBytes(rtbInputText.Text);
                        string dataString = DecodeFile(data);
                        if (dataString.Contains("\0\0"))
                        {
                            MessageBox.Show("The decoded data appears to be binary and can not be displayed as text.  Please continue and save the result as a file.", "Binary Data Detected", MessageBoxButtons.OK);

                            SaveFileDialog saveFileDialog1 = new SaveFileDialog()
                            {
                                InitialDirectory = _inputFilePath,
                                DefaultExt = ".tmp",
                                FileName = "base64",
                                FilterIndex = 1,
                                Filter = "All Files|*.*"
                            };

                            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                            {
                                File.WriteAllBytes(saveFileDialog1.FileName, GetBytes(dataString));
                                MessageBox.Show("Results saved as file successfully!", "File Saved");
                            }
                        }
                        else
                        {
                            string fileName = (_inputFilePath.EndsWith("\\")) ? string.Concat(_inputFilePath, "base64.tmp") : string.Concat(_inputFilePath, "\\base64.tmp");
                            FileObject fileData = new FileObject() { FileName = fileName, FileData = GetBytes(dataString) };
                            AddTab(string.Concat("Results for String input (", ++_stringInputTabCount, ")"), fileData, dataString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error");
            }
            SetNotBusy();
        }

        private void ResetForm()
        {
            rdoFileFormat.Checked = true;
            rdoEncNone.Checked = true;
            txtFileName.Text = "";
            rtbInputText.Text = "";
            btnPasteFromClipboard.Enabled = false;
            _data.Clear();
        }

        private string EncodeFile(byte[] data)
        {
            try
            {
                return Convert.ToBase64String(data);
            }
            catch (Exception ex)
            {
                return "Input is not in a format that is valid for selected Action.  Original error: " + ex.Message;
            }
        }

        private string DecodeFile(byte [] data)
        {
            try
            {
                string dataString = GetString(data);

                //this will fail to convert if data was not a base64 string
                byte[] b = Convert.FromBase64String(dataString);
                return GetString(b);
            }
            catch (Exception ex)
            {
                return "Input is not in a format that is valid for selected Action.  Original error: " + ex.Message;
            }
        }

        private void LoadFileObjects()
        {
            _data.Clear();
            if (!string.IsNullOrWhiteSpace(txtFileName.Text))
            {
                int fileCount = 0;
                string[] files = txtFileName.Text.Split(';');
                foreach (string file in files)
                {
                    try
                    {
                        if (fileCount++ == 0)
                            txtFileName.Text = file;
                        else
                            txtFileName.Text += "; " + file;

                        using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                        {
                            byte[] data = new byte[(int)stream.Length];
                            stream.Read(data, 0, (int)stream.Length);
                            _data.Add(new FileObject() { FileName = file.Trim(), FileData = data });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message, "Error");
                    }
                }

                _inputFilePath = Path.GetDirectoryName(files[0]);
            }
        }

        public void AddTab(string tabName, FileObject data, string results)
        {
            TabPage tbp = new TabPage();
            tbp.Name = "tab" + tabForms.TabPages.Count.ToString();
            tbp.Text = tabName;
            ResultsControl cnt = new ResultsControl(data, results, ref tabForms);
            cnt.Name = tbp.Name + "_cnt";
            cnt.Dock = DockStyle.Fill;
            tbp.Controls.Add(cnt);
            tabForms.TabPages.Add(tbp);
            cnt.Show();
            cnt.Select();
        }

        private int GetStringInputTabCount()
        {
            foreach (TabPage page in tabForms.TabPages)
            {
                if (page.Text.StartsWith("Results for String input ("))
                {
                    _stringInputTabCount++;
                }
            }

            return _stringInputTabCount;
        }

        private byte[] GetBytes(string str)
        {
            if (_encoding == Encoding.ASCII)
                return Encoding.ASCII.GetBytes(str);
            else if (_encoding == Encoding.UTF8)
                return Encoding.UTF8.GetBytes(str);
            else if (_encoding == Encoding.UTF7)
                return Encoding.UTF7.GetBytes(str);
            else if (_encoding == Encoding.UTF32)
                return Encoding.UTF32.GetBytes(str);
            else if (_encoding == Encoding.Unicode)
                return Encoding.Unicode.GetBytes(str);
            else if (_encoding == Encoding.BigEndianUnicode)
                return Encoding.BigEndianUnicode.GetBytes(str);
            else
            {
                byte[] bytes = new byte[str.Length * sizeof(char)];
                System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
                return bytes;
            }
        }

        private string GetString(byte[] bytes)
        {
            if (_encoding == Encoding.ASCII)
                return Encoding.ASCII.GetString(bytes);
            else if (_encoding == Encoding.UTF8)
                return Encoding.UTF8.GetString(bytes);
            else if (_encoding == Encoding.UTF7)
                return Encoding.UTF7.GetString(bytes);
            else if (_encoding == Encoding.UTF32)
                return Encoding.UTF32.GetString(bytes);
            else if (_encoding == Encoding.Unicode)
                return Encoding.Unicode.GetString(bytes);
            else if (_encoding == Encoding.BigEndianUnicode)
                return Encoding.BigEndianUnicode.GetString(bytes);
            else
            {
                char[] chars = new char[bytes.Length / sizeof(char)];
                System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
                return new string(chars);
            }
        }

        private void btnPasteFromClipboard_Click(object sender, EventArgs e)
        {
            rtbInputText.Text = Clipboard.GetText();
        }

        private void tsmiExit_Click(object sender, EventArgs e)
        {
            _data.Clear();

            Application.Exit();
        }

        private void tsmiClearTabs_Click(object sender, EventArgs e)
        {
            _stringInputTabCount = 0;
            foreach (TabPage page in tabForms.TabPages)
            {
                if (page.Name != "mainTabPage")
                    tabForms.TabPages.Remove(page);
            }
        }

        private void SetBusy()
        {
            this.Cursor = Cursors.WaitCursor;
        }

        private void SetNotBusy()
        {
            this.Cursor = Cursors.Default;
        }

        private void rdoEncNone_CheckedChanged(object sender, EventArgs e)
        {
             _encoding = null;
        }

        private void rdoEncAscii_CheckedChanged(object sender, EventArgs e)
        {
            _encoding = Encoding.ASCII;
        }

        private void rdoEncUTF8_CheckedChanged(object sender, EventArgs e)
        {
            _encoding = Encoding.UTF8;
        }

        private void rdoEncUTF7_CheckedChanged(object sender, EventArgs e)
        {
            _encoding = Encoding.UTF7;
        }

        private void rdoEncUTF32_CheckedChanged(object sender, EventArgs e)
        {
            _encoding = Encoding.UTF32;
        }

        private void rdoEncUnicode_CheckedChanged(object sender, EventArgs e)
        {
            _encoding = Encoding.Unicode;
        }

        private void rdoEncBigEndianUnicode_CheckedChanged(object sender, EventArgs e)
        {
            _encoding = Encoding.BigEndianUnicode;
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            if (e.Row == 0 || e.Row == 3 || e.Row == 7)
                e.Graphics.DrawLine(Pens.Black, e.CellBounds.Location, new Point(e.CellBounds.Right, e.CellBounds.Top));

            if (e.Row == 7)
                e.Graphics.DrawLine(Pens.Black, new Point(e.CellBounds.Left, e.CellBounds.Bottom), new Point(e.CellBounds.Right, e.CellBounds.Bottom));
        }
    }
}
