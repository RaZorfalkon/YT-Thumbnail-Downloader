﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YouTubeThumbnailDownloader.Properties;

namespace YouTubeThumbnailDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Events

        private void Form1_Load(object sender, EventArgs e)
        {
            textBoxPath.Text = Settings.Default.downloadPath;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Settings.Default.Save();
        }

        private void linkLabelGitHub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/RaZorfalkon");
        }

        private void textBoxUrl_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxUrl.Text))
            {
                Image previewThumbnail = Downloader.GetThumbnailPreview(textBoxUrl.Text);

                if (previewThumbnail != null)
                    pictureBoxPreview.Image = previewThumbnail;
                else
                    pictureBoxPreview.Image = Resources.NoUrlImage;
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();

            if (result == DialogResult.Yes || result == DialogResult.OK)
            {
                Settings.Default.downloadPath = saveFileDialog.FileName;
                textBoxPath.Text = saveFileDialog.FileName;
            }
        }

        private void buttonDownload_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxPath.Text) || String.IsNullOrEmpty(textBoxUrl.Text))
            {
                MessageBox.Show("No Path or URL set!");
                return;
            }

            Image thumbnail = Downloader.GetThumbnail(textBoxUrl.Text);

            if (thumbnail == null)
                return;

            if (SaveThumbnail(thumbnail, textBoxPath.Text))
                MessageBox.Show("Download Successfully");
        }
        #endregion

        #region Private Method

        /// <summary>
        /// Saves Thumbnail to Drive
        /// </summary>
        /// <param name="thumbnail">Image to Save</param>
        /// <param name="location">Path</param>
        private bool SaveThumbnail(Image thumbnail, string location)
        {
            if (File.Exists(location))
            {
                DialogResult result = MessageBox.Show("File already exists! Overwrite ?", "Warning", MessageBoxButtons.YesNo);

                if (result == DialogResult.No)
                    return false;
            }

            FileStream imgStream = File.Create(location);

            byte[] thumbnailBytes = imageToByteArray();

            imgStream.Write(thumbnailBytes, 0, thumbnailBytes.Length);
            imgStream.Close();
            return true;

            byte[] imageToByteArray()
            {
                MemoryStream ms = new MemoryStream();
                thumbnail.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        #endregion
    }
}
