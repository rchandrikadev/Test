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
    /// <summary>
    /// User Control for displaying text results
    /// </summary>
    public partial class ResultsControl : UserControl
    {
        FileObject _data;
        TabControl _tabControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsControl"/> class.
        /// </summary>
        public ResultsControl()
        {
            InitializeComponent();
        }

        #region Public Methods
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultsControl"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="results">The results.</param>
        /// <param name="tabControl">The tab control.</param>
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

        /// <summary>
        /// Removes the tab.
        /// </summary>
        public void RemoveTab()
        {
            _data = null;
            _tabControl.TabPages.Remove((TabPage)this.Parent);
            this.Dispose();
        }
        #endregion Public Methods

        #region Private Methods

        #endregion Private Methods

        #region Event Handlers
        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            RemoveTab();
        }

        /// <summary>
        /// Handles the Click event of the btnSaveResultAsFile control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
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

            //Test Hot Fix and Development Branch Test

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveFileDialog1.FileName, _data.FileData);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCopy control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(rtbResults.Text);
        }
        #endregion Event Handlers
    }
}
