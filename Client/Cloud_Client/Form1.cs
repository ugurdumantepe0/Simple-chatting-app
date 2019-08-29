using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cloud_Client
{
    public partial class Form1 : Form
    {
        public static string _nickName = string.Empty;
        public static string _portNum;
        public static string _serverIP;
        public static string _downloadDirectory = string.Empty;
        public static string _filePath = string.Empty;
        private static readonly char[] _punctuation = "\\/:*?\"<>|".ToCharArray();

        Thread _thrReceive;
        bool _connected = false;
        private Socket _socket;
        private bool _processing = false;

        // error and messages
        private const string IP_ERROR = "IP not in correct fotmat.";
        private const string PORT_ERROR = "Please enter a numeric port number.";
        private const string PATH_ERROR = "Please select a path for downloaded files.";
        private const string CONNECTION_ERROR = "Could not connect to server, please try again.";
        private const string CONNECTION_SUCCESS = "Connection successful.";
        private const string DISCONNECTED_FROM_SERVER_ERROR = "Disconnected from server, please try again.";
        private const string USERNAME_EXISTS_ERROR = "Username is taken, please try another one and try again.";
        private const string USERNAME_EMPTY_ERROR = "Username cannot be empty.";
        private const string USERNAME_CHAR_ERROR = "Username can only contain alphanumeric characters.";
        private const string SERVER_SHUTDOWN_ERROR = "The server has been stopped manualy, you will be disconnected.";

        private const string SERVER_INTERNAL_ERROR =
            "The server was stopped unexpectedly, please wait and try connecting again.";

        private const string CLIENT_MANUALLY_DC_MSG = "Disconnected from server.";
        private const string FILE_UPLOAD_ERROR = " could NOT be uploaded because of a connection error.";
        private const string FILE_UPLOADED_MSG = " was successfully uploaded to server.";

        // client commands
        private const string NEW_USER_COMMAND = "_ADD_NEW_USER_|";
        private const string GET_ALL_FILES_COMMAND = "_GET_ALL_FILES_|";
        private const string END_MSG_COMMAND = "|_END_OF_MSG_";
        private const string CLIENT_DC_COMMAND = "_DSC_|";
        private const string FILE_UPLOAD_COMMAND = "_FILE_UPLOAD_REQUEST_|";
        private const string DOWNLOAD_SELECTED_FILE_COMMAND = "_DOWNLOAD_FILE_|";
        private const string DELETE_SELECTED_FILE_COMMAND = "_DELETE_FILE_|";
        private const string RENAME_SELECTED_FILE_COMMAND = "_RENAME_FILE_|";

        // server commands
        private const string SERVER_DC_COMMAND = "_DSC_";
        private const string USERNAME_EXISTS_COMMAND = "_ALREADYEXISTS_";
        private const string SERVER_SHUTDOWN_COMMAND = "_STOPPED_MANUALLY_";
        private const string SERVER_INTERNAL_ERROR_COMMAND = "_CLOSED_FORCIBLY_";
        private const string NO_FILES_TO_RETURN_COMMAND = "_NO_FILES_FOUND_";
        private const string FILE_NOT_FOUND_COMMAND = "_FILE_NOT_FOUND_";
        private const string FILE_UPLOAD_TO_CLIENT_COMMAND = "_UPLOADING_A_FILE_";
        private const string ADD_NEW_FILE_COMMAND = "_ADD_NEW_FILE_";
        private const string REMOVE_ALL_FILES_COMMAND = "_REMOVE_ALL_FILES_";
        private const string FILE_TO_DELETE_NOT_FOUND_COMMAND = "_FILE_TO_DELETE_NOT_FOUND_";
        private const string FILE_TO_RENAME_NOT_FOUND_COMMAND = "_FILE_TO_RENAME_NOT_FOUND_";
        private const string DELETE_SUCCESS_COMMAND = "_FILE_DELETED_";
        private const string RENAME_SUCCESS_COMMAND = "_FILE_RENAMED_";
        private const string FILE_CANNOT_BE_ACCESSED_TO_DELETE_COMMAND = "_FILE_CANNOT_BE_ACCESSED_TO_DELETE_";
        private const string FILE_CANNOT_BE_ACCESSED_TO_RENAME_COMMAND = "_FILE_CANNOT_BE_ACCESSED_TO_RENAME_";
        private const string NEW_FILE_NAME_ALREADY_EXISTS_COMMAND = "_NEW_FILE_NAME_ALREADY_EXISTS_";


        private const int BYTE_LENGTH = 2048; // used for sending commands to server
        private const int FILE_BUFFER_LENGTH = 8192; // used for file transfers


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // focus IP_BOX (put cursor)
            ipBox.Select();
            // Fix column sizes of list view
            FileDataList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.None);
            FileDataList.Columns[0].Width = 165;
            FileDataList.Columns[1].Width = 67;
            FileDataList.Columns[2].Width = 110;
            FileDataList.Columns[3].Width = 80;
            FileDataList.Columns[4].Width = 55;
        }

        private void Apply_Connected()
        {
            uploadButton.Text = "Upload New File";
            ipBox.ReadOnly = true;
            nickBox.ReadOnly = true;
            portBox.ReadOnly = true;
            uploadButton.Enabled = true;
            connectButton.Enabled = true;
            connectButton.Text = "Disconnect";
            downloadButton.Enabled = true;
            _processing = false;
        }

        private void Apply_Disconnected()
        {
            uploadButton.Enabled = false;
            uploadButton.Text = "Upload New File";
            ipBox.ReadOnly = false;
            nickBox.ReadOnly = false;
            portBox.ReadOnly = false;
            connectButton.Enabled = true;
            connectButton.Text = "Connect";
            downloadButton.Enabled = true;
            _processing = false;
        }

        private void Apply_Processing()
        {
            uploadButton.Enabled = false;
            connectButton.Enabled = false;
            ipBox.ReadOnly = true;
            nickBox.ReadOnly = true;
            portBox.ReadOnly = true;
            downloadButton.Enabled = false;
            _processing = true;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            // changing inputs are not allowed 
            Apply_Processing();

            // DISCONNECT
            if (connectButton.Text.Equals("Disconnect"))
            {
                ApllyDisconnectProtocol_NotifyServer(CLIENT_MANUALLY_DC_MSG);
            }
            // CONNECT
            else
            {
                // For checking if username is alphanumeric
                Regex r = new Regex("^[a-zA-Z0-9]*$");

                //remove extra spaces
                nickBox.Text = nickBox.Text.Trim();

                if (!IpCheck()) // IP format check
                {
                    MessageBox.Show(IP_ERROR);
                    Apply_Disconnected();
                }

                else if (!PortCheck()) // port number format check
                {
                    MessageBox.Show(PORT_ERROR);
                    Apply_Disconnected();
                }

                else if (nickBox.Text.Equals(string.Empty)) // nick not entered check
                {
                    MessageBox.Show(USERNAME_EMPTY_ERROR);
                    Apply_Disconnected();
                }

                else if (!r.IsMatch(nickBox.Text)) // nick alpanumerical check
                {
                    MessageBox.Show(USERNAME_CHAR_ERROR);
                    Apply_Disconnected();
                }

                else
                {
                    // read into local variables
                    _nickName = nickBox.Text + "|";
                    _portNum = portBox.Text;
                    _serverIP = ipBox.Text;

                    try
                    {
                        // CREATE SOCKET AND TRY CONNECTING
                        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                        _socket.Connect(Form1._serverIP, Convert.ToInt32(Form1._portNum)); // CONNECT TO SERVER

                        _connected = true;
                        string serverLoginMsg = NEW_USER_COMMAND + _nickName + END_MSG_COMMAND;
                        _socket.Send(Encoding.Default.GetBytes(serverLoginMsg));
                        _thrReceive = new Thread(Receive);
                        Apply_Connected();
                        reportBox.AppendText(GetTime() + CONNECTION_SUCCESS + Environment.NewLine);
                        _thrReceive.Start();
                    }
                    catch (Exception ex)
                    {
                        ApllyDisconnectProtocol_ServerRefused(CONNECTION_ERROR);
                    }
                }
            }
        }

        private void Receive()
        {
            while (_connected)
            {
                byte[] buffer = new byte[BYTE_LENGTH];
                try
                {
                    _socket.Receive(buffer);
                }
                catch (Exception)
                {
                    if (_connected)
                        ApllyDisconnectProtocol_NotifyServer(DISCONNECTED_FROM_SERVER_ERROR);
                }

                string received = Encoding.Default.GetString(buffer).TrimEnd('\0');
                // remove NULL characters from the end

                if (!string.IsNullOrEmpty(received))
                {
                    string command = received.Substring(0, received.IndexOf('|'));
                    if (command.Equals(SERVER_DC_COMMAND))
                    {
                        string[] arr = received.Split(new char[] { '|' }, 4);
                        /*  arr[0] -> COMMAND
                            arr[1] -> REASON
                            arr[2] -> "_END_OF_MSG_"  */

                        // username exists, cannot connect to server
                        if (arr[1].Equals(USERNAME_EXISTS_COMMAND))
                            ApllyDisconnectProtocol_ServerRefused(USERNAME_EXISTS_ERROR);

                        // server was manually closed
                        else if (arr[1].Equals(SERVER_SHUTDOWN_COMMAND))
                            ApllyDisconnectProtocol_ServerRefused(SERVER_SHUTDOWN_ERROR);

                        // server was closed unexpectedly
                        else if (arr[1].Equals(SERVER_INTERNAL_ERROR_COMMAND))
                            ApllyDisconnectProtocol_ServerRefused(SERVER_INTERNAL_ERROR);
                    }
                    else if (command.Equals(REMOVE_ALL_FILES_COMMAND))
                    {
                        FileDataList.Items.Clear();
                        reportBox.AppendText(GetTime() + "Getting available files from server..." + Environment.NewLine);
                    }

                    else if (command.Equals(NO_FILES_TO_RETURN_COMMAND))
                    {
                        FileDataList.Items.Clear();
                        reportBox.AppendText(GetTime() + "There are no available files." + Environment.NewLine);
                    }

                    else if (command.Equals(FILE_NOT_FOUND_COMMAND))
                        reportBox.AppendText(GetTime() + "FILE NOT FOUND." + Environment.NewLine);

                    else if (command.Equals(FILE_UPLOAD_TO_CLIENT_COMMAND))
                    {
                        removeEND(ref received);
                        string strChunkCount = received.Substring(received.IndexOf('|') + 1);
                        int chunkCount = int.Parse(strChunkCount);
                        Thread downloadThread = new Thread(() => DownloadFile(chunkCount));
                        downloadThread.Start();
                        break; // stop this thread
                    }

                    else if (command.Equals(ADD_NEW_FILE_COMMAND))
                    {
                        if (!string.IsNullOrEmpty(received))
                        {
                            removeEND(ref received);

                            string[] arr = received.Split(new char[] { '|' });

                            for (int i = 1; i < arr.Length; i++)
                            {
                                string[] infos = arr[i].Split('*');
                                // foreach (string info in infos)
                                FileDataList.Items.Add(new ListViewItem(infos));
                            }
                        }
                    }

                    else if (command.Equals(FILE_TO_DELETE_NOT_FOUND_COMMAND))
                        reportBox.AppendText(GetTime() + "File could not be deleted: FILE NOT FOUND." + Environment.NewLine);

                    else if (command.Equals(DELETE_SUCCESS_COMMAND))
                        reportBox.AppendText(GetTime() + "File was deleted successfully." + Environment.NewLine);

                    else if (command.Equals(FILE_CANNOT_BE_ACCESSED_TO_DELETE_COMMAND))
                        reportBox.AppendText(GetTime() + "File could not be deleted: FILE CANNOT BE ACCESSED." + Environment.NewLine);

                    else if (command.Equals(RENAME_SUCCESS_COMMAND))
                        reportBox.AppendText(GetTime() + "File was renamed successfully." + Environment.NewLine);

                    else if (command.Equals(FILE_TO_RENAME_NOT_FOUND_COMMAND))
                        reportBox.AppendText(GetTime() + "File could not be renamed: FILE NOT FOUND." + Environment.NewLine);

                    else if (command.Equals(FILE_CANNOT_BE_ACCESSED_TO_RENAME_COMMAND))
                        reportBox.AppendText(GetTime() + "File could not be renamed: FILE CANNOT BE ACCESSED." + Environment.NewLine);

                    else if (command.Equals(NEW_FILE_NAME_ALREADY_EXISTS_COMMAND))
                        reportBox.AppendText(GetTime() + "File could not be renamed: NEW FILE NAME ALREADY EXISTS." + Environment.NewLine);
                }
            }
        }
        private bool hasWriteAccessToFolder(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        void DownloadFile(int noOfPackets)
        {
            Apply_Processing();
            Stream fileStream = null;

            try
            {
                fileStream = new FileStream(_filePath, FileMode.Create, FileAccess.ReadWrite);

                // Buffer for reading data  
                byte[] bytes = new byte[FILE_BUFFER_LENGTH];

                int length = 0;
                bool serverConnected = true;
                for (int i = 0; i < noOfPackets && serverConnected; i++)
                {
                    double percantage = (double)i / (double)noOfPackets * 100; // download progress
                    string progress = "DOWNLOAD PROGRESS: " + percantage.ToString("0.0") + " % ";
                    uploadButton.Text = progress;
                    // RECEIVE CHUNK FROM CLIENT ***
                    if (_connected)
                        length = _socket.Receive(bytes, 0, bytes.Length, SocketFlags.None);
                    else
                        break;
                    Thread.Sleep(10);
                    fileStream.Write(bytes, 0, length);
                    serverConnected = Ping();
                }

                fileStream?.Close(); // closes fileStream if its not null

                if (_connected && serverConnected)
                    reportBox.AppendText(GetTime() + "File successfully downloaded." + Environment.NewLine);
                else
                {
                    reportBox.AppendText(GetTime() + "File could NOT be downloaded." + Environment.NewLine);
                    if (System.IO.File.Exists(_filePath))
                    {
                        try
                        {
                            System.IO.File.Delete(_filePath); // remove incomplete upload
                            reportBox.AppendText(GetTime() + "Incomplate download has been removed." + Environment.NewLine);
                        }
                        catch (Exception) { }
                    }
                }
            }
            catch (Exception ex)
            {
                fileStream?.Close();
                reportBox.AppendText(GetTime() + ex.Message + Environment.NewLine);
            }
            finally
            {
                if (_connected)
                {
                    _thrReceive = new Thread(Receive);
                    _thrReceive.Start();
                    Apply_Connected();
                }
                else
                    Apply_Disconnected();
            }
        }

        // REMOVE _END_OF_MSG_ PROT
        private void removeEND(ref string msg)
        {
            msg = msg.Substring(0, msg.Length - 13);
        }

        // DISCONNECT PROTOCOL
        void ApllyDisconnectProtocol_NotifyServer(string COMMAND)
        {
            Apply_Processing();
            try
            {
                if (_socket != null)
                {
                    if (SocketConnected(_socket) && _connected)
                        _socket.Send(Encoding.Default.GetBytes(CLIENT_DC_COMMAND + _nickName + END_MSG_COMMAND));
                    _socket.Close();
                }
            }
            catch (Exception) { }
            finally
            {
                _connected = false;
                // changing inputs are allowed
                Apply_Disconnected();

                if (!COMMAND.Equals(string.Empty))
                    reportBox.AppendText(GetTime() + COMMAND + Environment.NewLine);
            }
        }

        bool Ping()
        {
            try
            {
                _socket.Send(Encoding.Default.GetBytes("_PING_|" + _nickName + END_MSG_COMMAND));
                return true;
            }
            catch (Exception)
            {
                ApllyDisconnectProtocol_NotifyServer("Disconnected from server.");
                return false;
            }
        }

        // Disconnected by server
        void ApllyDisconnectProtocol_ServerRefused(string COMMAND)
        {
            _connected = false;
            Apply_Processing();
            try
            {
                if (_socket != null)
                    _socket.Close();
            }
            finally
            {
                // changing inputs are allowed
                Apply_Disconnected();

                if (!COMMAND.Equals(string.Empty))
                    reportBox.AppendText(GetTime() + COMMAND + Environment.NewLine);
            }
        }

        // CHECK IF A SOCKET IS CONNECTED
        bool SocketConnected(Socket s)
        {
            try
            {
                bool part1 = s.Poll(1000, SelectMode.SelectRead);
                bool part2 = (s.Available == 0);
                if (part1 && part2)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        // CHECK IP FORMAT
        private bool IpCheck()
        {
            if (ipBox.Text.Equals(string.Empty) || ipBox.Text.IsValidIp())
                return true;
            return false;
        }

        // CHECK PORT FORMAT
        private bool PortCheck()
        {
            if (Regex.IsMatch(portBox.Text, @"^\d+$"))
                return true;
            return false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_connected)
                ApllyDisconnectProtocol_NotifyServer(CLIENT_MANUALLY_DC_MSG);
            else
                ApllyDisconnectProtocol_ServerRefused(string.Empty);
        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            // Chose file to be uploaded
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "All Files (*.*)|*.*";
            dialog.CheckFileExists = true;
            dialog.Title = "Choose a file to upload";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string dialogFileName = dialog.FileName;

                // START UPLOAD_FILE THREAD
                Thread uploadThread = new Thread(() => UploadFile(dialogFileName));
                uploadThread.Start();
            }
        }

        // OPEN A THREAD FOR UPLADING THE FILE
        public void UploadFile(string filePath)
        {
            FileStream fileStream = null;
            string fileName = new DirectoryInfo(filePath).Name;
            Apply_Processing(); // lock user controls

            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read); // open file stream for file with read access
                // calculate the number of chunks it will take to send the file
                int noOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileStream.Length) / Convert.ToDouble(FILE_BUFFER_LENGTH)));
                int totalLength = (int)fileStream.Length;
                // notify the server for a file upload
                string noticeCommand = FILE_UPLOAD_COMMAND + _nickName + noOfPackets + "|" + fileName + END_MSG_COMMAND;
                _socket.Send(Encoding.Default.GetBytes(noticeCommand));

                // start sending chunks
                for (int i = 0; i < noOfPackets; i++)
                {
                    double percantage = (double)i / (double)noOfPackets * 100;  // upload progress
                    string progress = "UPLOAD PROGRESS: " + percantage.ToString("0.0") + " % ";
                    uploadButton.Text = progress;

                    int currentPacketLength;

                    if (totalLength > FILE_BUFFER_LENGTH)
                    {
                        currentPacketLength = FILE_BUFFER_LENGTH;
                        totalLength = totalLength - currentPacketLength;
                    }
                    else
                        currentPacketLength = totalLength;

                    byte[] bytesToSend = new byte[currentPacketLength];
                    fileStream.Read(bytesToSend, 0, currentPacketLength);
                    // SEND CHUNK TO SERVER ***
                    _socket.Send(bytesToSend, 0, (int)bytesToSend.Length, SocketFlags.None);
                }
                reportBox.AppendText(GetTime() + fileName + FILE_UPLOADED_MSG + Environment.NewLine);
            }
            catch (Exception)
            {
                reportBox.AppendText(GetTime() + fileName + FILE_UPLOAD_ERROR + Environment.NewLine);
            }
            finally
            {
                // close file stream
                if (fileStream != null)
                    fileStream.Close();
                if (_connected)
                    Apply_Connected();
                else
                    Apply_Disconnected();
            }
        }

        // RETURNS CURRENT TIME
        private string GetTime()
        {
            return "[" + DateTime.Now.ToString("HH:mm") + "] ";
        }

        // AUTO SCROLL FOR REPORT BOX
        private void reportBox_TextChanged(object sender, EventArgs e)
        {
            reportBox.SelectionStart = reportBox.Text.Length;
            reportBox.ScrollToCaret();
        }

        // SEND A REQUEST TO SERVER TO GET ALL AVAILABLE FILE INFORMATION
        private void getfilesButton_Click(object sender, EventArgs e)
        {
            if (!_connected && !_processing && connectButton.Enabled)
                connectButton.Focus();

            else if (_connected && !_processing)
            {
                string getAllFilesCmd = GET_ALL_FILES_COMMAND + _nickName + END_MSG_COMMAND;
                try
                {
                    _socket.Send(Encoding.Default.GetBytes(getAllFilesCmd));
                }
                catch (Exception)
                {
                    ApllyDisconnectProtocol_NotifyServer(DISCONNECTED_FROM_SERVER_ERROR);
                }
            }
        }

        // SEND A REQUEST TO SERVER TO DOWNLOAD A FILE
        private void downloadButton_Click(object sender, EventArgs e)
        {
            // Apply_Processing(); // lock user controls

            if (_connected && FileDataList.SelectedItems.Count > 0)
            {
                // CHOOSE CLIENT UPLOAD LOCATION
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                DialogResult result = DialogResult.Cancel;
                result = fbd.ShowDialog();

                if (result != DialogResult.OK)
                    MessageBox.Show(PATH_ERROR);

                else
                {
                    _downloadDirectory = fbd.SelectedPath;
                    string fileName = FileDataList.SelectedItems[0].Text;

                    _filePath = Path.Combine(_downloadDirectory, fileName);


                    /*if (!hasWriteAccessToFolder(filePath))
                    {
                        MessageBox.Show("You don't have access to this folder. Please choose another one.");
                    }*/
                    if (File.Exists(_filePath))
                    {
                        MessageBox.Show("File already exists in download directory.");
                        if (_connected)
                            Apply_Connected();
                        else
                            Apply_Disconnected();
                    }
                    else
                    {
                        //MessageBox.Show(DOWNLOAD_SELECTED_FILE_COMMAND + _nickName + fileName + END_MSG_COMMAND);
                        // Send request to server
                        if (_connected)
                        {
                            try
                            {
                                _socket.Send(
                                    Encoding.Default.GetBytes(DOWNLOAD_SELECTED_FILE_COMMAND + _nickName + fileName +
                                                              END_MSG_COMMAND));
                            }
                            catch (Exception)
                            {
                                ApllyDisconnectProtocol_NotifyServer("File download failed, disconnected from server...");
                            }
                        }
                    }
                }
            }
            else
            {
                if (!_connected && connectButton.Enabled)
                    connectButton.Focus();
                else if (_connected && FileDataList.Items.Count > 0 && FileDataList.SelectedItems.Count == 0)
                    MessageBox.Show("Please select a file.");
                else if (_connected && FileDataList.Items.Count == 0)
                    getfilesButton.Focus();
                // unlock user controls
                if (_connected)
                    Apply_Connected();
                else
                    Apply_Disconnected();
            }
        }

        private void deleteTypingButton_Click(object sender, EventArgs e)
        {

            string fileName = deleteTextBox.Text;
            if (fileName == "")
                MessageBox.Show("Please enter a filename to delete.");

            else if (!_connected && !_processing && connectButton.Enabled)
                connectButton.Focus();

            else if (_connected && !_processing)
            {
                try
                {
                    _socket.Send(Encoding.Default.GetBytes(DELETE_SELECTED_FILE_COMMAND + _nickName + fileName + END_MSG_COMMAND));
                }
                catch (Exception)
                {
                    ApllyDisconnectProtocol_NotifyServer("Cannot delete file, disconnected from server...");
                }
            }

        }

        private void renameButton_Click(object sender, EventArgs e)
        {
            string oldFileName = oldFileNameBox.Text;
            string newFileName = newFileNameBox.Text;

            if (ContainsPunctuation(oldFileName) || ContainsPunctuation(newFileName))
                MessageBox.Show("A file name can't contain any of the following characters:" + Environment.NewLine + "\\ / : * ? \" < > |");

            else if (oldFileName == "" || newFileName == "")
                MessageBox.Show("Please enter both file names.");

            else if (!_connected && !_processing && connectButton.Enabled)
                connectButton.Focus();

            else if (_connected && !_processing)
            {
                try
                {
                    _socket.Send(Encoding.Default.GetBytes(RENAME_SELECTED_FILE_COMMAND + _nickName + oldFileName + "/" + newFileName + END_MSG_COMMAND));
                }
                catch (Exception)
                {
                    ApllyDisconnectProtocol_NotifyServer("Cannot rename file, disconnected from server...");
                }
            }
        }

        public static bool ContainsPunctuation(string text)
        {
            return text.IndexOfAny(_punctuation) >= 0;
        }
    }


    // Checks if the IP is actually a valid IP
    internal static class IpExtensions
    {
        public static bool IsValidIp(this string address)
        {
            if (!Regex.IsMatch(address, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b"))
                return false;

            IPAddress dummy;
            return IPAddress.TryParse(address, out dummy);
        }
    }
}
