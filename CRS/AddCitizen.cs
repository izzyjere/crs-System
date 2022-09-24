using CRS.Domain;
using CRS.Frutronic.SDK;
using CRS.Futronic.SDK;
using CRS.Service;

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

namespace CRS
{
    public partial class AddCitizen : Form
    {
        public AddCitizen()
        {
            InitializeComponent();
            deviceAccessor = new();
            
            InitDevice();
        }
        DeviceAccessor deviceAccessor;
        FingerprintDevice device;
        CitizenRequest citizen = new();
        CitizenService service = new CitizenService();  
        async void InitDevice()
        {
            nrcNumber.Text = await service.GetNRCNumber();
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
                citizen.ThumbPrintData = image.ToByteArray();
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

        private async void button2_Click(object sender, EventArgs e)
        {
            if(citizen.ThumbPrintData!=null && citizen.ThumbPrintData.Any())
            {
                citizen.FirstName = firstName.Text;
                citizen.MiddleName = middleName.Text;
                citizen.DateOfBirth = dateofBirth.Value;
                citizen.Village = village.Text;
                citizen.District = district.Text;
                citizen.Chief = chief.Text;
                citizen.Gender = gender.Text;
                citizen.LastName = lastName.Text;
                citizen.NRC = nrcNumber.Text;
                citizen.PlaceOfBirth = placeOfBirth.Text;
                citizen.DateOfRegistration = DateTime.Now;
                var save = await service.CreateCitizen(citizen);
                if(save.Succeeded)
                {
                    MessageBox.Show("Save successifully");
                    citizen = new();
                    label009.Text = village.Text = district.Text = chief.Text = gender.Text = lastName.Text = middleName.Text = placeOfBirth.Text = String.Empty;
                    nrcNumber.Text = await service.GetNRCNumber();
                }
            }
            else
            {
                MessageBox.Show("Please scan the right thumb print.");
                return;
            }
        }
        Form owner;
        public void Show(Form form)
        {
            owner = form;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Close();                
        }
    }
}
