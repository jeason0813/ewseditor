﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using EWSEditor.Common.Exports;
using Microsoft.Exchange.WebServices.Data;


namespace EWSEditor.Forms.Searches
{
    public partial class SearchExportPicker : Form
    {
        public bool bChoseOk = false;
        public List<EWSEditor.Common.Exports.AdditionalPropertyDefinition> AdditionalPropertyDefinitions = null;
        public List<ExtendedPropertyDefinition> ExtendedPropertyDefinitions = null;
        //public CsvStringHandling StringHandling = CsvStringHandling.Base64encode;
        public CsvExportOptions ExportOptions = new CsvExportOptions();

        public SearchExportPicker()
        {
            InitializeComponent();
        }

        private void btnPickFolderDisplayedResults_Click(object sender, EventArgs e)
        {
            txtDisplayedResultsFolderPath.Text = ChooseFilePath(txtDisplayedResultsFolderPath.Text.Trim(), "CSV");
        }

        private void btnPickFolderIncludeUsersAdditionalProperties_Click(object sender, EventArgs e)
        {

            List<AdditionalPropertyDefinition> oAPD = null;
            List<ExtendedPropertyDefinition> oEPD = null;

            string sChosenFile = string.Empty;

            sChosenFile = txtIncludeUsersAdditionalPropertiesFile.Text;

            if (AdditionalProperties.GetAdditionalPropertiesFromCsv(ref sChosenFile, ref oAPD, ref oEPD))
            {
                AdditionalPropertyDefinitions = oAPD;
                ExtendedPropertyDefinitions = oEPD;

                this.txtIncludeUsersAdditionalPropertiesFile.Text = sChosenFile;
            }

            txtIncludeUsersAdditionalPropertiesFile.Enabled = false;
        }

        private void SearchExportPicker_Load(object sender, EventArgs e)
        {
            string StartFolder = Application.StartupPath + "";
            this.txtDisplayedResultsFolderPath.Text = StartFolder + "\\Export\\ExportedSearchItemsResults.CSV";
             //this.txtDiagnosticExportFolderPath.Text = StartFolder + "\\Export\\ExportedDiagnosticMeetingMessageResults.CSV";
            this.txtBlobFolderPath.Text = Application.StartupPath + "\\Export\\";
            this.txtIncludeUsersAdditionalPropertiesFile.Text = Application.StartupPath + "\\AdditionalPropertiesExamples\\";

            SetEnablement();
        }

        private void SetEnablement()
        {
 

            txtDisplayedResultsFolderPath.Enabled = rdoExportDisplayedResults.Checked;
            btnPickFolderDisplayedResults.Enabled = rdoExportDisplayedResults.Checked;

            txtBlobFolderPath.Enabled = rdoExportItemsAsBlobs.Checked;
            btnPickFolderBlobProperties.Enabled = rdoExportItemsAsBlobs.Checked;

            if (this.rdoExportDisplayedResults.Checked == true)
            {
                SetEnablement_ExportOptions(true);
            }

 

            if (this.rdoExportItemsAsBlobs.Checked == true)
            {
                SetEnablement_ExportOptions(false);
            }

 
        }

        private void SetEnablement_ExportOptions(bool isEnabled)
        {

            if (this.rdoExportItemsAsBlobs.Checked == true)
            {
                this.chkIncludeUsersAdditionalProperties.Enabled = false;
                txtIncludeUsersAdditionalPropertiesFile.Enabled = false;
                btnPickFolderIncludeUsersAdditionalProperties.Enabled = false;
            }
            else
            {
                // Note: this.rdoExportItemsAsBlobs.Checked == false

                this.chkIncludeUsersAdditionalProperties.Enabled = true;
                this.txtIncludeUsersAdditionalPropertiesFile.Enabled = this.chkIncludeUsersAdditionalProperties.Checked;
                this.btnPickFolderIncludeUsersAdditionalProperties.Enabled = this.chkIncludeUsersAdditionalProperties.Checked;
            }

 

            rdoSanitizeStrings.Enabled = isEnabled;
            rdoHexEncodeStrings.Enabled = isEnabled;
            rdoBase64EncodeStrings.Enabled = isEnabled;
            rdoNone.Enabled = isEnabled;

 
            rdoExportAllGridData.Enabled = isEnabled;
            rdoExcludeAllGridContentExceptFolderPath.Enabled = isEnabled;
            rdoExcludeAllSearchGridContent.Enabled = isEnabled;
 
            chkConvertBase64BinaryHex.Enabled = isEnabled;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }


