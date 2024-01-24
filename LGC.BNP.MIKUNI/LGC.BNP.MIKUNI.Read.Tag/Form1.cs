using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net.Http;
using System.Threading;
using Org.LLRP.LTK.LLRPV1;
using Org.LLRP.LTK.LLRPV1.DataType;
using Org.LLRP.LTK.LLRPV1.Impinj;
using static System.Net.WebRequestMethods;
using System.Security.Cryptography.X509Certificates;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using Microsoft.AspNetCore.SignalR.Client;
using Renci.SshNet.Messages;
using System.Timers;

namespace LGC.BNP.MIKUNI.Read.Tag
{

    public partial class Form1 : Form
    {

        static int reportCount = 0;
        static int eventCount = 0;
        private HubConnection hubConnection;

		public Form1()
        {
            InitializeComponent();
            //InitializeSignalRConnection();
            button2.Enabled = false;
            status_segnalr.Enabled = false;
            status_reader.Enabled = false;
            btn_start.Enabled = true;
            btn_con_signalr.Enabled = false;

			myTimer.Tick += new EventHandler(TimerEventProcessor);
            // Sets the timer interval to 5 seconds.
            myTimer.Interval = Properties.Settings.Default.intervalTime;

        }

		private async void InitializeSignalRConnection()
        {
            var host = Properties.Settings.Default.Socket;
            hubConnection = new HubConnectionBuilder()
                .WithUrl(host + "notifications")
            .Build();

            hubConnection.On<string>("onReceiveAlarm", async (message) =>
            {
                // Handle received message asynchronously if needed
                await AppendMessage(message);
            });

            try
            {
                await hubConnection.StartAsync();
                if (hubConnection.State == HubConnectionState.Connected)
                {
                    listBox2.Items.Add("Connect  WebSocket seccess");
                    btn_con_signalr.Enabled = false;
                    btn_start.Enabled = true;
                    status_segnalr.Text = "Connected";
                    status_segnalr.BackColor = Color.YellowGreen;
                    status_segnalr.ForeColor = Color.White;

                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show($"Connection error: {ex.Message}", "Error");
                listBox2.Items.Add(ex.Message);
                btn_con_signalr.Enabled = true;
                btn_start.Enabled = false;
                status_segnalr.Text = "Disconnect";
                status_segnalr.BackColor = Color.Red;
                status_segnalr.ForeColor = Color.White;

            }
        }
        private async Task AppendMessage(string message)
        {
            // Update your UI with the received message
            if (InvokeRequired)
            {
                Invoke(new Action(() => AppendMessage(message)));
            }
            else
            {
                try
                {
                    listBox1.Items.Add(message);
                    if (reader.IsConnected)
                    {
                        await Task.Run(() => GPO1_ON());
                    }
                    else
                    {
                        reader.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    listBox1.Items.Add("Fail" + message);
                    reader.Dispose();
                }
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }
        }


        LLRPClient reader = new LLRPClient();

        private async void button1_Click(object sender, EventArgs e) //start
        {
            button2.Enabled = true;
            btn_start.Enabled = false;
            status_reader.Text = "Connecting..";
            status_reader.BackColor = Color.Thistle;
            status_reader.ForeColor = Color.White;
			myTimer.Start();





			if (reader.ReaderName != null)
            {
                reader.Close();
                reader = new LLRPClient();
            }
            // string readerName = "impinj-15-50-20";
            string readerName = Properties.Settings.Default.Hostname;

            listBox1.Items.Clear();
            listBox2.Items.Clear();
            Impinj_Installer.Install();

            #region EventHandlers
            {
                Console.WriteLine("Adding Event Handlers\n");
                // reader.OnReaderEventNotification += new delegateReaderEventNotification(Reader_OnReaderEventNotification);
                reader.OnRoAccessReportReceived += new delegateRoAccessReport(Reader_OnRoAccessReportReceived);
            }
            #endregion

            #region Connecting
            {

                listBox2.Items.Add("Connecting.. Reader");
                // Open the reader connection.  Timeout after 5 seconds
                bool ret =  reader.Open(readerName, 5000, out ENUM_ConnectionAttemptStatusType status);

                if (!ret || status != ENUM_ConnectionAttemptStatusType.Success)
                {
                    listBox2.Items.Add("Failed to Connect to Reader");
                    status_reader.Text = "Disconnect";
                    status_reader.BackColor = Color.Red;
                    status_reader.ForeColor = Color.White;
                    button2.Enabled = false;
                    btn_start.Enabled = true;
                    return;
                }
                listBox2.Items.Add("Connect Success");
                status_reader.Text = "Connected";
                status_reader.BackColor = Color.YellowGreen;
                status_reader.ForeColor = Color.White;
                button2.Enabled = true;
            }
            #endregion

            #region EnableExtensions
            {
                Console.WriteLine("Enabling Impinj Extensions\n");

                MSG_IMPINJ_ENABLE_EXTENSIONS imp_msg = new MSG_IMPINJ_ENABLE_EXTENSIONS()
                {
                    MSG_ID = 1 // note this doesn't need to be set as the library will default
                };

                // Send the custom message and wait for 8 seconds
                MSG_CUSTOM_MESSAGE cust_rsp = reader.CUSTOM_MESSAGE(imp_msg, out MSG_ERROR_MESSAGE msg_err, 8000);

                if (cust_rsp is MSG_IMPINJ_ENABLE_EXTENSIONS_RESPONSE msg_rsp)
                {
                    if (msg_rsp.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                    {
                        Console.WriteLine(msg_rsp.LLRPStatus.StatusCode.ToString());
                        listBox2.Items.Add(msg_rsp.LLRPStatus.StatusCode.ToString());
                        reader.Close();
                        return;
                    }
                }
                else if (msg_err != null)
                {
                    Console.WriteLine(msg_err.ToString());
                    listBox2.Items.Add(msg_err.ToString());
                    reader.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("Enable Extensions Command Timed out\n");
                    listBox2.Items.Add("Enable Extensions Command Timed out");
                    reader.Close();
                    return;
                }
            }
            #endregion

            #region FactoryDefault
            {
                Console.WriteLine("Factory Default the Reader\n");

                // factory default the reader
                MSG_SET_READER_CONFIG msg_cfg = new MSG_SET_READER_CONFIG()
                {
                    ResetToFactoryDefault = true,
                    MSG_ID = 2 // this doesn't need to be set as the library will default
                };

                // if SET_READER_CONFIG affects antennas it could take several seconds to return
                MSG_SET_READER_CONFIG_RESPONSE rsp_cfg = reader.SET_READER_CONFIG(msg_cfg, out MSG_ERROR_MESSAGE msg_err, 12000);

                if (rsp_cfg != null)
                {
                    if (rsp_cfg.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                    {
                        Console.WriteLine(rsp_cfg.LLRPStatus.StatusCode.ToString());
                        reader.Close();
                        return;
                    }
                }
                else if (msg_err != null)
                {
                    Console.WriteLine(msg_err.ToString());
                    reader.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("SET_READER_CONFIG Command Timed out\n");
                    reader.Close();
                    return;
                }
            }
            #endregion

            #region ADDRoSpecWithObjects
            {
                Console.WriteLine("Adding RoSpec\n");

                // set up the basic parameters in the ROSpec. Use all the defaults from the reader
                MSG_ADD_ROSPEC msg = new MSG_ADD_ROSPEC()
                {
                    ROSpec = new PARAM_ROSpec
                    {
                        CurrentState = ENUM_ROSpecState.Disabled,
                        Priority = 0x00,
                        ROSpecID = 1111,

                        // setup the start and stop triggers in the Boundary Spec
                        ROBoundarySpec = new PARAM_ROBoundarySpec
                        {
                            ROSpecStartTrigger = new PARAM_ROSpecStartTrigger
                            {
                                ROSpecStartTriggerType = ENUM_ROSpecStartTriggerType.Null
                            },

                            ROSpecStopTrigger = new PARAM_ROSpecStopTrigger
                            {
                                ROSpecStopTriggerType = ENUM_ROSpecStopTriggerType.Null,
                                DurationTriggerValue = 0 // ignored by reader
                            }
                        },

                        // Add a single Antenna Inventory to the ROSpec
                        SpecParameter = new UNION_SpecParameter()
                    }
                };
                PARAM_AISpec aiSpec = new PARAM_AISpec()
                {
                    AntennaIDs = new UInt16Array(),
                    AISpecStopTrigger = new PARAM_AISpecStopTrigger
                    {
                        AISpecStopTriggerType = ENUM_AISpecStopTriggerType.Null
                    },

                    // use all the defaults from the reader.  Just specify the minimum required
                    InventoryParameterSpec = new PARAM_InventoryParameterSpec[1]
                };
                aiSpec.InventoryParameterSpec[0] = new PARAM_InventoryParameterSpec
                {
                    InventoryParameterSpecID = 1234,
                    ProtocolID = ENUM_AirProtocols.EPCGlobalClass1Gen2
                };
                aiSpec.AntennaIDs.Add(0);       // all antennas

                msg.ROSpec.SpecParameter.Add(aiSpec);

                MSG_ADD_ROSPEC_RESPONSE rsp = reader.ADD_ROSPEC(msg, out MSG_ERROR_MESSAGE msg_err, 12000);
                if (rsp != null)
                {
                    if (rsp.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                    {
                        Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString());
                        reader.Close();
                        return;
                    }
                }
                else if (msg_err != null)
                {
                    Console.WriteLine(msg_err.ToString());
                    reader.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("ADD_ROSPEC Command Timed out\n");
                    reader.Close();
                    return;
                }
            }
            #endregion

            #region EnableRoSpec
            {
                Console.WriteLine("Enabling RoSpec\n");
                MSG_ENABLE_ROSPEC msg = new MSG_ENABLE_ROSPEC()
                {
                    ROSpecID = 1111 // this better match the ROSpec we created above
                };
                MSG_ENABLE_ROSPEC_RESPONSE rsp = reader.ENABLE_ROSPEC(msg, out MSG_ERROR_MESSAGE msg_err, 12000);
                if (rsp != null)
                {
                    if (rsp.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                    {
                        Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString());
                        reader.Close();
                        return;
                    }
                }
                else if (msg_err != null)
                {
                    Console.WriteLine(msg_err.ToString());
                    reader.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("ENABLE_ROSPEC Command Timed out\n");
                    reader.Close();
                    return;
                }
            }
            #endregion

            #region StartRoSpec
            {
                Console.WriteLine("Starting RoSpec\n");
                listBox2.Items.Add("Running");
                MSG_START_ROSPEC msg = new MSG_START_ROSPEC()
                {
                    ROSpecID = 1111 // this better match the RoSpec we created above
                };
                MSG_START_ROSPEC_RESPONSE rsp = reader.START_ROSPEC(msg, out MSG_ERROR_MESSAGE msg_err, 12000);
                if (rsp != null)
                {
                    if (rsp.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                    {
                        Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString());
                        reader.Close();
                        return;
                    }
                }
                else if (msg_err != null)
                {
                    Console.WriteLine(msg_err.ToString());
                    listBox2.Items.Add(msg_err.ToString());
                    reader.Close();
                    return;
                }
                else
                {
                    Console.WriteLine("START_ROSPEC Command Timed out\n");
                    listBox2.Items.Add("START_ROSPEC Command Timed out");
                    reader.Close();
                    return;
                }
            }
			#endregion
			// wait around to collect some data
			myTimer.Start();


		}

        private void button2_Click(object sender, EventArgs e) //stop
        {
            try
            {
				myTimer.Stop();
				if (reader.IsConnected)
                {
                    button2.Enabled = false;
                    btn_start.Enabled = true;
                    listBox2.Items.Clear();
                    listBox2.Items.Add("Waiting");

                    status_reader.Text = "Disconnect";
                    status_reader.BackColor = Color.Red;
                    status_reader.ForeColor = Color.White;
                    
					#region StopRoSpec
					{
                        Console.WriteLine("Stopping RoSpec\n");
                        MSG_STOP_ROSPEC msg = new MSG_STOP_ROSPEC()
                        {
                            ROSpecID = 1111 // this better match the RoSpec we created above
                        };
                        MSG_STOP_ROSPEC_RESPONSE rsp = reader.STOP_ROSPEC(msg, out MSG_ERROR_MESSAGE msg_err, 12000);
                        if (rsp != null)
                        {
                            if (rsp.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                            {
                                Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString());
                                reader.Close();
                                return;
                            }
                        }
                        else if (msg_err != null)
                        {
                            Console.WriteLine(msg_err.ToString());
                            reader.Close();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("STOP_ROSPEC Command Timed out\n");
                            reader.Close();
                            reader.Dispose();
                            return;
                        }
                    }
                    #endregion

                    #region Clean Up Reader Configuration
                    {
                        Console.WriteLine("Factory Default the Reader\n");

                        // factory default the reader
                        MSG_SET_READER_CONFIG msg_cfg = new MSG_SET_READER_CONFIG()
                        {
                            ResetToFactoryDefault = true,
                            MSG_ID = 2 // note this doesn't need to be set as the library will default
                        };

                        // Note that if SET_READER_CONFIG affects antennas it could take several seconds to return
                        MSG_SET_READER_CONFIG_RESPONSE rsp_cfg = reader.SET_READER_CONFIG(msg_cfg, out MSG_ERROR_MESSAGE msg_err, 12000);

                        if (rsp_cfg != null)
                        {
                            if (rsp_cfg.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                            {
                                Console.WriteLine(rsp_cfg.LLRPStatus.StatusCode.ToString());
                                listBox1.Items.Add(rsp_cfg.LLRPStatus.StatusCode.ToString());
                                reader.Close();
                                return;
                            }
                        }
                        else if (msg_err != null)
                        {
                            Console.WriteLine(msg_err.ToString());
                            listBox2.Items.Add(msg_err.ToString());
                            reader.Close();
                            return;
                        }
                        else
                        {
                            Console.WriteLine("SET_READER_CONFIG Command Timed out\n");
                            listBox2.Items.Add("SET_READER_CONFIG Command Timed out");
                            reader.Close();
                            return;
                        }
                    }
                    #endregion
                    reader.Dispose();
                    reader = new LLRPClient();
                    listBox2.Items.Add("Stop");
                }
                else
                {
                    reader.Dispose();
                }
            }
            catch
            {
                reader.Dispose();
            }

        }
        private void setText(string Text)
        {
            try {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    listBox1.Items.Add(Text);
                    int nItems = (int)(listBox1.Height / listBox1.ItemHeight);
                    listBox1.TopIndex = listBox1.Items.Count - nItems;
                }));
            }
            catch (Exception err) {
                Console.WriteLine(err.Message);
            }
        }

        async public void Reader_OnRoAccessReportReceived(MSG_RO_ACCESS_REPORT msg)
        {
            try
            {

                if (msg.TagReportData == null) return;

                for (int i = 0; i < msg.TagReportData.Length; i++)
                {
                    reportCount++;
                    string epc;
                    long antenna;


					if (msg.TagReportData[i].EPCParameter[0].GetType() == typeof(PARAM_EPC_96))
                    {
                        epc = ((PARAM_EPC_96)(msg.TagReportData[i].EPCParameter[0])).EPC.ToHexString();
						antenna = (msg.TagReportData[i].AntennaID.AntennaID);
					}
                    else
                    {
                        epc = ((PARAM_EPCData)(msg.TagReportData[i].EPCParameter[0])).EPC.ToHexString();
                        antenna = (msg.TagReportData[i].AntennaID.AntennaID);

					}
					var CovertToSting = "";
					CovertToSting = getTag_id(epc);
                    await Task.Run(() => setText(CovertToSting));
                    await Task.Run(() => SendToSocket(CovertToSting, antenna));

				}
            }
            catch
            {
                reader.Dispose();

            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon.Visible = false;
        }
        private async Task GPO1_ON()
        {
            try
            {
                // this routine turns on 3.3v power in LLRP GPO1
                // LLRP GPO1 = Hardware PIN 14 / GPOUT0MSG_SET_READER_CONFIG 
                if (reader.IsConnected)
                {
                    MSG_ERROR_MESSAGE msg_err = new MSG_ERROR_MESSAGE();
                    MSG_SET_READER_CONFIG msg = new MSG_SET_READER_CONFIG();

                    msg.GPOWriteData = new PARAM_GPOWriteData[1];
                    msg.GPOWriteData[0] = new PARAM_GPOWriteData();
                    msg.GPOWriteData[0].GPOData = true;
                    msg.GPOWriteData[0].GPOPortNumber = 1;

                    MSG_SET_READER_CONFIG_RESPONSE rsp = reader.SET_READER_CONFIG(msg, out msg_err, 12000);
                    if (rsp != null)
                    {
                        int time = Properties.Settings.Default.Worktime;
                        await Task.Run(() => Thread.Sleep(time));
                        GPO1_OFF();
                    }
                    else if (msg_err != null)
                    {
                        listBox2.Items.Add(rsp.ToString());
                    }
                    else
                    {
                        listBox2.Items.Add("Commmand time out!");
                    }
                }
                else
                {
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                reader.Dispose();
                listBox2.Items.Add(ex.Message);
            }
        }
        private void GPO1_OFF()
        {
            try
            {
                if (reader.IsConnected)
                {
                    // this routine turns off power in LLRP GPO1
                    // LLRP GPO1 = Hardware PIN 14 / GPOUT0MSG_SET_READER_CONFIG 
                    MSG_SET_READER_CONFIG msg = new MSG_SET_READER_CONFIG();
                    msg.GPOWriteData = new PARAM_GPOWriteData[1];
                    msg.GPOWriteData[0] = new PARAM_GPOWriteData();
                    msg.GPOWriteData[0].GPOData = false;
                    msg.GPOWriteData[0].GPOPortNumber = 1;
                    MSG_ERROR_MESSAGE msg_err = new MSG_ERROR_MESSAGE();
                    MSG_SET_READER_CONFIG_RESPONSE rsp = reader.SET_READER_CONFIG(msg, out msg_err, 12000);
                    if (rsp != null)
                    {
                        //listBox2.Items.Add("GPO1 OFF");
                    }
                    else if (msg_err != null)
                    {
                        listBox2.Items.Add(rsp.ToString());
                    }
                    else
                        listBox2.Items.Add("Commmand time out!");
                }
                else
                {
                    reader.Dispose();
                }
            }
            catch
            {
                reader.Dispose();
            }
        }

        private async void btn_clear_DoubleClick(object sender, EventArgs e)
        {
			listBox1.Items.Clear();
			//await Task.Run(() => ClearText());
        }
        private String getTag_id(String text) {
            var tag_code = "";
            if (!string.IsNullOrEmpty(text))
            {
                switch (text)
                {
                    case "E2801191A5030062E59558C1":
                        tag_code = "DMIT10033";
                        break;
                    case "E2801191A5030062E59558C2":
                        tag_code = "DMIT10030";
                        break;
                    case "E2801191A5030062E59676DA":
                        tag_code = "DMIT10346";
                        break;
                    case "E2801191A5030062E59676D8":
                        tag_code = "DMIT10409";
                        break;
                    case "E280689400005029A3B58C3D":
                        tag_code = "DMIT10079";
                        break;
                    default:
                        tag_code = text;
                        break;
                }
            }
            return tag_code;
        }

        private void btn_con_signalr_Click(object sender, EventArgs e)
        {
            status_segnalr.Text = "Connecting..";
            status_segnalr.ForeColor = Color.White;
            status_segnalr.BackColor = Color.Thistle;
            InitializeSignalRConnection();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (reader.IsConnected)
            {
				GPO1_OFF();
				#region StopRoSpec
				{
                    Console.WriteLine("Stopping RoSpec\n");
                    MSG_STOP_ROSPEC msg = new MSG_STOP_ROSPEC()
                    {
                        ROSpecID = 1111 // this better match the RoSpec we created above
                    };
                    MSG_STOP_ROSPEC_RESPONSE rsp = reader.STOP_ROSPEC(msg, out MSG_ERROR_MESSAGE msg_err, 12000);
                    if (rsp != null)
                    {
                        if (rsp.LLRPStatus.StatusCode != ENUM_StatusCode.M_Success)
                        {
                            Console.WriteLine(rsp.LLRPStatus.StatusCode.ToString());
                            reader.Close();
                            return;
                        }
                    }
                    else if (msg_err != null)
                    {
                        Console.WriteLine(msg_err.ToString());
                        reader.Close();
                        return;
                    }
                    else
                    {
                        Console.WriteLine("STOP_ROSPEC Command Timed out\n");
                        reader.Close();
                        return;
                    }
                }
                #endregion
            }
        }

        public String ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public String ConvertHexToString(string hexString)
        {
            string ascii = string.Empty;

            for (int i = 0; i < hexString.Length; i += 2)
            {
                String hs = string.Empty;

                hs = hexString.Substring(i, 2);
                uint decval = System.Convert.ToUInt32(hs, 16);
                char character = System.Convert.ToChar(decval);
                ascii += character;

            }

            return ascii;
        }

        private async Task SendToSocket(string Tag ,long atenna_id)
        {
            try
			{
				var atenta_port_in = Properties.Settings.Default.AtentaPortIN;
				var atenta_port_out = Properties.Settings.Default.AtentaPortOut;
                var type = string.Empty;

				if (atenna_id == atenta_port_in) 
                {
                    type = "in";
                }
                else if(atenna_id == atenta_port_out)
                {
                    type = "out";
                }
                var host = Properties.Settings.Default.Socket;
                var url = host + "Notification/autobank/pushnoti";
                var client = new HttpClient();
				var data = new Dictionary<string, string>
                            {
                                {"tag_code", Tag},
								{"type", type},
							};
                var res =  client.PostAsync(url, new FormUrlEncodedContent(data));

			}
            catch (Exception e){
                throw e;
            }
        }
        private async Task RandomTag()
        {
            try {
				string[] tag_code = { "DMIT10081", "DMIT10117", "DMIT10346", "DMIT10409", "DMIT10079" };
				Random rnd = new Random();
				int random = rnd.Next(0, 5);
                int ran12 = rnd.Next(1, 3);
				var tag_ran = tag_code[random];
				await Task.Run(() => setText(tag_ran));
                await Task.Run(() => SendToSocket(tag_ran, ran12));
            }
            catch (Exception e) {
				throw e;
			}

            
			
		}
		static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();

		private async void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
		{
			    myTimer.Stop();

			// Displays a message box asking whether to continue running the timer.
			await Task.Run(() => RandomTag());
			myTimer.Enabled = true;
			

		}

	}
}
