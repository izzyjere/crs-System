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
        }
        DeviceAccessor deviceAccessor;
        FingerprintDevice device;
        SuspectService suspectService = new();
        CitizenResponse Model = new();
        bool detilsPulled;
        async void InitDevice()
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
                if(!detilsPulled)
                {
                    Model = await suspectService.GetDetails((Bitmap)image);
                    if(Model== null)
                    {
                        mainLabel.Invoke(s => { s.Text = "Details not found."; s.ForeColor = Color.Red; });
                    }
                    else
                    {
                        mainLabel.Invoke(s => { s.Text = "Details  found."; s.ForeColor = Color.Green; });
                        nrcNumber.Text = Model.NRC;
                        firstName.Text = Model.FirstName;
                        lastName.Text = Model.LastName;
                        gender.Text = Model.Gender;
                        district.Text = Model.District;
                        dateOfBirth.Text = Model.DateOfBirth.ToString("dd MMM yyyy");
                        chief.Text = Model.Chief;
                        village.Text = Model.Village;
                        placeOfBirth.Text = Model.PlaceOfBirth;
                        detilsPulled = true;
                    }
                }
                else
                {

                }

                scannerDisplay.Invoke(s => { s.Text = "Fingerprint Captured."; s.ForeColor = Color.Green; });

            };
        }
            private void groupBox2_Enter(object sender, EventArgs e)
        {

        }
    }
}