        private string ChooseFilePath(string sFullPath, string sExtensionType)
        {
            //string sFolderPath = string.Empty;
            string sNewFullPath = string.Empty;

            System.Windows.Forms.SaveFileDialog oFD = new System.Windows.Forms.SaveFileDialog();

            sNewFullPath = sFullPath;

            oFD.InitialDirectory = Path.GetDirectoryName(sFullPath);
            oFD.FileName = sFullPath;

            oFD.CheckPathExists = true;
            if (sExtensionType.ToUpper() == "CSV")
            {
                oFD.DefaultExt = "csv";
                oFD.Filter = "csv files (*.csv)|*.csv";
                oFD.FilterIndex = 1;
                oFD.Title = "Save item as csv";
            }

            if (sExtensionType.ToUpper() == "BIN")
            {
                oFD.DefaultExt = "bin";
                oFD.Filter = "bin files (*.bin)|*.bin";
                oFD.FilterIndex = 1;
                oFD.Title = "Save item as bin";
            }

            if (oFD.ShowDialog() == DialogResult.OK)
            {
                sNewFullPath = oFD.FileName;

            }
            return sNewFullPath;
        }

        private void rdoExportDisplayedResults_CheckedChanged(object sender, EventArgs e)
        {
            SetEnablement();
        }

        private void rdoExportItemsAsBlobs_CheckedChanged(object sender, EventArgs e)
        {
            SetEnablement();
        }

        private void btnPickFolderBlobProperties_Click(object sender, EventArgs e)
        {
            string sFolderPath = string.Empty;
            System.Windows.Forms.FolderBrowserDialog oFDB = new FolderBrowserDialog();


            oFDB.SelectedPath = txtBlobFolderPath.Text;
            if (oFDB.ShowDialog() == DialogResult.OK)
            {
                sFolderPath = oFDB.SelectedPath;

                txtBlobFolderPath.Text = sFolderPath;
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            bool bAllowOK = true;

            if (rdoExportDisplayedResults.Checked)
            {
                if (File.Exists(txtDisplayedResultsFolderPath.Text.Trim()))
                {
                    MessageBox.Show("Diplayed Results Export File already exists", "File already exists.  Choose a different file name.");
                    bAllowOK = false;
                }
            }

            // Export Options
            if (rdoBase64EncodeStrings.Checked == true)
                ExportOptions._CsvStringHandling = CsvStringHandling.Base64encode;
            if (this.rdoHexEncodeStrings.Checked == true)
                ExportOptions._CsvStringHandling = CsvStringHandling.HexEncode;
            if (rdoSanitizeStrings.Checked == true)
                ExportOptions._CsvStringHandling = CsvStringHandling.SanitizeStrings;
            if (rdoNone.Checked == true)
                ExportOptions._CsvStringHandling = CsvStringHandling.None;

            if (rdoExportAllGridData.Checked == true)
                ExportOptions._CsvExportGridExclusions = CsvExportGridExclusions.ExportAll;
            if (rdoExcludeAllGridContentExceptFolderPath.Checked == true)
                ExportOptions._CsvExportGridExclusions = CsvExportGridExclusions.ExcludeAllInGridExceptFilePath;
            if (rdoExcludeAllSearchGridContent.Checked == true)
                ExportOptions._CsvExportGridExclusions = CsvExportGridExclusions.ExcludeAllInGrid;

            ExportOptions.HexEncodeBinaryData = chkConvertBase64BinaryHex.Checked;


            if (bAllowOK == true)
            {
                bChoseOk = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bChoseOk = false;
            this.Close();
        }

        private void chkIncludeUsersAdditionalProperties_CheckedChanged(object sender, EventArgs e)
        {
            txtIncludeUsersAdditionalPropertiesFile.Enabled = chkIncludeUsersAdditionalProperties.Checked;
            btnPickFolderIncludeUsersAdditionalProperties.Enabled = chkIncludeUsersAdditionalProperties.Checked;
        }
    }
}
