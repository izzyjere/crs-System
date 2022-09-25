using CRS.Client.Services;
using CRS.Domain;
using CRS.Frutronic.SDK;
using CRS.Futronic.SDK;

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

namespace CRS.Client
{
    public partial class AddSuspect : Form
    {
        public AddSuspect()
        {
            InitializeComponent();
            deviceAccessor = new DeviceAccessor();
            suspect.Bytes = new();
            InitDevice();
        }
        DeviceAccessor deviceAccessor;
        FingerprintDevice device;
        SuspectService suspectService = new();
        CitizenResponse Model = new();
        bool detilsPulled;
        SuspectRequest suspect = new();
        async void InitDevice(PictureBox pictureBox=null)
        {
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            device = connect.Data;
            device.FingerDetected += async (send, eventArgs) =>
            {
                scannerDisplay.Invoke(s => { s.Text = "Finger Detected."; s.ForeColor = Color.Green; });
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
                fingerImage.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (!detilsPulled)
                {
                    mainLabel.Invoke(s => { s.Text = "Please wait...."; s.ForeColor = Color.Green; });
                    Model = await suspectService.GetDetails((Bitmap)image);
                    if (Model== null)
                    {
                        mainLabel.Invoke(s => { s.Text = "Details not found."; s.ForeColor = Color.Red; });
                        suspectService = new();
                    }
                    else
                    {
                        mainLabel.Invoke(s => { s.Text = "Details  found."; s.ForeColor = Color.Green; });
                        nrcNumber.Invoke(i => i.Text = Model.NRC);
                        firstName.Invoke(i => i.Text = Model.FirstName);
                        lastName.Invoke(i => i.Text = Model.LastName);
                        gender.Invoke(i => i.Text = Model.Gender);
                        district.Invoke(i => i.Text = Model.District);
                        dateOfBirth.Invoke(i => i.Text = Model.DateOfBirth.ToString("dd MMM yyyy"));
                        chief.Invoke(i => i.Text = Model.Chief);
                        village.Invoke(i => i.Text = Model.Village);
                        placeOfBirth.Invoke(i => i.Text = Model.PlaceOfBirth);
                        detilsPulled = true;
                    }
                }
                else
                {
                    var img = image.Clone();
                    if(pictureBox==null)
                    {
                        MessageBox.Show("Click on the finger to scan.");
                        return;
                    }
                    pictureBox.Image = image;
                    suspect.Bytes.Add(((Image)img).ToByteArray()); 
                    
                }

                scannerDisplay.Invoke(s => { s.Text = "Fingerprint Captured."; s.ForeColor = Color.Green; });

            };
            device.FingerReleased += (send, eventArgs) =>
            {
                scannerDisplay.Invoke(i => i.Text = "Finger Released.");

                device.SwitchLedState(false, true);
            };

            device.StartFingerDetection();
            device.SwitchLedState(false, true);
            device.SwitchLedState(false, false);
        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if(suspect.Bytes.Count<10)
            {
                MessageBox.Show("Capture all 10 fingers first.");
                return;
            }
            suspect.Name = Model.FirstName + " "+ Model.LastName;
            suspect.NRC = Model.NRC;
            suspect.PhysicalAddress = address.Text;
            suspect.Complexion = complexion.Text;
            suspect.Occupation = occupation.Text;
            suspect.EyeColor = occupation.Text;
            var trySave = await suspectService.Add(suspect);
            if(trySave.Succeeded)
            {
                MessageBox.Show("Saved");
                Close();
            }
            else
            {
                MessageBox.Show("Could not save. Check if all info was added correctly.");
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }         
      
    }
}
