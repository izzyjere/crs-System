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
        void InitDevice()
        {
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            device = connect.Data;
            device.FingerDetected +=Device_FingerDetected;

            device.FingerReleased += (send, eventArgs) =>
            {
                scannerDisplay.Invoke(i => i.Text = "Finger Released.");

                device.SwitchLedState(false, true);
            };

            device.StartFingerDetection();
            device.SwitchLedState(false, true);
            device.SwitchLedState(false, false);
        }

        private async void Device_FingerDetected(object sender, EventArgs e)
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
            scannerDisplay.Invoke(s => { s.Text = "Fingerprint Captured."; s.ForeColor = Color.Green; });
        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            if (suspect.Bytes.Count<10)
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
            if (trySave.Succeeded)
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

        private void scanButton1_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);                 
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Any())
                {
                    suspect.Bytes.RemoveAt(0);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage1.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[0])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton2_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
               // fingerImage2.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <1)
                {
                    suspect.Bytes.RemoveAt(1);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage2.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[1])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton3_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
                //fingerImage3.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <2)
                {
                    suspect.Bytes.RemoveAt(2);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage3.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[2])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton4_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
                //fingerImage4.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <3)
                {
                    suspect.Bytes.RemoveAt(3);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage4.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[3])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton5_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
               //fingerImage5.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <4)
                {
                    suspect.Bytes.RemoveAt(4);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage5.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[4])));
                dev.StopFingerDetection();
            };                          
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton6_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
                //fingerImage6.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <5)
                {
                    suspect.Bytes.RemoveAt(5);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage6.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[5])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton7_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
                //fingerImage7.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <6)
                {
                    suspect.Bytes.RemoveAt(6);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage7.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[6])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton8_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
                //fingerImage8.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <7)
                {
                    suspect.Bytes.RemoveAt(7);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage8.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[7])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton9_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
               // fingerImage9.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <8)
                {
                    suspect.Bytes.RemoveAt(8);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage9.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[8])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }

        private void scanButton10_Click(object sender, EventArgs e)
        {
            if (!detilsPulled)
            {
                return;
            }
            device.StopFingerDetection();
            device.FingerDetected -=Device_FingerDetected;
            var connect = deviceAccessor.AccessFingerprintDevice();
            if (!connect.Succeeded)
            {
                MessageBox.Show(connect.Messages.First());
                return;

            }
            var dev = connect.Data;
            dev.FingerDetected +=(sendr, args) =>
            {
                device.SwitchLedState(true, false);
                // Save fingerprint to temporary folder
                var fingerprint = device.ReadFingerprint();
                scannerDisplay.Invoke(s => { s.Text = "Release Finger."; s.ForeColor = Color.Red; });
                var tempFile = Path.GetTempFileName();
                var tmpBmpFile = Path.ChangeExtension(tempFile, "bmp");
                fingerprint.Save(tmpBmpFile);
               // fingerImage10.Invoke(i => i.Image = Image.FromFile(tmpBmpFile));
                var image = Image.FromFile(tmpBmpFile);
                if (suspect.Bytes.Count <9)
                {
                    suspect.Bytes.RemoveAt(9);
                }
                suspect.Bytes.Add(image.ToByteArray());
                fingerImage10.Invoke(i => i.Image = Image.FromStream(new MemoryStream(suspect.Bytes[9])));
                dev.StopFingerDetection();
            };
            dev.StartFingerDetection();
            dev.SwitchLedState(false, true);
            dev.SwitchLedState(false, false);
        }
    }
}
