using S;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Diagnostics;

using CSLibrary.Barcode.Constants;
using CSLibrary.Barcode.Structures;

namespace CSLibrary
{
    /// <summary>
    /// Barcode library
    /// </summary>
    public partial class Barcode
    {
        /// <summary>
        /// BarcodeEventHandler Delegate 
        /// </summary>
        /// <param name="e"></param>
        public delegate void BarcodeEventHandler(BarcodeEventArgs e);
        /// <summary>
        /// BarcodeStateEventHandler Delegate 
        /// </summary>
        /// <param name="e"></param>
        public delegate void BarcodeStateEventHandler(BarcodeStateEventArgs e);

        private static event BarcodeEventHandler m_captureCompleted;
        private static event BarcodeStateEventHandler m_stateChanged;
        /// <summary>
        /// BarcodeEventHandler : Capture completed event trigger
        /// </summary>
        public static event BarcodeEventHandler OnCapturedNotify
        {
            add { lock (synlock) m_captureCompleted += value; }
            remove { lock (synlock) m_captureCompleted -= value; }
        }
        /// <summary>
        /// BarcodeStateEventHandler : report current operation
        /// </summary>
        public static event BarcodeStateEventHandler OnStateChanged
        {
            add { lock (synlock) m_stateChanged += value; }
            remove { lock (synlock) m_stateChanged -= value; }
        }
        /// <summary>
        /// True will only return decoded data in <see cref="OnCapturedNotify"/>
        /// </summary>
        public static bool bCaptureDecoded
        {
            get { lock (synlock) return b_CaptureDecoded; }
            set { lock (synlock) b_CaptureDecoded = value; }
        }
        /// <summary>
        /// Current operation state
        /// </summary>
        public static BarcodeState State
        {
            get { lock (synlock) return m_state; }
            protected set { lock (synlock) m_state = value; }
        }

        private static BarcodeState m_state = BarcodeState.IDLE;
        // Helper for marshalling execution to GUI thread
        private static bool b_CaptureDecoded = true;
        private static int mStop = 0;
        private static object synlock = new object();

        private void TellThemCaptureCompleted(BarcodeEventArgs e)
        {
            if (m_captureCompleted != null)
            {
                m_captureCompleted(e);
            }
        }

        private void TellThemStateChanged(BarcodeStateEventArgs e)
        {
            if (m_stateChanged != null)
            {
                m_stateChanged(e);
            }
        }

        private void FireStateChangedEvent(BarcodeState e)
        {
            State = e;
            TellThemStateChanged(new BarcodeStateEventArgs(e));
        }

        private void FireCaptureCompletedEvent(BarcodeEventArgs e)
        {
            TellThemCaptureCompleted(e);
        }

        /// <summary>
        /// Start to capture barcode, until stop is sent.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            try
            {
                if (State != BarcodeState.IDLE)
                    return false;
                else
                {
                    throw new System.Exception(nResult.ToString());
                }
            }
            catch (System.Exception ex)
            {
                FireStateChangedEvent(BarcodeState.IDLE);
                return false;
            }

            FireStateChangedEvent(BarcodeState.BUSY);
            //bStop = false;
            return true;
        }

        /// <summary>
        /// Stop capturing
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            bool rc = true;
            try
            {
                FireStateChangedEvent(BarcodeState.STOPPING);
            }
            catch (System.Exception ex)
            {
                rc = false;
            }
            return rc;
        }
    }
}
