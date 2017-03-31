using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WintabDN;

namespace WintabControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CWintabContext _context = null;
        private CWintabData _data = null;
        bool enabled = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void showEnableButtonText()
        {
            if (enabled)
            {
                btnEnable.Content = "Disable";
            }
            else
            {
                btnEnable.Content = "Enable";
            }
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            showEnableButtonText();
            radDigitizerContext.IsChecked = true;
            
            if (System.Windows.Forms.SystemInformation.MonitorCount > 1)
            {
                this.Left = System.Windows.Forms.Screen.AllScreens[1].Bounds.Left + 
                    (System.Windows.Forms.Screen.AllScreens[1].Bounds.Width / 2) - 
                    (this.Width / 2);
            }
        }

        private CWintabContext OpenQueryDigitizerContext(bool enable)
        {
            bool status = false;
            CWintabContext logContext = null;
            
            try
            {
                //Use DigitizingContext to receive tablet values
                logContext = CWintabInfo.GetDefaultDigitizingContext(ECTXOptionValues.CXO_MESSAGES);
                
                if (enable)
                {
                    logContext.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;
                }

                if (logContext == null)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to get digitizing context");
                }

                logContext.Name = "Digitizer Context";

                WintabAxis tabletX = CWintabInfo.GetTabletAxis(EAxisDimension.AXIS_X);
                WintabAxis tabletY = CWintabInfo.GetTabletAxis(EAxisDimension.AXIS_Y);
                
                // In Wintab, the tablet origin is lower left.  Move origin to upper left
                // so that it coincides with screen origin.
                logContext.OutExtY = -logContext.OutExtY;

                status = logContext.Open();
                _data = new CWintabData(logContext);
                _data.SetWTPacketEventHandler(PacketHandler);

                btnEnable.IsEnabled = true;
                showEnableButtonText();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            
            return logContext;
        }

        private CWintabContext OpenSystemContext(bool enable)
        {
            bool status = false;
            CWintabContext logContext = null;

            try
            {
                logContext = CWintabInfo.GetDefaultSystemContext();
                
                if (logContext == null)
                {
                    System.Windows.Forms.MessageBox.Show("Failed to get digitizing context");
                }

                logContext.Name = "System Context";

                WintabAxis tabletX = CWintabInfo.GetTabletAxis(EAxisDimension.AXIS_X);
                WintabAxis tabletY = CWintabInfo.GetTabletAxis(EAxisDimension.AXIS_Y);
                
                logContext.OutExtY = -logContext.OutExtY;
                
                status = logContext.Open();
                _data = new CWintabData(logContext);
                _data.SetWTPacketEventHandler(PacketHandler);

                enabled = true;
                showEnableButtonText();
                btnEnable.IsEnabled = false;
                
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return logContext;
        }

        private void PacketHandler(Object sender, MessageReceivedEventArgs args)
        {

            uint pktId = (uint)args.Message.WParam;
            WintabPacket pkt = _data.GetDataPacket((uint)args.Message.LParam, pktId);
            
            if(pkt.pkContext == _context.HCtx)
            {

                lblSeq.Text = "Seq: " + pkt.pkSerialNumber.ToString();
                lblTime.Text = "Time: " + pkt.pkTime.ToString();
                
                lblX.Text = "X: " + pkt.pkX.ToString();
                lblY.Text = "Y: " + pkt.pkY.ToString();
                lblZ.Text = "Z: " + pkt.pkZ.ToString();
                
                lblP.Text = "P: " + pkt.pkNormalPressure.ToString();
                lblTP.Text = "TP: " + pkt.pkTangentPressure.ToString();
            }
        }

        private void CloseCurrentContext()
        {
            try
            {
                if (_context != null)
                {
                    _context.Close();
                    _context = null;
                    _data.ClearWTPacketEventHandler();
                    _data = null;
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void btnEnable_Click(object sender, RoutedEventArgs e)
        {
            CloseCurrentContext();

            if(radDigitizerContext.IsChecked == true)
            {
                _context = OpenQueryDigitizerContext(!enabled);
            }
            else if (radSystemContext.IsChecked == true)
            {
                _context = OpenSystemContext(!enabled);
            }
            
            enabled = !enabled;
            showEnableButtonText();
            
        }

        private void radSystemContext_Checked(object sender, RoutedEventArgs e)
        {
            CloseCurrentContext();
            _context = OpenSystemContext(enabled);
        }

        private void radDigitizerContext_Checked(object sender, RoutedEventArgs e)
        {
            CloseCurrentContext();
            _context = OpenQueryDigitizerContext(enabled);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
        }
    }

    internal class _data
    {
    }
}
