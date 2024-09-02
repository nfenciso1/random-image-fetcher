using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FetchRandomImage
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private readonly FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        private MemoryStream memoryStream;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) {
                if (!String.IsNullOrEmpty(textBoxWidth.Text))
                {
                    textBoxHeight.Text = textBoxWidth.Text;
                }
                else
                {
                    textBoxWidth.Text = textBoxHeight.Text;
                }
                
            }
        }

        private void TextBoxWidth_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBoxHeight.Text = textBoxWidth.Text;
            }
        }
        private void TextBoxHeight_TextChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBoxWidth.Text = textBoxHeight.Text;
            }
        }

        private async void ButtonFetch_ClickAsync(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxWidth.Text) && !String.IsNullOrEmpty(textBoxHeight.Text))
            {
                labelMessage1.Text = "Loading...";

                try
                {
                    System.IO.Stream imageStream;
                    memoryStream = new MemoryStream();
                    if (textBoxWidth.Text == textBoxHeight.Text)
                    {
                        imageStream = await client.GetStreamAsync(new Uri($"https://picsum.photos/{textBoxWidth.Text}"));
                    }
                    else
                    {
                        imageStream = await client.GetStreamAsync(new Uri($"https://picsum.photos/{textBoxWidth.Text}/{textBoxHeight.Text}"));
                    }

                    await imageStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    pictureBox1.Image = Image.FromStream(memoryStream);

                    labelMessage1.ResetText();
                    buttonDownload.Enabled = true;
                }
                catch (System.Net.Http.HttpRequestException)
                {
                    //DialogResult shouldClose = 
                    MessageBox.Show("An error occurred while fetching the image.", "Error", MessageBoxButtons.OK);
                    labelMessage1.Text = "Error";
                    //if (shouldClose == System.Windows.Forms.DialogResult.Yes)
                    //{
                    //    Application.Exit();
                    //}
                }
                catch (Exception) {
                    MessageBox.Show("An error was encountered.", "Error", MessageBoxButtons.OK);
                    labelMessage1.Text = "Error";
                }

                
            }
            else
            {
                labelMessage1.Text = "Width and Height fields should be filled.";
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            //System.Console.WriteLine("clicked picture");
            
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private async void ButtonDownload_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = Directory.GetCurrentDirectory();
            folderBrowserDialog.Description = "Select the directory where you want to save the image.";
            DialogResult result = folderBrowserDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                memoryStream.Position = 0;

                string filepath = Path.Combine(folderBrowserDialog.SelectedPath, String.Concat(DateTime.Now.ToString("MM-dd-yyyy,hh-mm-ss"), ".jpg"));
                using (FileStream fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await memoryStream.CopyToAsync(fileStream);
                }

                MessageBox.Show("The image has been downloaded. The filename is based on the date and time of download.", "Download Complete", MessageBoxButtons.OK);
            }
        }
    }
}
